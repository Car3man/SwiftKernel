using SwiftKernelServerProjectCommon;
using SwiftKernelCommon.Core;
using Common;
using System.Collections.Generic;
using UnityEngine;
using SwiftKernelServerProjectCommon.Classes;

namespace SwiftKernelServerProject.RequestHandlers {
    public static class MoveHandler {
        public static void DoHandle(NetData data, Client client, string networkID) {
            string id = (string)data.Values["id"];
            Vector3K pos = (Vector3K)data.Values["position"];

            client.Position = pos;

            NetData allResponse = new NetData(RequestTypes.Move, new Dictionary<string, object>() { { "id", id }, { "position", pos } });

            List<Client> clients = new List<Client>(EntryPoint.Clients);
            clients.Remove(client);

            EntryPoint.SendResponse(clients, Utils.ToBytesJSON(allResponse), networkID);
        }
    }
}
