/*
 * JSONException.cs
 * 
 * Author: Kelum Peiris (kelum.peiris@polarmobile.com)
 * Date: 12/7/2011 12:16:23 AM
 *
 */

using System;

namespace CocoB.Rest.WindowsPhone.Core.Serializers.JSON
{
    public class JSONException : Exception
    {
        #region Constructors

        public JSONException(string message)
            : base(message)
        {
        }

        #endregion
    }
}
