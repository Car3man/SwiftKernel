using SwiftKernelServerProjectCommon;
using SwiftKernelCommon.Core;
using Common;
using System.Collections.Generic;

namespace SwiftKernelServerProject.RequestHandlers {
    public static class LoadPlayersHandler {
        public static void DoHandle(NetData data, Client client, string networkID) {
            List<Client> clients = new List<Client>(EntryPoint.Clients);
            clients.Remove(client);

            NetData response = new NetData(RequestTypes.LoadPlayer, new Dictionary<string, object>() { { "clients", clients } });

            EntryPoint.SendResponse(new List<Client>() { client }, Utils.ToBytesJSON(response), networkID);
        }
    }
}
