using Common;
using Common.Core.Data;
using SwiftKernelCommon.Core;
using SwiftKernelCommon.Core.Utils;
using System;
using System.Threading;
using UnityEngine;

namespace SwiftKernelUnity.Core {
    public abstract class UnityPeer {
        private string ip;
        private int port;
        private string connectionKey;
        private NetManager client;
        private NetPeer peer;
        private Thread threadEventReceiver;
        private bool isRunning = false;
        private UnityEventReceiver unityEventReceiver;

        public string Ip {
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

        public bool IsConnected {
            get {
                return peer != null;
            }
        }

        public UnityEventReceiver UnityEventReceiver {
            get {
                if(unityEventReceiver == null) unityEventReceiver = new UnityEventReceiver();
                return unityEventReceiver;
            }
        }

        public virtual void Setup(string ip, int port, string connectionKey) {
            this.ip = ip;
            this.port = port;
            this.connectionKey = connectionKey;

            UnityThreadHelper.Dispatcher.GetType();
        }

        public void Connect() {
            isRunning = true;

            EventBasedNetListener listener = new EventBasedNetListener();
            client = new NetManager(listener, connectionKey);
            client.Start();
            client.Connect(ip, port);

            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;

            threadEventReceiver = new Thread(EventReceiver);
            threadEventReceiver.Start();
        }

        public void Disconnect() {
            isRunning = false;

            if(client != null) {
                client.Stop();
                client = null;
            }

            if(threadEventReceiver != null) {
                threadEventReceiver.Join();
                threadEventReceiver = null;
            }
        }

        public void SendResponse(byte[] data, string networkID = "") {
            if(peer.ConnectionState == ConnectionState.Connected) {
                Send(BaseData.Types.ResponseData, data, networkID);
            }
        }

        public void SendRequest(byte[] data, string networkID = "") {
            if(peer.ConnectionState == ConnectionState.Connected) {
                Send(BaseData.Types.RequestData, data, networkID);
            }
        }

        private void Send(BaseData.Types type, byte[] data, string networkID) {
            BaseData d = new BaseData(type, data, networkID);
            peer.Send(Utils.ToBytesJSON(d), SendOptions.ReliableOrdered);
        }

        private void EventReceiver() {
            while(isRunning) {
                client.PollEvents();
                Thread.Sleep(15);
            }
        }

        private void Listener_PeerConnectedEvent(NetPeer peer) {
            this.peer = peer;

            UnityThreadHelper.Dispatcher.Dispatch(() => {
                OnConnect();
            });
        }

        private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo) {
            this.peer = null;

            UnityThreadHelper.Dispatcher.Dispatch(() => {
                OnDisconnect();
            });
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetDataReader reader) {
            byte[] bytes = new byte[reader.Data.Length];
            Array.Copy(reader.Data, bytes, bytes.Length);

            UnityThreadHelper.Dispatcher.Dispatch(() => {
                BaseData data = Utils.FromBytesJSON<BaseData>(bytes);

                GC.KeepAlive(bytes);

                switch(data.Type) {
                    case BaseData.Types.ResponseData:
                        if(string.IsNullOrEmpty(data.NetworkID)) OnResponseReceived(data.Data);
                        else UnityEventReceiver.DoHandleResponse(data.Data, data.NetworkID);
                        break;
                    case BaseData.Types.EventData:
                        if(string.IsNullOrEmpty(data.NetworkID)) OnEventReceived(data.Data);
                        else UnityEventReceiver.DoHandleEvent(data.Data, data.NetworkID);
                        break;
                }
            });
        }

        protected abstract void OnConnect();
        protected abstract void OnDisconnect();
        protected abstract void OnResponseReceived(byte[] data);
        protected abstract void OnEventReceived(byte[] data);
    }
}
