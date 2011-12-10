/*
 * HttpWebClient.cs
 * 
 * Author: Kelum Peiris
 * Date: 11/27/2011 2:37:27 AM
 *
 */

using System;
using System.Collections.Generic;
using System.Net;
using CocoB.Rest.WindowsPhone.Core.Concurrency;
using CocoB.Rest.WindowsPhone.Core.Logger;

namespace CocoB.Rest.WindowsPhone.Network
{
    internal delegate void HttpResponseEventHandler(object sender, HttpResponseEventArgs e);

    internal abstract class HttpWebClient
    {
        #region Member Variables

        private static readonly Logger Log = Logger.GetCurrentClassLogger();

        private static readonly Worker NetworkWorker = new WorkerImpl("Network Worker");

        #endregion

        #region Constructors

        public static HttpWebClient Create()
        {
            return new HttpWebClientImpl(NetworkWorker);
        }

        public static HttpWebClient Create(Dictionary<string, string> headers)
        {
            return new HttpWebClientImpl(NetworkWorker, headers);
        }

        #endregion

        #region Properties

        public abstract bool HasNetworkConnection { get; }

        public Dictionary<string, string> Headers { get; set; }

        #endregion

        #region Events

        public event HttpResponseEventHandler ResponseAvailable;

        protected void TriggerResponseAvailableEvent(HttpWebResponse webResponse)
        {
            NetworkWorker.QueueJob(() => ReplyResponse(webResponse));
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
                var eventArgs = new HttpResponseEventArgs(webResponse);

                Log.Info("Response: " + Environment.NewLine + eventArgs);
                handler(this, eventArgs);
            }
        }

        #endregion

        #region Methods

        public abstract void DoGETRequest(Uri uri);

        public abstract void DoPostRequest(
            Uri uri, byte[] postContents, string contentType = "application/x-www-form-urlencoded");

        #endregion
    }
}