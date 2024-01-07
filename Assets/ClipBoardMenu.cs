using HurricaneVR.Framework.Core.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipBoardMenu : WorldSpaceCanvas
{
    public void OnGrab() {
        HVRInputModule.Instance.AddCanvas(canvas);
    }
    public void OnRelease() {
        HVRInputModule.Instance.RemoveCanvas(canvas);
    }
}
