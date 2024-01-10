using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NinjaMonoBehaviour
{
    [SerializeField] CharacterController characterController;
    public void Start() {
        GameManager.Instance.OnGameStart += EnableCharacterController;
    }
    public void EnableCharacterController() {
        characterController.enabled = true;
    }

    private void OnDestroy() {
        GameManager.Instance.OnGameStart -= EnableCharacterController;
    }
}
