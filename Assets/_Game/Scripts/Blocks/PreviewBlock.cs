using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class PreviewBlock : NinjaMonoBehaviour {
    [SerializeField] GameObject visu;
    bool _inValidPosition = true;
    bool shouldBeVisible = true;
    public bool InValidPosition {
        get => _inValidPosition;
        private set {
            var logId = "InValidPosition_set";
            if (_inValidPosition == value) {
                logd(logId, "Setting InValidPosition to same value=" + _inValidPosition + " => returning");
                return;
            }
            logd(logId, "Setting InValidPosition from " + _inValidPosition + " to " + value);
            _inValidPosition = value && shouldBeVisible;
            visu.SetActive(_inValidPosition);
        }
    }
    private void OnTriggerEnter(Collider other) {
        var logId = "OnTriggerEnter";
        logd(logId, "Other=" + other.name);
        AddTrigger(other);
    }
    List<Collider> collidersTriggered = new List<Collider>();
    public LayerMask ignoreLayers;
    void AddTrigger(Collider collider) {
        var logId = "AddTrigger";
        if(collider == null) {
            logw(logId, "Collider is null => returning");
            return;
        }
        if (IsLayerInLayerMask(collider.gameObject.layer, ignoreLayers)) {
            logd(logId, "Collider=" + collider.logf() + " is in ignoreLayers => returning");
            return;
        }
        collidersTriggered.Add(collider);
        InValidPosition = false;
    }
    public bool IsLayerInLayerMask(int layer, LayerMask layerMask) {
        return layerMask == (layerMask | (1 << layer));
    }
    void RemoveTrigger(Collider collider) {
        var logId = "RemoveTrigger";
        if (!collidersTriggered.Contains(collider)) {
            logw(logId, "Collider="+collider.logf()+" is not in list");
            return;
        }
        collidersTriggered.Remove(collider);
        var collidersTriggeredCount = collidersTriggered.Count;
        logd(logId, "collidersTriggeredCount=" + collidersTriggeredCount);
        InValidPosition = collidersTriggeredCount == 0;
    }

    private void OnTriggerExit(Collider other) {
        var logId = "OnTriggerExit";
        logd(logId, "Other=" + other.name);
        RemoveTrigger(other);
    }
    public void Show(bool isShown) {
        shouldBeVisible = isShown;
        InValidPosition = isShown;
    }
}
