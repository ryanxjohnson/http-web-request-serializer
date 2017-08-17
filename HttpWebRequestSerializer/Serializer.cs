using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace HttpWebRequestSerializer
{
    public static class Serializer
    {
        private static JavaScriptSerializer serializer => new JavaScriptSerializer();

        public static string SerializeRequestProperties(this object properties)
        {
            var request = properties.MakeDictionary();
            return serializer.Serialize(request);
        }

        public static IDictionary<string, object> DeserializeRequestProperties(this string json)
        {
            return serializer.Deserialize<IDictionary<string, object>>(json);
        }

        public static IDictionary<string, object> MakeDictionary(this object properties)
        {
            var dict = properties as IDictionary<string, object>;
            return dict?.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}