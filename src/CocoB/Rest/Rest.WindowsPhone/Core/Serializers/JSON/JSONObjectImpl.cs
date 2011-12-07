/*
 * JSONObjectImpl.cs
 *
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/7/2011 12:14:34 AM
 *
 */

using System;
using System.Collections.Generic;
using CocoB.Rest.WindowsPhone.Core.Parsers;
using CocoB.Rest.WindowsPhone.Core.Serializers.Json;

namespace CocoB.Rest.WindowsPhone.Core.Serializers.JSON
{
    public class JSONObjectImpl : JSONObject
    {
        #region Member Variables

        private readonly Dictionary<string, object> _data;

        #endregion

        #region Constructors

        public JSONObjectImpl(string json)
        {
            _data = JSONSerializer.JsonDecode(json) as Dictionary<string, object>;

            if (_data == null)
            {
                throw new JSONException("Failed to parse the JSON string to an JSON object.");
            }
        }

        #endregion

        #region Overrides of JSONObject

        public override T Get<T>(string key)
        {
            object value;
            if (!_data.TryGetValue(key, out value))
            {
                throw new JSONException("Key does not exist");
            }

            try
            {
                return (T) value;
            }
            catch (Exception exception)
            {
                throw new JSONException(exception.Message);
            }
        }

        public override string ToString()
        {
            var serializedObject = JSONSerializer.JsonEncode(_data);
            if (serializedObject == null)
            {
                throw new JSONException("Failed to serialize this object");
            }

            return serializedObject;
        }

        #endregion
    }
}