/*
 * HttpWebClient.cs
 * 
 * Author: Kelum Peiris
 * Date: 11/27/2011 2:37:27 AM
 *
 */

using System;
using System.IO;
using System.Net;
using System.Text;
using CocoB.Rest.WindowsPhone.Core.Concurrency;
using CocoB.Rest.WindowsPhone.Core.Logger;

namespace CocoB.Rest.WindowsPhone.Network
{
    internal class HttpResponseEventArgs : EventArgs
    {

        public HttpResponseEventArgs(
            HttpStatusCode statusCode,
            string method,
            Uri uri,
            byte [] response)
        {
            StatusCode = statusCode;
            Method = method;
            Uri = uri;
            Response = response;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public string Method { get; set; }
        public Uri Uri { get; set; }
        public byte[] Response { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Status Code: " + StatusCode);
            builder.AppendLine("Method: " + Method);
            builder.AppendLine("Uri: " + Uri);

            var responseString = Encoding.UTF8.GetString(Response, 0, Response.Length);
            builder.AppendLine("Response (UTF8): " + responseString);
            return builder.ToString();
        }
    }

    internal delegate void HttpResponseEventHandler(object sender, HttpResponseEventArgs e);

    internal abstract class HttpWebClient
    {
        private static readonly Logger Log = Logger.GetCurrentClassLogger();

        private static readonly Worker NetwotkWorker = new WorkerImpl();

        #region Constructors

        public static HttpWebClient Create()
        {
            return new HttpWebClientImpl(NetwotkWorker);
        }

        #endregion

        #region Properties

        public abstract bool HasNetworkConnection { get; }

        #endregion

        #region Events

        public event HttpResponseEventHandler ResponseAvailable;

        protected void TriggerResponseAvailableEvent(HttpWebResponse webResponse)
        {
            NetwotkWorker.QueueJob(() => ReplyResponse(webResponse));
        }

        private void ReplyResponse(HttpWebResponse webResponse)
        {
            if (webResponse == null)
            {
                throw new ArgumentNullException("webResponse");
            }

            var handler = ResponseAvailable;
            if (handler != null)
            {
                var statusCode = webResponse.StatusCode;
                var method = webResponse.Method;
                var uri = webResponse.ResponseUri;
                var response = GetResponseBody(webResponse);

                var eventArgs = new HttpResponseEventArgs(statusCode, method, uri, response);

                Log.Info("Response: " + Environment.NewLine + eventArgs);
                handler(this, eventArgs);
            }
        }

        private static byte[] GetResponseBody(WebResponse webResponse)
        {
            var responseBody = new byte[0];
            if (webResponse != null)
            {
                var responseStream = webResponse.GetResponseStream();

                using (var binaryReader = new BinaryReader(responseStream))
                {
                    try
                    {
                        responseBody = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                    }
                    catch (IOException)
                    {
                        responseBody = new byte[0];
                    }
                }
            }

            return responseBody;
        }

        #endregion

        #region Methods

        public abstract void DoGETRequest(Uri uri);

        public abstract void DoPostRequest(
            Uri uri, byte[] postContents,string contentType = "application/x-www-form-urlencoded");

        #endregion

    }
}
