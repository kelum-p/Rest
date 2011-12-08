/*
 * JSONSerializer.cs
 * 
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/8/2011 12:27:36 AM
 *
 */

using System.Collections.Generic;

namespace CocoB.Rest.WindowsPhone.Core.Serializers.JSON
{
    internal class JSONSerializer : Serializer
    {
        #region Overrides of Serializer

        public override Dictionary<string, object> DeserializeModel(string data)
        {
            var result = JSONParser.JsonDecode(data) as Dictionary<string, object>;
            if (result == null)
            {
                throw new JSONException("Failed to deserialize the model data - " + data);
            }
            return result;
        }

        public override List<object> DeserializeCollection(string data)
        {
            var result = JSONParser.JsonDecode(data) as List<object>;
            if (result == null)
            {
                throw new JSONException("Failed to deserialize the collection data - " + data);
            }
            return result;
        }

        public override string Serialize(Dictionary<string, object> model)
        {
            var result = JSONParser.JsonEncode(model);
            if (result == null)
            {
                throw new JSONException("Failed to serialize the model data");
            }
            return result;
        }

        public override string Serialize(List<object> collection)
        {
            var result = JSONParser.JsonEncode(collection);
            if (result == null)
            {
                throw new JSONException("Failed to serialize the collection data");
            }
            return result;
        }

        #endregion
    }
}