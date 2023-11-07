using System;
using UnityEngine;
using NinjaTools;
using System.Collections;

public class Clock : NinjaMonoBehaviour {
    private const float hoursToDegrees = 360f / 12f;
    private const float minutesToDegrees = 360f / 60f;
    public Transform hourHand;
    public Transform minuteHand;
    private void Start() {
        StartCoroutine(UpdateTime());
    }
    private IEnumerator UpdateTime() {
        var waitOneSecond = new WaitForSecondsRealtime(1f);
        while(true) {
            TimeSpan now = DateTime.Now.TimeOfDay;
            hourHand.localRotation = Quaternion.Euler((float)now.TotalHours * hoursToDegrees, 0f, 0f);
            minuteHand.localRotation = Quaternion.Euler((float)now.TotalMinutes * minutesToDegrees, 0f, 0f);
            yield return waitOneSecond;	
        }
    }
}