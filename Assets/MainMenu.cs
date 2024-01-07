using HurricaneVR.Framework.Core.UI;
using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : NinjaMonoBehaviour {
    public Canvas mainMenuCanvas;
    public GameObject mainMenuLayout;
    private void Start() {
        HVRInputModule.Instance.AddCanvas(mainMenuCanvas);
    }
    public void OnStartGameButtonClick() {
        var logId = "OnStartGameButtonClick";
        GameManager.Instance.StartGame();
        mainMenuLayout.SetActive(false);
        HVRInputModule.Instance.RemoveCanvas(mainMenuCanvas);
    }
    public void OnExitButtonClick() {
        var logId = "OnExitButtonClick";
    }
}
