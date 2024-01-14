using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NinjaTools;

public class AnimateHandOnInput : NinjaMonoBehaviour {
    public Animator handAnimator;
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    private void Update() {
        var logId = "Update";
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        float gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
        handAnimator.SetFloat("Grip", gripValue);
        //logd(logId, "GripValue="+gripValue+" TriggerValue="+triggerValue, true);
    }
}
