using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;
using static UnityEngine.Rendering.DebugUI;

public class PreviewBlock : NinjaMonoBehaviour {
    [SerializeField] GameObject visu;
    bool _inValidPosition = true;
    public bool InValidPosition {
        get => _inValidPosition;
        private set {
            if (_inValidPosition == value) {
                return;
            }
            _inValidPosition = value;
            visu.SetActive(_inValidPosition);
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
        InValidPosition = false;
    }
    void RemoveTrigger(Collider collider) {
        if (!collidersTriggered.Contains(collider)) {
            return;
        }
        collidersTriggered.Remove(collider);
        var collidersTriggeredCount = collidersTriggered.Count;
        logd("REMOVE TRIGGER", "Count="+ collidersTriggeredCount);
        InValidPosition = collidersTriggeredCount == 0;
    }

    private void OnTriggerExit(Collider other) {
        var logId = "OnTriggerExit";
        logd(logId, "Other=" + other.name);
        RemoveTrigger(other);
    }
    public void Show(bool isActive) {
        InValidPosition = isActive;
    }
}
