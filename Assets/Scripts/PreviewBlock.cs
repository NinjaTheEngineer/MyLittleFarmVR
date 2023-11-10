using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;
using static UnityEngine.Rendering.DebugUI;

public class PreviewBlock : NinjaMonoBehaviour {
    [SerializeField] GameObject visu;
    bool _canBePlaced = true;
    public bool CanBePlaced {
        get => _canBePlaced;
        private set {
            if (_canBePlaced == value) {
                return;
            }
            visu.SetActive(value);
            _canBePlaced = value;
        }
    }
    bool _activePreview = false;
    private void OnTriggerEnter(Collider other) {
        var logId = "OnTriggerEnter";
        logd(logId, "Other=" + other.name);
        AddTrigger(other);
    }
    List<Collider> collidersTriggered = new List<Collider>();
    void AddTrigger(Collider collider) {
        if(collidersTriggered.Contains(collider)) {
            return;
        }
        collidersTriggered.Add(collider);
        CanBePlaced = false;
    }
    void RemoveTrigger(Collider collider) {
        if (!collidersTriggered.Contains(collider)) {
            return;
        }
        collidersTriggered.Remove(collider);
        var collidersTriggeredCount = collidersTriggered.Count;
        logd("REMOVE TRIGGER", "Count="+ collidersTriggeredCount);
        CanBePlaced = collidersTriggeredCount == 0;
    }

    private void OnTriggerExit(Collider other) {
        var logId = "OnTriggerExit";
        logd(logId, "Other=" + other.name);
        RemoveTrigger(other);
    }
    public void SetActivePreview(bool isActive) {
        CanBePlaced = isActive;
    }
}
