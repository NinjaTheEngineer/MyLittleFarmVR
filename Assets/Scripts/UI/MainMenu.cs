using HurricaneVR.Framework.Core.UI;
using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : WorldSpaceCanvas {
    public GameObject mainMenuLayout;
    public void OnStartGameButtonClick() {
        var logId = "OnStartGameButtonClick";
        AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
        GameManager.Instance.StartGame();
        mainMenuLayout.SetActive(false);
        HVRInputModule.Instance.RemoveCanvas(canvas);
    }
    public void OnExitButtonClick() {
        AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
        var logId = "OnExitButtonClick";
        Application.Quit();
    }
}
