/*
 * HttpResponseEventArgs.cs
 *
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/10/2011 2:05:11 AM
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CocoB.Rest.WindowsPhone.Network
{
    internal class HttpResponseEventArgs : EventArgs
    {
        private readonly HttpWebResponse _webResponse;

        public HttpResponseEventArgs(
            HttpWebResponse webResponse)
        {
            _webResponse = webResponse;
        }

        public HttpStatusCode StatusCode
        {
            get { return _webResponse.StatusCode; }
        }

        public string Method
        {
            get { return _webResponse.Method; }
        }

        public Uri Uri
        {
            get { return _webResponse.ResponseUri; }
        }

        public Dictionary<string, string> Headers
        {
            get
            {
                var headers = new Dictionary<string, string>();
                foreach (var key in _webResponse.Headers.AllKeys)
                {
                    headers[key] = _webResponse.Headers[key];
                }
                return headers;
            }
        }

        public byte[] Response
        {
            get { return GetResponseBody(_webResponse); }
        }

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
    }
}
