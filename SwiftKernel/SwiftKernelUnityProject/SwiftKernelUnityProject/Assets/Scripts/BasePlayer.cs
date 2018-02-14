using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BasePlayer : MonoBehaviour {
    public string ID;

    public bool IsInterpolate = false;
    public float PowerInterpolate = 20F;
    private Vector3 newPosition;

    private void Start() {
        newPosition = transform.position;
    }

    private void Update() {
        Interpolating();
    }

    public void ApplyPosition(Vector3 newPosition) {
        this.newPosition = newPosition;

        if(!IsInterpolate) transform.position = newPosition;
    }

    protected virtual void Interpolating () {
        if (IsInterpolate) {
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * PowerInterpolate);
        }
    }
}