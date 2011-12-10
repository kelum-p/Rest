/*
 * NetworkException.cs
 *
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/10/2011 2:08:34 AM
 *
 */

using System;
using System.Collections.Generic;
using System.Net;
using CocoB.Rest.WindowsPhone.Network;

namespace CocoB.Rest.WindowsPhone.Core.Exceptions
{
    public class NetworkException : Exception
    {
        #region Member Variables

        private readonly HttpResponseEventArgs _httpResponseEventArgs;

        #endregion

        #region Constructors

        internal NetworkException(HttpResponseEventArgs httpResponseEventArgs)
        {
            _httpResponseEventArgs = httpResponseEventArgs;
        }

        #endregion

        #region Properties

        public HttpStatusCode StatusCode
        {
            get { return _httpResponseEventArgs.StatusCode; }
        }

        public string Method
        {
            get { return _httpResponseEventArgs.Method; }
        }

        public Uri Uri
        {
            get { return _httpResponseEventArgs.Uri; }
        }

        public Dictionary<string, string> Headers
        {
            get { return _httpResponseEventArgs.Headers; }
        }

        public byte[] Response
        {
            get { return _httpResponseEventArgs.Response; }
        }

        #endregion

        #region Methods

        public override string Message
        {
            get
            {
                return _httpResponseEventArgs.ToString();
            }
        }

        #endregion

    }
}
