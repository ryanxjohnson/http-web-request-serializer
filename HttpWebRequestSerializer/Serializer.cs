﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace HttpWebRequestSerializer
{
    public static class Serializer
    {
        // ReSharper disable once InconsistentNaming
        private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static string SerializeRequestProperties(this IDictionary<string, object> properties, IgnoreSerializationOptions so = null)
        {
            var request = properties.MakeDictionary();

            if (so?.DoNotSerialize == null)
                return serializer.Serialize(request);

            foreach (var s in so.DoNotSerialize)
                request.Remove(s);

            // check if any values are null
            // What's likely happening is that request is indirectly changing the serialized dictionary under the hood during the loop.
            // So convert to list fixes that
            foreach (var o in request.ToList())
                if (o.Value == null) request[o.Key] = "";

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