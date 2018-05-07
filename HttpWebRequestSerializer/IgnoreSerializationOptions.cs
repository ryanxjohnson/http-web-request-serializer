using System;
using System.Collections.Generic;

namespace HttpWebRequestSerializer
{
    public class IgnoreSerializationOptions
    {
        public List<string> DoNotSerialize;

        public IgnoreSerializationOptions(IgnoreSerializationOptionKey[] ignoreSerializationOptionKeys = null)
        {
            DoNotSerialize = new List<string>();

            if (ignoreSerializationOptionKeys == null) return;

            foreach (var key in ignoreSerializationOptionKeys)
                IgnoreKey(key);
        }

        public void IgnoreKey(IgnoreSerializationOptionKey ignoreSerializationOptionKey)
        {
            DoNotSerialize.Add(GetKeyName(ignoreSerializationOptionKey));
        }

        public static string GetKeyName(IgnoreSerializationOptionKey ignoreSerializationOptionKey)
        {
            return Enum.GetName(typeof(IgnoreSerializationOptionKey), ignoreSerializationOptionKey);
        }
    }
}
