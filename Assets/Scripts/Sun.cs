using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class Sun : NinjaMonoBehaviour {
    [SerializeField] Light light;
    [SerializeField] [Range(0, 24)] float timeOfDay;
    [SerializeField] float rotationSpeed;

    [Header("Lighting Preset")]
    [SerializeField] Gradient skyColor;
    [SerializeField] Gradient equatorColor;
    [SerializeField] Gradient sunColor;

    void OnValidate()
    {
        UpdateSunRotation();
        UpdateLighting();
    }
    void Update()
    {
        timeOfDay += Time.deltaTime * rotationSpeed;
        if(timeOfDay>24) {
            timeOfDay = 0;
        }
        UpdateSunRotation();
        UpdateLighting();
    }

    void UpdateSunRotation() {
        float rotation = Mathf.Lerp(-90, 270, timeOfDay/24);
        transform.rotation = Quaternion.Euler(rotation, transform.rotation.y, transform.rotation.z);
    }

    void UpdateLighting() {
        float timeFraction = timeOfDay/24;
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
        light.color = sunColor.Evaluate(timeFraction);
    }
}
