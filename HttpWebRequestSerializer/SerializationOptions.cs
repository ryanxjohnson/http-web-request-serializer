using System.Collections.Generic;

namespace HttpWebRequestSerializer
{
    public class SerializationOptions
    {
        public List<string> DoNotSerialize;

        public SerializationOptions()
        {
            DoNotSerialize = new List<string>();    
        }

        public void IgnoreKey(string key)
        {
            DoNotSerialize.Add(key);
        }
    }
}
