using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftKernelServerProjectCommon {
    public class NetData {
        public RequestTypes Type;
        public Dictionary<string, object> Values = new Dictionary<string, object>();

        public NetData (RequestTypes type, Dictionary<string, object> values) {
            Type = type;
            Values = values;
        }
    }
}
