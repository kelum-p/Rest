/*
 * HttpWebClientException.cs
 * 
 * Author: Kelum Peiris
 * Date: 11/27/2011 1:42:16 PM
 *
 */

using System;
using System.Net;

namespace CocoB.Rest.WindowsPhone.Network
{
    public class HttpWebClientException : Exception
    {
        #region Constructors

        public HttpWebClientException(Uri uri, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
            : base(message)
        {
            Uri = uri;
            StatusCode = statusCode;
        }

        #endregion

        #region Properties

        public HttpStatusCode StatusCode { get; private set; }
        public Uri Uri { get; private set; }

        #endregion
    }
}
