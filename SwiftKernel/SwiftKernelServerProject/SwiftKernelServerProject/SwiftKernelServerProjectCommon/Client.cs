using Newtonsoft.Json;
using SwiftKernelCommon.Core;
using SwiftKernelServerProjectCommon.Classes;
using UnityEngine;

namespace SwiftKernelServerProjectCommon {
    public class Client {
        [JsonIgnore]
        public NetPeer Peer;
        public Vector3K Position;
        public string ID;

        [JsonConstructor]
        public Client(NetPeer peer, Vector3K position, string id) {
            Peer = peer;
            Position = position;
            ID = id;
        }

        public Client(NetPeer peer) {
            Peer = peer;
        }
    }
}
