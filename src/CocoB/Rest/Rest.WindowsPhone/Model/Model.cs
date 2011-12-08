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
using CocoB.Rest.WindowsPhone.Core.Serializers;
using CocoB.Rest.WindowsPhone.Core.Serializers.JSON;
using CocoB.Rest.WindowsPhone.Network;

namespace CocoB.Rest.WindowsPhone.Model
{
    public class Model
    {
        private readonly HttpWebClient _webClient;
        private readonly Serializer _serializer;

        public Model()
        {
            _webClient = CreateWebClient();
            _serializer = CreateSerializer();
        }

        private HttpWebClient CreateWebClient()
        {
            return InitializeWebClient();
        }

        internal virtual HttpWebClient InitializeWebClient()
        {
           return HttpWebClient.Create(Headers); 
        }

        private Serializer CreateSerializer()
        {
            return InitializeSerializer();
        }

        internal virtual Serializer InitializeSerializer()
        {
            return new JSONSerializer();
        }

        /// <summary>
        /// Override to set custom HTTP headers
        /// Defaults to empty headers.
        /// </summary>
        protected virtual Dictionary<string, string> Headers
        {
            get { return new Dictionary<string, string>(); }
        }

        /// <summary>
        /// Resets the model's state from the server.
        /// Uses the Uri property to get the end point of the server.
        /// </summary>
        /// <param name="success"> Called if the HTTP response is 200 </param>
        /// <param name="error"> Called if the HTTP response is other than 200 </param>
        public virtual void Fetch(Action<Model> success, Action<HttpStatusCode> error)
        {
            // TODO: Process non OK responses. Might have to include headers and response to error       
            HttpResponseEventHandler handler = null;
            handler =
                (sender, eventArgs) =>
                {
                    _webClient.ResponseAvailable -= handler;

                    if (eventArgs.StatusCode == HttpStatusCode.OK)
                    {
                        var response = eventArgs.Response;
                        var decodedResponse = Encoding.GetString(response, 0, response.Length);
                        Parse(decodedResponse); // TODO: call error on parse failure
                        success(this);
                    }
                    else
                    {
                        error(eventArgs.StatusCode);
                    }
                };
            _webClient.DoGETRequest(Uri);
        }

        /// <summary>
        /// Override to set the end point address of the server.
        /// Defaults to http://localhost
        /// </summary>
        protected virtual Uri Uri
        {
            get { return new Uri("http://localhost"); }
        }

        /// <summary>
        /// Override to set the encoding that will be used to decode the server response
        /// Defaults to UTF8
        /// </summary>
        protected virtual Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// Override to set custom parsing logic.
        /// </summary>
        /// <param name="response"> Decoded server response </param>
        /// <returns> Dictionary of parsed objects </returns>
        protected virtual Dictionary<string, string> Parse(string response)
        {
            return new Dictionary<string, string>();
        }
    }
}