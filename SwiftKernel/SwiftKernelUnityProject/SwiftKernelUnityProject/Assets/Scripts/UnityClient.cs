using System;
using SwiftKernelUnity.Core;
using UnityEngine;
using System.Text;
using SwiftKernelServerProjectCommon;
using Common;
using System.Collections.Generic;
using SwiftKernelServerProjectCommon.Classes;
using SwiftKernelServerProjectCommon.Extensions;

public class UnityClient : UnityPeer {
    public event Action OnConnectEvent = delegate { };

    public EntryPoint EntryPoint;

    protected override void OnConnect() {
        Debug.Log("Connected");

        OnConnectEvent.Invoke();
    }

    protected override void OnDisconnect() {
        Debug.Log("Disconnected");
    }

    protected override void OnEventReceived(byte[] data) {
        NetData ndata = Utils.FromBytesJSON<NetData>(data);

        switch(ndata.Type) {
            case RequestTypes.Exit:
                BasePlayer pl = EntryPoint.Players.Find(x => x.ID.Equals((string)ndata.Values["id"]));
                if(pl != null) {
                    GameObject.Destroy(pl.gameObject);
                    EntryPoint.Players.Remove(pl);
                }
                break;
        }
    }

    protected override void OnResponseReceived(byte[] data) {
        NetData ndata = Utils.FromBytesJSON<NetData>(data);

        switch(ndata.Type) {
            case RequestTypes.Enter:
                EntryPoint.SpawnPlayer((string)ndata.Values["id"], (Vector3K)ndata.Values["position"]);

                if(((string)ndata.Values["id"]).Equals(EntryPoint.ID)) {
                    EntryPoint.Client.SendRequest(Utils.ToBytesJSON(new NetData(RequestTypes.LoadPlayer, new Dictionary<string, object>())));
                }
                break;
            case RequestTypes.LoadPlayer:
                List<Client> clients = (List<Client>)ndata.Values["clients"];
                foreach(Client c in clients) {
                    EntryPoint.SpawnPlayer(c.ID, c.Position);
                }
                break;
            case RequestTypes.Move:
                BasePlayer pl = EntryPoint.Players.Find(x => x.ID.Equals((string)ndata.Values["id"]));
                if(pl != null) {
                    Vector3K pos = (Vector3K)ndata.Values["position"];
                    pl.ApplyPosition(pos.FromServerVector3());
                }
                break;
        }
    }
}
