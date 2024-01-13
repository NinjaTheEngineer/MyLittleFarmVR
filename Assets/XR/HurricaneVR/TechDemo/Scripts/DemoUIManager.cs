using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HurricaneVR.TechDemo.Scripts
{
    public class DemoUIManager : MonoBehaviour
    {
        public HVRPlayerController Player;
        public HVRCameraRig CameraRig;
        public HVRPlayerInputs Inputs;

        public TextMeshProUGUI SitStandText;
        public TextMeshProUGUI PauseText;
        public TextMeshProUGUI ForceGrabText;
        public TextMeshProUGUI LeftForceText;
        public TextMeshProUGUI RightForceText;
        public Slider TurnRateSlider;
        public Slider SnapTurnSlider;
        public TextMeshProUGUI TurnRateText;
        public TextMeshProUGUI SnapRateText;
        public Toggle SmoothTurnToggle;
        public Toggle LineGrabTrigger;

        public HVRForceGrabber LeftForce;
        public HVRForceGrabber RightForce;

        public HVRJointHand LeftHand;
        public HVRJointHand RightHand;

        private Transform leftparent;
        private Transform rightParent;

        private bool Paused;

        private void OnEnable() {
            if(initialized) {
                SetupCameraTransform();
                StartCoroutine(CloseMenuRoutine());
            }
        }

        void Start()
        {
            if (!Player)
            {
                Player = GameObject.FindObjectsOfType<HVRPlayerController>().FirstOrDefault(e => e.gameObject.activeInHierarchy);
            }

            if (Player)
            {
                if (!CameraRig)
                {
                    CameraRig = Player.GetComponentInChildren<HVRCameraRig>();
                }

                if (!Inputs)
                {
                    Inputs = Player.GetComponentInChildren<HVRPlayerInputs>();
                }

                if (!LeftHand) LeftHand = Player.Root.GetComponentsInChildren<HVRHandGrabber>().FirstOrDefault(e => e.HandSide == HVRHandSide.Left)?.GetComponent<HVRJointHand>();
                if (!RightHand) RightHand = Player.Root.GetComponentsInChildren<HVRHandGrabber>().FirstOrDefault(e => e.HandSide == HVRHandSide.Right)?.GetComponent<HVRJointHand>();
            }


            if(LeftHand) leftparent = LeftHand.transform.parent;
            if(RightHand)rightParent = RightHand.transform.parent;

            UpdateSitStandButton();
            UpdateForceGrabButton();
            TurnRateSlider.value = Player.SmoothTurnSpeed;
            SnapTurnSlider.value = Player.SnapAmount;

            TurnRateText.text = Player.SmoothTurnSpeed.ToString();
            SnapRateText.text = Player.SnapAmount.ToString();

            SmoothTurnToggle.isOn = Player.RotationType == RotationType.Smooth;
            LineGrabTrigger.isOn = HVRSettings.Instance.LineGrabTriggerLoose;

            TurnRateSlider.onValueChanged.AddListener(OnTurnRateChanged);
            SnapTurnSlider.onValueChanged.AddListener(OnSnapTurnRateChanged);
            SmoothTurnToggle.onValueChanged.AddListener(OnSmoothTurnChanged);
            LineGrabTrigger.onValueChanged.AddListener(OnLineGrabTriggerChanged);

            LeftForce = Player.transform.root.GetComponentsInChildren<HVRForceGrabber>().FirstOrDefault(e => e.HandSide == HVRHandSide.Left);
            RightForce = Player.transform.root.GetComponentsInChildren<HVRForceGrabber>().FirstOrDefault(e => e.HandSide == HVRHandSide.Right);
            UpdateLeftForceButton();
            UpdateRightForceButton();
            if(initialized) {
                return;
            }
            initialized = true;
            SetupCameraTransform();
            StartCoroutine(CloseMenuRoutine());
        }
        bool initialized = false;
        public float heightOffSet = 0.5f;
        public void SetupCameraTransform() {
            var camTransform = CameraRig.Camera.transform;
            var cameraDir = camTransform.forward;
            transform.position = camTransform.position + cameraDir * 2f + Vector3.up * heightOffSet;
            transform.forward = cameraDir;
        }

        public void CloseMenu() {
            AudioManager.Instance.PlaySFX(SoundType.CloseMenu, transform.position);
            Destroy(gameObject);
        }


        private IEnumerator CloseMenuRoutine() {
            //I want to check if the player is facing the menu, and if not, turn it off.
            
            var waitForSeconds = new WaitForSeconds(0.5f);
            while(true) {
                CheckMenuFacing();
                yield return waitForSeconds;
            }
        }
        public float closeAngle = 70f;
        public float angle;
        private void CheckMenuFacing() {
            var camTransform = CameraRig.Camera.transform;
            var cameraDir = camTransform.forward;
            var menuDir = transform.position - camTransform.position;
            angle = Vector3.Angle(cameraDir, menuDir);
            if(angle > closeAngle) {
                CloseMenu();
            }
        }

        private void OnLineGrabTriggerChanged(bool arg0)
        {
            HVRSettings.Instance.LineGrabTriggerLoose = arg0;
        }

        public void CalibrateHeight()
        {
            AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
            if (CameraRig)
                CameraRig.Calibrate();
        }

        public void OnSitStandClicked()
        {
            AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
            var index = (int)CameraRig.SitStanding;
            index++;
            if (index > 2)
            {
                index = 0;
            }

            CameraRig.SetSitStandMode((HVRSitStand)index);
            UpdateSitStandButton();
        }

        public void OnForceGrabClicked()
        {
            AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
            var index = (int)Inputs.ForceGrabActivation;
            index++;
            if (index > 1)
            {
                index = 0;
            }

            Inputs.ForceGrabActivation = (HVRForceGrabActivation)index;
            UpdateForceGrabButton();
        }

        private void UpdateForceGrabButton()
        {
            ForceGrabText.text = Inputs.ForceGrabActivation.ToString();
        }

        private void UpdateSitStandButton()
        {
            SitStandText.text = CameraRig.SitStanding.ToString();
        }

        public void OnTurnRateChanged(float rate)
        {
            Player.SmoothTurnSpeed = rate;
            TurnRateText.text = Player.SmoothTurnSpeed.ToString();
        }

        public void OnSnapTurnRateChanged(float rate)
        {
            Player.SnapAmount = rate;
            SnapRateText.text = Player.SnapAmount.ToString();
        }

        public void OnSmoothTurnChanged(bool smooth)
        {
            Player.RotationType = smooth ? RotationType.Smooth : RotationType.Snap;
        }

        public void OnLeftForceGrabModeClicked()
        {
            AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
            if (LeftForce)
            {
                if (LeftForce.GrabStyle == HVRForceGrabMode.ForcePull)
                {
                    LeftForce.GrabStyle = HVRForceGrabMode.GravityGloves;
                }
                else
                {
                    LeftForce.GrabStyle = HVRForceGrabMode.ForcePull;
                }

                UpdateLeftForceButton();
            }
        }

        public void OnRightForceGrabModeClicked()
        {
            AudioManager.Instance.PlaySFX(SoundType.ButtonClick, transform.position);
            if (RightForce)
            {
                if (RightForce.GrabStyle == HVRForceGrabMode.ForcePull)
                {
                    RightForce.GrabStyle = HVRForceGrabMode.GravityGloves;
                }
                else
                {
                    RightForce.GrabStyle = HVRForceGrabMode.ForcePull;
                }

                UpdateRightForceButton();
            }
        }

        private void UpdateLeftForceButton()
        {
            LeftForceText.text = LeftForce.GrabStyle.ToString();
        }

        private void UpdateRightForceButton()
        {
            RightForceText.text = RightForce.GrabStyle.ToString();
        }

        public void TogglePause()
        {
            if (LeftHand && RightHand)
            {
                if (Paused)
                {
                    PauseText.text = "Pause";
                    Time.timeScale = 1f;
                    LeftHand.transform.parent = leftparent;
                    RightHand.transform.parent = rightParent;
                }
                else
                {
                    PauseText.text = "Unpause";
                    Time.timeScale = .00000001f;
                    LeftHand.transform.parent = LeftHand.Target;
                    RightHand.transform.parent = RightHand.Target;
                }

                Paused = !Paused;
            }
        }
    }
}
