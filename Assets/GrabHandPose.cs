using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using NinjaTools;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GrabHandPose : NinjaMonoBehaviour {
    public float poseTransitionDuration = 0.2f;
    public HandData rightHandPose, leftHandPose;
    private Vector3 startHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startHandRotation;
    private Quaternion finalHandRotation;
    private Quaternion[] startFingerRotations;
    private Quaternion[] finalFingerRotations;

    private void Start() {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(ResetPose);
        rightHandPose.gameObject.SetActive(false);
        leftHandPose.gameObject.SetActive(false);
    }

    public void SetupPose(BaseInteractionEventArgs arg) {
        var logId = "SetupPose";
        if(arg.interactorObject is XRBaseInteractor) {
            logd(logId, "Arg="+arg.logf()+" interactorObject="+arg.interactorObject.logf());
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;

            var currentHandData = handData.handType==HandData.HandModelType.Left ? leftHandPose : rightHandPose;

            SetHandDataValues(handData, currentHandData);
            SetHandData(handData);
        } else {
            logw(logId, "Arg="+arg.logf()+" interactorObject="+arg.interactorObject.logf()+" => no-op");
        }
    }
    public void ResetPose(BaseInteractionEventArgs arg) {
        var logId = "SetupPose";
        if(arg.interactorObject is XRBaseInteractor) {
            logd(logId, "Arg="+arg.logf()+" interactorObject="+arg.interactorObject.logf());
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = true;

            ResetHandData(handData);
        } else {
            logw(logId, "Arg="+arg.logf()+" interactorObject="+arg.interactorObject.logf()+" => no-op");
        }
    }

    public void SetHandDataValues(HandData startHand, HandData finalHand) {
        startHandPosition = startHand.root.localPosition;
        finalHandPosition = finalHand.root.localPosition;

        startHandRotation = startHand.root.localRotation;
        finalHandRotation = finalHand.root.localRotation;

        int fingerBonesCount = startHand.fingerBones.Length;
        startFingerRotations = new Quaternion[fingerBonesCount];
        finalFingerRotations = new Quaternion[fingerBonesCount];
    
        for (int i = 0; i < fingerBonesCount; i++) {
            startFingerRotations[i] = startHand.fingerBones[i].localRotation;
            finalFingerRotations[i] = finalHand.fingerBones[i].localRotation;
        }
    }
    public void ResetHandData(HandData handData, bool smooth = false) {
        var logId = "ResetHandData";
        if(handData==null) {
            logw(logId, "HandData is null => no-op");
            return;
        }
        if(smooth) {
            StartCoroutine(SetHandDataRoutine(handData, finalHandPosition, startHandPosition, finalHandRotation, startHandRotation, finalFingerRotations, startFingerRotations));
        } else {
            SetHandData(handData, startHandPosition, startHandRotation, startFingerRotations);
        }
    }
    public void SetHandData(HandData handData, bool smooth = false) {
        var logId = "SetHandData";
        if(handData==null) {
            logw(logId, "HandData is null => no-op");
            return;
        }
        if(smooth) {
            StartCoroutine(SetHandDataRoutine(handData, startHandPosition, finalHandPosition, startHandRotation, finalHandRotation, startFingerRotations, finalFingerRotations));
        } else {
            SetHandData(handData, finalHandPosition, finalHandRotation, finalFingerRotations);
        }
    }
    public void SetHandData(HandData handData, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotations) {
        var logId = "SetHandData";
        if(handData==null) {
            logw(logId, "HandData is null => no-op");
            return;
        }
        handData.root.localPosition = newPosition;
        handData.root.localRotation = newRotation;

        int fingerBonesCount = newBonesRotations.Length;
        for (int i = 0; i < fingerBonesCount; i++) {
            handData.fingerBones[i].localRotation = newBonesRotations[i];
        }
    }

    public IEnumerator SetHandDataRoutine(HandData handData, Vector3 startPosition, Vector3 targetPosition, Quaternion startRotation, Quaternion targetRotation, Quaternion[] startBonesRotations, Quaternion[] targetBonesRotations) {
        var logId = "SetHandDataRoutine";
        if(handData==null) {
            logw(logId, "HandData is null => no-op");
            yield break;
        }
        float timer = 0;
        int fingerBonesCount = targetBonesRotations.Length;
        while(timer < poseTransitionDuration) {
            var interpolationRate = timer/poseTransitionDuration;
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, interpolationRate);
            Quaternion rotation = Quaternion.Lerp(startRotation, targetRotation, interpolationRate);

            handData.root.localPosition = position;
            handData.root.localRotation = rotation;

            for (int i = 0; i < fingerBonesCount; i++) {
                handData.fingerBones[i].localRotation = Quaternion.Lerp(startBonesRotations[i], targetBonesRotations[i], interpolationRate);
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
#if UNITY_EDITOR
    [MenuItem("Tools/Mirror Selected Right Grab Pose")]
    public static void MirrorRightPose() {
        var logId = "MirrorRightPose";
        GrabHandPose handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        if(handPose==null) {
            Utils.logw(logId, "HandPose is null => no-op");
            return;
        }
        handPose.MirrorPose(handPose.leftHandPose, handPose.rightHandPose);
    }
#endif
    public void MirrorPose(HandData poseToMirror, HandData originalPose) {
        var logId = "MirrorPose";
        Vector3 mirroredPosition = originalPose.root.localPosition;
        mirroredPosition.x *= -1;

        Quaternion mirroredRotation = originalPose.root.localRotation;
        mirroredRotation.z *= -1;

        logd(logId, "Setting PoseToMirror to Position="+mirroredPosition.logf()+" Rotation="+mirroredRotation.logf());
        
        poseToMirror.root.localPosition = mirroredPosition;
        poseToMirror.root.localRotation = mirroredRotation;

        int fingerBonesCount = originalPose.fingerBones.Length;
        for (int i = 0; i < fingerBonesCount; i++) {
            poseToMirror.fingerBones[i].localRotation = originalPose.fingerBones[i].localRotation;
        }
    }
}
