using Common;
using SwiftKernelServerProjectCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventID : MonoBehaviour {
    public void DoTest() {
        EntryPoint.Client.SendRequest(Utils.ToBytesJSON(new NetData(RequestTypes.TestEventID, new Dictionary<string, object>())), EntryPoint.Client.UnityEventReceiver.AddResponseObserver(TestAction, true));
    }

    private void TestAction(byte[] data) {
        Debug.Log("EVent id works");
    }
}
