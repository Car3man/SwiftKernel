using Common;
using Common.Core.Data;
using SwiftKernelCommon.Core;
using SwiftKernelCommon.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;

namespace SwiftKernel.Core {
    public class SwiftKernelServer {
        private EventBasedNetListener listener;
        private NetManager server;
        private Thread threadEventReceiver;
        private bool isRunning = false;

        private IPAddress ip;
        private int port;
        private string connectionKey;

        public IPAddress Ip {
            get {
                return ip;
            }
        }

        public int Port {
            get {
                return port;
            }
        }

        public string ConnectionKey {
            get {
                return connectionKey;
            }
        }

        public event Action<NetPeer> OnPeerConnected = delegate { };
        public event Action<NetPeer, DisconnectInfo> OnPeerDisconnected = delegate { };
        public event Action<NetPeer, byte[], string> OnRequestReceived = delegate { };
        public event Action<NetPeer, byte[], string> OnResponseReceived = delegate { };

        #region Public API

        public virtual void Setup(IPAddress ip, int port, string connectionKey) {
            this.ip = ip;
            this.port = port;
            this.connectionKey = connectionKey;
        }

        public void Start() {
            isRunning = true;

            listener = new EventBasedNetListener();
            server = new NetManager(listener, 1000, connectionKey);

            server.Start(ip, port);

            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;

            threadEventReceiver = new Thread(EventReceiver);
            threadEventReceiver.Start();

            Console.WriteLine("Server running on port: {0}, and ip: {1}", server.LocalPort, server.LocalIp);
        }

        public void Stop() {
            isRunning = false;

            if(server != null) {
                server.Stop();
                server = null;
            }

            if(threadEventReceiver != null) {
                threadEventReceiver.Join();
                threadEventReceiver = null;
            }
        }

        public void SendResponse(List<NetPeer> peers, byte[] data, string networkID = "") {
            foreach(NetPeer peer in peers) {
                if(peer.ConnectionState == ConnectionState.Connected) {
                    Send(peer, BaseData.Types.ResponseData, data, networkID);
                }
            }
        }

        public void SendResponse(NetPeer peer, byte[] data, string networkID = "") {
            if(peer.ConnectionState == ConnectionState.Connected) {
                Send(peer, BaseData.Types.ResponseData, data, networkID);
            }
        }

        public void SendEvent(List<NetPeer> peers, byte[] data, string networkID = "") {
            foreach(NetPeer peer in peers) {
                if(peer.ConnectionState == ConnectionState.Connected) {
                    Send(peer, BaseData.Types.EventData, data, networkID);
                }
            }
        }

        public void SendEvent(NetPeer peer, byte[] data, string networkID = "") {
            if(peer.ConnectionState == ConnectionState.Connected) {
                Send(peer, BaseData.Types.EventData, data, networkID);
            }
        }

        #endregion

        private void Send(NetPeer peer, BaseData.Types type, byte[] data, string networkID) {
            BaseData d = new BaseData(type, data, networkID);
            peer.Send(Utils.ToBytesJSON(d), SendOptions.ReliableOrdered);
        }

        private void EventReceiver() {
            while(isRunning) {
                server.PollEvents();
                Thread.Sleep(15);
            }
        }

        #region Events 

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetDataReader reader) {
            BaseData data = Utils.FromBytesJSON<BaseData>(reader.Data);

            switch(data.Type) {
                case BaseData.Types.RequestData:
                    OnRequestReceived.Invoke(peer, data.Data, data.NetworkID);
                    break;
                case BaseData.Types.ResponseData:
                    OnResponseReceived.Invoke(peer, data.Data, data.NetworkID);
                    break;
            }
        }

        private void Listener_PeerConnectedEvent(NetPeer peer) {
            OnPeerConnected.Invoke(peer);
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo) {
            OnPeerDisconnected.Invoke(peer, disconnectInfo);
        }

        #endregion
    }
}
