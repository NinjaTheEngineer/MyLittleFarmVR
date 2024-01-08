using HurricaneVR.Framework.Core.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipBoardMenu : WorldSpaceCanvas {
    public GameObject exitConfirmationMenu;
    public GameObject optionsMenu;
    GameObject _optionsMenu;
    private void Start() {
        exitConfirmationMenu.SetActive(false);
    }

    public void OnTogglePlaceBlock() {
        BlockPlacer.Instance.TogglePlacement();
    }

    public void OnSocketed() {
        BlockPlacer.Instance.StopPlacement();
    }

    public void OnGrab() {
        HVRInputModule.Instance.AddCanvas(canvas);
    }
    public void OnRelease() {
        HVRInputModule.Instance.RemoveCanvas(canvas);
    }

    public void OnOptionsButtonClick() {
        if (_optionsMenu == null) {
            _optionsMenu = Instantiate(optionsMenu);
        }
    }

    public void OnExitButtonClick() {
        if (exitConfirmationMenu.activeSelf) {
            return;
        }
        exitConfirmationMenu.SetActive(true);
    }

    public void OnConfirmExitClick() {
        //save game first
        Application.Quit();
    }
    public void OnDeclineExitClick() {
        exitConfirmationMenu.SetActive(false);
    }
}
