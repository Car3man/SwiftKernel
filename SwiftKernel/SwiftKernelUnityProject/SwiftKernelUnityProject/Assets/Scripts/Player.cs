using Common;
using SwiftKernelServerProjectCommon;
using SwiftKernelServerProjectCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : BasePlayer {
    [SerializeField]
    private float moveSpeed = 0.1f;

    private void Update() {
        if(Input.GetKey(KeyCode.W)) {
            transform.position += Vector3.up * moveSpeed;
        }
        if(Input.GetKey(KeyCode.A)) {
            transform.position += Vector3.left * moveSpeed;
        }
        if(Input.GetKey(KeyCode.S)) {
            transform.position += Vector3.down * moveSpeed;
        }
        if(Input.GetKey(KeyCode.D)) {
            transform.position += Vector3.right * moveSpeed;
        }

        UpdatePosition();
    }

    private void UpdatePosition() {
        EntryPoint.Client.SendRequest(
            Utils.ToBytesJSON(new NetData(RequestTypes.Move, new Dictionary<string, object>() { { "id", ID }, { "position", (transform.position).ToServerVector3() } }))
        );
    }
}