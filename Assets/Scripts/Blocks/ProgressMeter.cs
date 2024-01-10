using NinjaTools;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressMeter : NinjaMonoBehaviour {
    [SerializeField] float maxCapacity = 100f;
    [SerializeField] float currentCapacity;
    [SerializeField] float fillRate = 0.05f;
    [SerializeField] Image fillImage;
    [field: SerializeField] public bool IsComplete => currentCapacity >= maxCapacity;
    public Action OnComplete;
    private void Start() {
        currentCapacity = 0;
        fillImage.fillAmount = 0;
    }
    public void Increment() {
        var logId = "Increment";
        if(IsComplete) {
            logd(logId, "Progress Full!");
            return;
        }
        currentCapacity += fillRate;
        if(IsComplete) {
            SetProgressMeter();
            currentCapacity = maxCapacity;
            OnComplete?.Invoke();
        }
        fillImage.fillAmount = currentCapacity / maxCapacity;
    }

    public void SetProgressMeter(float maxCap = 100f, float startValue = 0f, float fillRate = 0.5f) {
        var logId = "SetProgressMeter";
        logd(logId, "Setting MaxCap=" + maxCap + " StartValue=" + startValue + " FillRate=" + fillRate);
        maxCapacity = maxCap;
        currentCapacity = startValue;
        this.fillRate = fillRate;
        fillImage.fillAmount = 0;
    }
}