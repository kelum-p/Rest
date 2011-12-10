/*
 * HttpWebClientImpl.cs
 * 
 * Author: Kelum Peiris
 * Date: 11/27/2011 2:38:20 AM
 *
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using CocoB.Rest.WindowsPhone.Core.Concurrency;
using CocoB.Rest.WindowsPhone.Core.Logger;
using Microsoft.Phone.Net.NetworkInformation;

namespace CocoB.Rest.WindowsPhone.Network
{
    internal class HttpWebClientImpl : HttpWebClient
    {
        #region Member Variables

        private const long DEFAULT_TIMEOUT = 20*1000;

        private static readonly Logger Log = Logger.GetCurrentClassLogger();

        private readonly Worker _networkWorker;
        private readonly long _timeout;

        #endregion

        #region Constructors

        public HttpWebClientImpl(
            Worker worker,
            Dictionary<string, string> headers,
            long timeout = DEFAULT_TIMEOUT)
        {
            _networkWorker = worker;
            Headers = headers;
            _timeout = timeout;
        }

        public HttpWebClientImpl(Worker worker)
            : this(worker, new Dictionary<string, string>())
        {
        }

        #endregion

        #region Overrides of HttpWebClient

        public override bool HasNetworkConnection
        {
            get { return NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None; }
        }

        public override void DoGETRequest(Uri uri)
        {
            if (HasNetworkConnection)
            {
                QueueGETResquest(uri);
            }
            else
            {
                throw new HttpWebClientException(uri, "Network connection unavailable");
            }
        }

        private void QueueGETResquest(Uri uri)
        {
            Log.Info("GET URL: {0}", uri.OriginalString);

            _networkWorker.QueueJob(
                () =>
                {
                    try
                    {
                        var webRquest = SetupWebRequest(uri, "GET");
                        BeginGetResponseWithTimeOut(webRquest);
                    }
                    catch (WebException e)
                    {
                        NotifyWebException(e);
                    }
                });
        }


        public override void DoPostRequest(
            Uri uri, byte[] postContents, string contentType = "application/x-www-form-urlencoded")
        {
            throw new NotImplementedException();
        }

        private HttpWebRequest SetupWebRequest(Uri uri, string method)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(uri);
            webRequest.Method = method;
            webRequest.AllowAutoRedirect = true;
            webRequest.AllowReadStreamBuffering = true;

            foreach (var key in Headers.Keys)
            {
                webRequest.Headers[key] = Headers[key];
            }

            return webRequest;
        }

        private void BeginGetResponseWithTimeOut(HttpWebRequest webRequest)
        {
            WaitHandle waitHandle = new AutoResetEvent(false);
            var timeoutHandle = RegisterTimeout(webRequest, waitHandle);
            object[] requestParams = {webRequest, waitHandle, timeoutHandle};

            webRequest.BeginGetResponse(ResponseCallback, requestParams);
        }

        private void ResponseCallback(IAsyncResult result)
        {
            var requestParams = result.AsyncState as object[];
            if (requestParams == null)
            {
                throw new ArgumentNullException(
                    "result", "result.AsyncState cannot be null");
            }

            var webRequest = requestParams[0] as HttpWebRequest;
            if (webRequest == null)
            {
                throw new ArgumentNullException(
                    "result", "result.AsyncState[0] (webRequest) cannot be null");
            }

            var waitHandle = requestParams[1] as WaitHandle;
            if (waitHandle == null)
            {
                throw new ArgumentNullException(
                    "result", "result.AsyncState[1] (waithandle) cannot be null");
            }

            var timeoutHandle = requestParams[2] as RegisteredWaitHandle;
            if (timeoutHandle == null)
            {
                throw new ArgumentNullException(
                    "result", "result.AsyncState[2] (timeoutHandle) cannot be null");
            }

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = EndGetResponseWithTimeout(
                    webRequest, result, waitHandle, timeoutHandle);
                TriggerResponseAvailableEvent(webResponse);
            }
            catch (WebException e)
            {
                NotifyWebException(e);
            }
            finally
            {
                UnregisterTimeout(waitHandle, timeoutHandle);
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
        }

        private static HttpWebResponse EndGetResponseWithTimeout(
            WebRequest webRequest,
            IAsyncResult result,
            WaitHandle waitHandle,
            RegisteredWaitHandle timeoutHandle)
        {
            var webResponse = webRequest.EndGetResponse(result) as HttpWebResponse;
            UnregisterTimeout(waitHandle, timeoutHandle);
            return webResponse;
        }

        private RegisteredWaitHandle RegisterTimeout(
            HttpWebRequest webRequest, WaitHandle waitHandle)
        {
            return ThreadPool.RegisterWaitForSingleObject(
                waitHandle, TimeoutCallback, webRequest, _timeout, true);
        }

        private static void TimeoutCallback(object state, bool timeout)
        {
            if (!timeout) return;

            var webRequest = state as HttpWebRequest;

            if (webRequest == null) return;

            Log.Warn("Request to URL: {0} timed out.", webRequest.RequestUri.OriginalString);
            webRequest.Abort();
        }

        private static void UnregisterTimeout(
            WaitHandle waitHandle, RegisteredWaitHandle timeoutHandle)
        {
            timeoutHandle.Unregister(waitHandle);
        }

        private void NotifyWebException(WebException exception)
        {
            var webResponse = exception.Response as HttpWebResponse;
            if (webResponse != null)
            {
                var method = webResponse.Method;
                Log.Exception(
                    exception, "{0} Request for URI: {1} failed", method, webResponse.ResponseUri);
                TriggerResponseAvailableEvent(webResponse);
            }
        }

        #endregion
    }
}