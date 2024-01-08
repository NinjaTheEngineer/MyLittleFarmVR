using HurricaneVR.Framework.Core.UI;
using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : WorldSpaceCanvas {
    public GameObject mainMenuLayout;
    public void OnStartGameButtonClick() {
        var logId = "OnStartGameButtonClick";
        GameManager.Instance.StartGame();
        mainMenuLayout.SetActive(false);
        HVRInputModule.Instance.RemoveCanvas(canvas);
    }
    public void OnExitButtonClick() {
        var logId = "OnExitButtonClick";
    }
}
