using System;
using System.Collections.Generic;

namespace HttpWebRequestSerializer
{
    public class SerializationOptions
    {
        private string[] validKeys = new [] { "Uri", "Headers", "Cookie", "Data" };

        public List<string> DoNotSerialize;

        public SerializationOptions()
        {
            DoNotSerialize = new List<string>();    
        }

        public void IgnoreKey(string key)
        {
            if (Array.Exists(validKeys, k => k == key))
                DoNotSerialize.Add(key);
        }
    }
}
