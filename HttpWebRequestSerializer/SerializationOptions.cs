using System;
using System.Collections.Generic;

namespace HttpWebRequestSerializer
{
    public class SerializationOptions
    {
        public List<string> DoNotSerialize;

        public SerializationOptions(SerializationOptionKey[] serializationOptionKeys = null)
        {
            DoNotSerialize = new List<string>();

            if (serializationOptionKeys == null) return;

            foreach (var key in serializationOptionKeys)
                IgnoreKey(key);
        }

        public void IgnoreKey(SerializationOptionKey serializationOptionKey)
        {
            DoNotSerialize.Add(GetKeyName(serializationOptionKey));
        }

        public static string GetKeyName(SerializationOptionKey serializationOptionKey)
        {
            return Enum.GetName(typeof(SerializationOptionKey), serializationOptionKey);
        }
    }
}
