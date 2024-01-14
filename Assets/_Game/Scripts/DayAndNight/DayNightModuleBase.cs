using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public abstract class DayNightModuleBase : NinjaMonoBehaviour {
    protected DayNightCycle dayNightControl;

    void OnEnable() {
        var logId = "OnEnable";
        dayNightControl = GetComponent<DayNightCycle>();
        if (dayNightControl == null) {
            logw(logId, "No DayNightCycle component found => no-op");
            return;
        }
        dayNightControl.AddModule(this);
    }

    void OnDisable() {
        var logId = "OnDisable";
        if (dayNightControl == null) {
            logw(logId, "DayNightControl is null => no-op");
        }
        dayNightControl.RemoveModule(this);
    }

    public abstract void UpdateModule(float intensity);
}
