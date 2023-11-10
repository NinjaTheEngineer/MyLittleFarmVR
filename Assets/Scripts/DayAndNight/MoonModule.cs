using UnityEngine;
using NinjaTools;

public class MoonModule : DayNightModuleBase {
    [SerializeField]
    Light moon;
    [SerializeField]
    Gradient moonColor;
    [SerializeField]
    float baseIntensity;
    private void Start() {
        moon.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
    }
    public override void UpdateModule(float intensity) {
        moon.color = moonColor.Evaluate(1 - intensity);
        moon.intensity = (1 - intensity) * baseIntensity + 0.05f;
    }
}
