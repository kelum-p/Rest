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
using CocoB.Rest.WindowsPhone.Network;

namespace CocoB.Rest.WindowsPhone.Model
{
    public class Model
    {
        private readonly HttpWebClient _webClient;

        private Model(HttpWebClient webClient)
        {
            _webClient = webClient;
        }

        /// <summary>
        /// Creates a new Model.
        /// Uses Headers property to get HTTP headers.
        /// </summary>
        /// <returns></returns>
        public Model Create()
        {
            var headers = Headers;
            var webClient = HttpWebClient.Create(headers);
            return new Model(webClient);
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
                        Parse(decodedResponse);
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
        /// </summary>
        protected virtual Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// Override to set custom parsing logic.
        /// </summary>
        /// <param name="response"> Decoded server response </param>
        protected virtual void Parse(string response)
        {
            
        }
    }
}