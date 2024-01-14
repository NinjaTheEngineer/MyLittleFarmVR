using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class WaterParticleActivator : NinjaMonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    public float tiltThreshold = 50f;
    public float updateDelay = 0.1f;
    public float angle;
    private bool _activeAngle;
    public bool ActiveAngle {
        get => _activeAngle;
        private set {
            if (_activeAngle != value) {
                _activeAngle = value;
                if (_activeAngle) {
                    _particleSystem.Play();
                } else {
                    _particleSystem.Stop();
                }
            }
        }
    }
    private void LateUpdate() {
        // Check the rotation of the bag
        angle = Vector3.Angle(Vector3.up, transform.forward);
        ActiveAngle = angle > tiltThreshold;
    }
}
