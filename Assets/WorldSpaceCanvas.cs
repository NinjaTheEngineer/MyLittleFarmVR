using HurricaneVR.Framework.Core.UI;
using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvas : NinjaMonoBehaviour
{
    public Canvas canvas;
    private void Awake() {
        canvas = canvas ?? GetComponent<Canvas>();
    }
    void Start() {
        HVRInputModule.Instance.AddCanvas(canvas);
    }
}
