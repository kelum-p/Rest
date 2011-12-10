/*
 * Model.cs
 * 
 * Author: Kelum Peiris
 * Date: 11/27/2011 2:15:28 AM
 *
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CocoB.Rest.WindowsPhone.Core.Exceptions;
using CocoB.Rest.WindowsPhone.Core.Serializers;
using CocoB.Rest.WindowsPhone.Network;

namespace CocoB.Rest.WindowsPhone.Model
{
    public class Model
    {
        private readonly HttpWebClient _webClient;
        private readonly Serializer _serializer;

        private Dictionary<string, object> _data;

        public Model()
            : this(HttpWebClient.Create(), Serializer.Create(SerializerType.JSON))
        {
        }

        internal Model(HttpWebClient webClient, Serializer serializer)
        {
            _webClient = webClient;
            _serializer = serializer;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Custom HTTP headers
        /// Defaults to empty headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Server end point
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Set the response encoding.
        /// Default  
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Resets the model's state from the server.
        /// Uses the Uri property to get the end point of the server.
        /// </summary>
        /// <param name="success"> Called if the HTTP response is 200 </param>
        /// <param name="error"> Called if the HTTP response is other than 200 </param>
        public virtual void Fetch(Action<Model> success, Action<Exception> error)
        {
            if (Uri == null)
            {
                throw new InvalidOperationException("Uri cannot be null");
            }

            HttpResponseEventHandler handler = null;
            handler =
                (sender, eventArgs) =>
                {
                    _webClient.ResponseAvailable -= handler;

                    if (eventArgs.StatusCode == HttpStatusCode.OK)
                    {
                        ProcessResponse(eventArgs, success, error);
                    }
                    else
                    {
                        error(new NetworkException(eventArgs));
                    }
                };

            _webClient.DoGETRequest(Uri);
        }

        private void ProcessResponse(
            HttpResponseEventArgs eventArgs, Action<Model> success, Action<Exception> error)
        {
            var response = eventArgs.Response;
            var decodedResponse = Encoding.GetString(response, 0, response.Length);

            _data = Parse(decodedResponse);
            
            if (_data != null)
            {
                success(this);
            }
            else
            {
                error(new ResponseParseException());
            }
        }

        /// <summary>
        /// Override to set custom parsing logic.
        /// </summary>
        /// <param name="response"> Decoded server response </param>
        /// <returns> Dictionary of parsed objects </returns>
        protected virtual Dictionary<string, object> Parse(string response)
        {
            return new Dictionary<string, object>();
        }
    }
}