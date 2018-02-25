using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace SwiftKernelUnity.Core {
    public class UnityEventReceiver {
        public event Action<byte, byte[]> OnHandleEvent = delegate { };
        public event Action<byte, byte[]> OnHandleResponse = delegate { };

        private Dictionary<string, Action<byte[]>> eventCallbacks =
            new Dictionary<string, Action<byte[]>>();
        private Dictionary<string, Action<byte[]>> responseCallbacks =
            new Dictionary<string, Action<byte[]>>();
        private Dictionary<string, bool> onceCallbackDictionary =
            new Dictionary<string, bool>();

        public void DoHandleEvent(byte[] data, string networkID) {
            if(string.IsNullOrEmpty(networkID)) Debug.Log("unknow networkID - " + networkID);

            if(eventCallbacks.ContainsKey(networkID)) {
                eventCallbacks[networkID].Invoke(data);

                if(onceCallbackDictionary.ContainsKey(networkID)) {
                    if(onceCallbackDictionary[networkID]) {
                        eventCallbacks.Remove(networkID);
                        onceCallbackDictionary.Remove(networkID);
                    }
                } else {
                    eventCallbacks.Remove(networkID);
                    onceCallbackDictionary.Remove(networkID);
                }
            } else {
                Debug.Log("unknow networkID - " + networkID);
            }
        }

        public void DoHandleResponse(byte[] data, string networkID) {
            if(string.IsNullOrEmpty(networkID)) Debug.Log("unknow networkID - " + networkID);

            if(responseCallbacks.ContainsKey(networkID)) {
                responseCallbacks[networkID].Invoke(data);

                if(onceCallbackDictionary.ContainsKey(networkID)) {
                    if(onceCallbackDictionary[networkID]) {
                        responseCallbacks.Remove(networkID);
                        onceCallbackDictionary.Remove(networkID);
                    }
                } else {
                    responseCallbacks.Remove(networkID);
                    onceCallbackDictionary.Remove(networkID);
                }
            } else {
                Debug.Log("unknow networkID - " + networkID);
            }
        }

        public string AddEventObserver(Action<byte[]> callback, bool onceCallback) {
            var mth = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            var cls = mth.ReflectedType.Name;

            string guid = "Event" + cls + callback.Method.Name;

            eventCallbacks.Add(guid, callback);
            onceCallbackDictionary.Add(guid, onceCallback);

            return guid;
        }

        public string AddResponseObserver(Action<byte[]> callback, bool onceCallback) {
            var mth = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            var cls = mth.ReflectedType.Name;

            string guid = "Response" + cls + callback.Method.Name;

            responseCallbacks.Add(guid, callback);
            onceCallbackDictionary.Add(guid, onceCallback);

            return guid;
        }

        public void RemoveEventObserver(string guid) {
            if(eventCallbacks.ContainsKey(guid)) eventCallbacks.Remove(guid);
            if (onceCallbackDictionary.ContainsKey(guid)) onceCallbackDictionary.Remove(guid);
        }

        public void RemoveResponseObserver(string guid) {
            if(responseCallbacks.ContainsKey(guid)) responseCallbacks.Remove(guid);
            if (onceCallbackDictionary.ContainsKey(guid)) onceCallbackDictionary.Remove(guid);
        }
    }
}
