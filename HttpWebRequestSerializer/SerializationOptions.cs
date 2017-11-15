using System.Collections.Generic;

namespace HttpWebRequestSerializer
{
    public class SerializationOptions
    {
        public List<string> doNotSerialize;
        //public Dictionary<string, object> doSerialize;

        public SerializationOptions()
        {
            doNotSerialize = new List<string>();    
            //doSerialize = new Dictionary<string, object>();
        }

        public void IgnoreKey(string key)
        {
            doNotSerialize.Add(key);
        }

        //public void AddKey(string key, string value)
        //{
        //    doSerialize[key] = value;
        //}
    }
}
