using Common;
using SwiftKernel.Core;
using SwiftKernelCommon.Core;
using SwiftKernelServerProject.Events;
using SwiftKernelServerProject.RequestHandlers;
using SwiftKernelServerProjectCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftKernelServerProject {
    public class EntryPoint {
        public static List<Client> Clients = new List<Client>();
        public static SwiftKernelServer Server = null;

        private static void Main(string[] args) {
            Server = new SwiftKernelServer();
            Server.Setup(6000, "swift");

            Server.OnPeerConnected += Server_OnPeerConnected;
            Server.OnPeerDisconnected += Server_OnPeerDisconnected;
            Server.OnRequestReceived += Server_OnRequestReceived;

            Server.Start();
        }

        private static void Server_OnPeerConnected(NetPeer peer) {
            Console.WriteLine("Peer connected with ip {0}", peer.EndPoint.Host);

            Clients.Add(new Client(peer));
        }

        private static void Server_OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            Console.WriteLine("Peer disconnected with ip {0}", peer.EndPoint.Host);
            Client client = Clients.Find(x => x.Peer == peer);
            if(client != null) {
                ExitEvent.DoEvent(client);
                Clients.Remove(client);
            }
        }

        private static void Server_OnRequestReceived(NetPeer peer, byte[] data, string networkID) {
            Client client = Clients.Find(x => x.Peer == peer);

            if(client == null) return;

            NetData ndata = Utils.FromBytesJSON<NetData>(data);

            switch(ndata.Type) {
                case RequestTypes.Enter: EnterHandler.DoHandle(ndata, client, networkID); break;
                case RequestTypes.LoadPlayer: LoadPlayersHandler.DoHandle(ndata, client, networkID); break;
                case RequestTypes.Move: MoveHandler.DoHandle(ndata, client, networkID); break;
                case RequestTypes.TestEventID: TestEventIDHandler.DoHandle(ndata, client, networkID); break;
            }
        }

        #region Public API
         
        public static void SendResponse(List<Client> clients, byte[] data, string networkID = "") {
            foreach(Client c in clients) {
                Server.SendResponse(c.Peer, data, networkID);
            }
        }

        public static void SendEvent(List<Client> clients, byte[] data, string networkID = "") {
            foreach(Client c in clients) {
                Server.SendEvent(c.Peer, data, networkID);
            }
        }

        #endregion
    }
}
