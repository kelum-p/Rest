/*
 * JSONObject.cs
 *
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/6/2011 11:57:04 PM
 *
 */

namespace CocoB.Rest.WindowsPhone.Core.Serializers.Json
{
    public abstract class JSONObject
    {
        #region Methods

        public abstract T Get<T>(string key);

        #endregion
    }
}
