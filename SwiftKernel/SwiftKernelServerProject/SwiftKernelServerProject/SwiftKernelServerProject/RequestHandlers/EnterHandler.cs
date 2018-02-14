using SwiftKernelServerProjectCommon;
using SwiftKernelCommon.Core;
using Common;
using System.Collections.Generic;
using UnityEngine;
using SwiftKernelServerProjectCommon.Classes;

namespace SwiftKernelServerProject.RequestHandlers {
    public static class EnterHandler {
        public static void DoHandle(NetData data, Client client, string networkID) {
            string id = (string)data.Values["id"];
            Vector3K pos = (Vector3K)data.Values["position"];

            client.ID = id;
            client.Position = pos;

            NetData allResponse = new NetData(RequestTypes.Enter, new Dictionary<string, object>() { { "id", id }, { "position", pos } });

            EntryPoint.SendResponse(EntryPoint.Clients, Utils.ToBytesJSON(allResponse), networkID);
        }
    }
}
