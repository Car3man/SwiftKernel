using Common;
using SwiftKernelServerProjectCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftKernelServerProject.RequestHandlers {
    public class TestEventIDHandler {
        public static void DoHandle(NetData data, Client client, string networkID) {
            NetData allResponse = new NetData(RequestTypes.Move, new Dictionary<string, object>());
            EntryPoint.SendResponse(EntryPoint.Clients, Utils.ToBytesJSON(allResponse), networkID);
        }
    }
}
