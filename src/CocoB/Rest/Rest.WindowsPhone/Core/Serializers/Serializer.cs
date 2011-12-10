/*
 * Serializer.cs
 * 
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/8/2011 12:12:09 AM
 *
 */

using System;
using System.Collections.Generic;
using CocoB.Rest.WindowsPhone.Core.Serializers.JSON;

namespace CocoB.Rest.WindowsPhone.Core.Serializers
{
    internal enum SerializerType
    {
        JSON
    }

    internal abstract class Serializer
    {
        #region Methods

        public abstract Dictionary<string, object> DeserializeModel(string data);

        public abstract List<object> DeserializeCollection(string data);

        public abstract string Serialize(Dictionary<string, object> model);

        public abstract string Serialize(List<object> collection);

        public static Serializer Create(SerializerType serializerType)
        {
            switch (serializerType)
            {
                case SerializerType.JSON:
                    return new JSONSerializer();
                default:
                    throw new InvalidOperationException(
                        "Invalid serialization type - " + serializerType);
            }
        }

        #endregion
    }
}