﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace HttpWebRequestSerializer
{
    public static class Serializer
    {
        private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static string SerializeRequestProperties(this IDictionary<string, object> properties, SerializationOptions so = null)
        {
            var request = properties.MakeDictionary();

            if (so?.DoNotSerialize == null)
                return serializer.Serialize(request);

            foreach (var s in so.DoNotSerialize)
                request.Remove(s);

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