using HurricaneVR.Framework.Core.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class ClipBoardMenu : WorldSpaceCanvas {
    public GameObject exitConfirmationMenu;
    public GameObject optionsMenu;
    public TextMeshProUGUI goldAmountText;
    GameObject _optionsMenu;
    protected override void Awake() {
        base.Awake();
        exitConfirmationMenu.SetActive(false);
        GoldSystem.OnGoldChanged += OnGoldChanged;
    }

    void OnGoldChanged(int goldAmount) {
        goldAmountText.text = goldAmount.ToString();
    }

    public void OnTogglePlaceBlock() {
        if (exitConfirmationMenu.activeSelf) {
            return;
        }
        BlockPlacer.Instance.TogglePlacement();
        AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
    }

    public void OnSocketed() {
        BlockPlacer.Instance.StopPlacement();
        AudioManager.Instance.PlaySFX(SoundType.StoreItem, transform.position);
    }

    public void OnGrab() {
        HVRInputModule.Instance.AddCanvas(canvas);
    }
    public void OnRelease() {
        HVRInputModule.Instance.RemoveCanvas(canvas);
    }

    public void OnOptionsButtonClick() {
        if (exitConfirmationMenu.activeSelf) {
            return;
        }
        AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
        if (_optionsMenu == null) {
            _optionsMenu = Instantiate(optionsMenu);
            return;
        }
        if(_optionsMenu.activeSelf) {
            return;
        }
        _optionsMenu.SetActive(true);
    }

    public void OnExitButtonClick() {
        if (exitConfirmationMenu.activeSelf) {
            return;
        }
        AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
        exitConfirmationMenu.SetActive(true);
    }

    public void OnConfirmExitClick() {
        //save game first
        Application.Quit();
    }
    public void OnDeclineExitClick() {
        AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
        exitConfirmationMenu.SetActive(false);
    }
}
