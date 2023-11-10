using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using UnityEngine.Rendering;

public class DayNightCycle : NinjaMonoBehaviour {
    [Header("Time")]
    [Tooltip("Day lenght in minutes")]
    [SerializeField]
    [Range(0.1f, 10f)]
    float _targetDayLength = 0.5f;
    public float TargetDayLength => _targetDayLength;

    [SerializeField]
    float elapsedTime;

    [SerializeField]
    [Range(0f, 1f)]
    float _timeOfDay;
    public float TimeOfDay => _timeOfDay;
    [SerializeField]
    int _dayNumber = 0;
    public int DayNumber => _dayNumber;
    [SerializeField]
    int _yearNumber = 0;
    int YearNumber => _yearNumber;

    [SerializeField]
    float _timeScale = 100f;

    [SerializeField]
    int _yearLength = 100;
    int YearLength => _yearLength;
    public bool pause = false;

    [SerializeField]
    AnimationCurve timeCurve;
    float timeCurveNormalization;

    [Header("Sun light")]
    [SerializeField]
    Transform dailyRotation;

    [SerializeField]
    Light sun;
    float intensity;
    [SerializeField]
    float sunBaseInstensity = 1f;
    [SerializeField]
    float sunVariation = 1.5f;
    [SerializeField]
    Gradient sunColor;

    [Header("Seasonal Variables")]
    [SerializeField]
    Transform sunSeasonalRotation;
    [SerializeField]
    [Range(-45f, 45f)]
    float maxSeasonalTilt;

    [Header("Modules")]
    [SerializeField]
    List<DayNightModuleBase> moduleList = new List<DayNightModuleBase>();

    void Start() {
        NormalizeTimeCurve();
        sun.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }

    void Update() {
        if (pause) return;
        UpdateTimeScale();
        UpdateTime();
        AdjustSunRotation();
        SunIntensity();
        AdjustSunColor();
        UpdateModules();
        UpdateClock();
    }

    void UpdateTimeScale() {
        _timeScale = 24 / (_targetDayLength / 60);
        _timeScale *= timeCurve.Evaluate(_timeOfDay);
        _timeScale /= timeCurveNormalization;
    }

    void UpdateTime() {
        _timeOfDay += Time.deltaTime * _timeScale / 86400;
        elapsedTime += Time.deltaTime;
        if (_timeOfDay > 1) //new day 
        {
            elapsedTime = 0;
            _dayNumber++;
            _timeOfDay -= 1;

            if (_dayNumber > _yearLength) {
                _yearNumber++;
                _dayNumber = 0;
            }
        }
    }

    void UpdateClock() {
        var logId = "UpdateClock";
        float time = elapsedTime / (TargetDayLength * 60);
        int hour = Mathf.FloorToInt(time * 24);
        int minute = Mathf.FloorToInt( ((time * 24)-hour)*60 );
        logd(logId, "Time=" + time + " Hour=" + hour + " Minute=" + minute, true, 10f);
    }

    void AdjustSunRotation() {
        float sunAngle = _timeOfDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, sunAngle));
        float seasonalAngle = -maxSeasonalTilt * Mathf.Cos(_dayNumber / _yearLength * 2f * Mathf.PI);
        
        sunSeasonalRotation.localRotation = Quaternion.Euler(new Vector3(seasonalAngle, 0f, 0f));
    }
    [SerializeField]
    float stepSize = 0.01f;
    void NormalizeTimeCurve() {
        int numberOfSteps = Mathf.FloorToInt(1f / stepSize);
        float curveTotal = 0;
        for (int i = 0; i < numberOfSteps; i++) {
            curveTotal += timeCurve.Evaluate(i * stepSize);
        }

        timeCurveNormalization = curveTotal / numberOfSteps;
    }
    void SunIntensity() {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp01(intensity);

        sun.intensity = intensity * sunVariation + sunBaseInstensity;
    }

    void AdjustSunColor() {
        sun.color = sunColor.Evaluate(intensity);
    }

    public void AddModule(DayNightModuleBase module) {
        var logId = "AddModule";
        if (module == null) {
            logd(logId, "Module is null => no-op");
            return;
        }
        moduleList.Add(module);
    }
    public void RemoveModule(DayNightModuleBase module) {
        var logId = "RemoveModule";
        if (module == null) {
            logd(logId, "Module is null => no-op");
            return;
        }
        moduleList.Remove(module);
    }

    void UpdateModules() {
        foreach (var module in moduleList) {
            module.UpdateModule(intensity);
        }
    }
}
