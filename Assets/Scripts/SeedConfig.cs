using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName ="SeedConfig", menuName ="ScriptableObjects/Create New Seed Config")]
public class SeedConfig : ScriptableObject {
    public GameObject cropPrefab;
    public DG.Tweening.Ease growScaleEase;
    public Vector3 initialGrowScale = new Vector3(0.05f, 0.05f, 0.05f);
    public float initialYOffset = 0.1f;
    public float growthTime = 90f;
    public float maxGrowDelay = 20f;
}
