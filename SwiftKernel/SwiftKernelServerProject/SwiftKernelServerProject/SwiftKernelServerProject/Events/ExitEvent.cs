using SwiftKernelServerProjectCommon;
using Common;
using System.Collections.Generic;

namespace SwiftKernelServerProject.Events {
    public static class ExitEvent {
        public static void DoEvent(Client client) {
            string id = client.ID;

            NetData allResponse = new NetData(RequestTypes.Exit, new Dictionary<string, object>() { { "id", id } });

            EntryPoint.SendEvent(EntryPoint.Clients, Utils.ToBytesJSON(allResponse));
        }
    }
}
