using Common;
using SwiftKernelServerProjectCommon;
using SwiftKernelServerProjectCommon.Classes;
using SwiftKernelServerProjectCommon.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EntryPoint : MonoBehaviour {
    public static UnityClient Client;
    public static string ID = string.Empty;

    public List<BasePlayer> Players = new List<BasePlayer>();
    public Vector3 SpawnPoint = Vector3.zero;

    [SerializeField]
    private GameObject ownPlayerPrefab;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private TestEventID testEventID;

    private void Start() {
        ID = Guid.NewGuid().ToString();

        Client = new UnityClient();
        Client.EntryPoint = this;

        Client.Setup("localhost", 6000, "swift");
        Client.OnConnectEvent += Client_OnConnectEvent;
        Client.Connect();
    }

    private void Client_OnConnectEvent() {
        Client.SendRequest(Utils.ToBytesJSON(new NetData(RequestTypes.Enter, new Dictionary<string, object>() { { "id", ID }, { "position", SpawnPoint.ToServerVector3() } })));

        //Test Event ID System
        //testEventID.DoTest();
    }

    private void OnDestroy() {
        Client.Disconnect();
    }

    public void SpawnPlayer(string id, Vector3K pos) {
        GameObject pl = Instantiate(id.Equals(ID) ? ownPlayerPrefab : playerPrefab);
        
        pl.transform.position = pos.FromServerVector3();

        pl.GetComponent<BasePlayer>().ID = id;

        Players.Add(pl.GetComponent<BasePlayer>());
    }
}
