using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace HttpWebRequestSerializer
{
    public static class ObjectDisassembler
    {
        public static IDictionary<string, object> DisassembleWebRequest(this HttpWebRequest req)
        {
            var properties = GetProperties(req);

            return properties.ToDictionary(p => p.Name, p => p.GetValue(req, null));
        }

        public static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }
    }
}

