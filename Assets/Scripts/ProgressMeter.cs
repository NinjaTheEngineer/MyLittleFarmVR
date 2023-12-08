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
    public static Action OnComplete;
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

    public void SetProgressMeter(float maxCap = 100f, float startCap = 0f, float fillRate = 0.5f) {
        maxCapacity = maxCap;
        currentCapacity = startCap;
        this.fillRate = fillRate;
    }
}