using NinjaTools;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[SelectionBase]
public class SeedBag : NinjaMonoBehaviour
{
    [SerializeField] SeedConfig _seedConfig;
    Seed seedPrefab;
    public Transform spawnPoint; // The point where seeds will be instantiated
    public float tiltThreshold = 110f;
    public float seedDropDelay = 0.85f;
    public float angle;
    public bool droppingSeed = false;
    public int seedCount;
    private void Start() {
        if(spawnPoint==null) {
            spawnPoint = GetComponentInChildren<SeedSpawnPoint>().transform;
        }
        if(seedPrefab==null) {
            seedPrefab = _seedConfig.seedPrefab;
        }
    }

    public void SetSeedConfiguration(SeedConfig seedConfig) {
        _seedConfig = seedConfig;
        seedCount = seedConfig.seedsAmount;
        seedPrefab = seedConfig.seedPrefab;
        StartCoroutine(DropSeedsRoutine());
    }
    IEnumerator DropSeedsRoutine() {
        var logId = "UpdateStatusRoutine";
        var waitForSeconds = new WaitForSeconds(seedDropDelay);
        while(seedCount>0) {
            if(droppingSeed) {
                InstantiateSeed();
            }
            yield return waitForSeconds;
        }
    }
    private void LateUpdate() {
        // Check the rotation of the bag
        angle = Vector3.Angle(Vector3.up, transform.up);
        droppingSeed = angle > tiltThreshold;
    }

    private void InstantiateSeed() {
        if(seedPrefab==null || spawnPoint==null || _seedConfig == null) {
            loge("InstantiateSeed", "Seed=" + seedPrefab.logf() + " SpawnPoint=" + spawnPoint.logf() + " SeedConfig=" + _seedConfig.logf());
            return;
        }
        seedCount--;
        Instantiate(seedPrefab, spawnPoint.position, Quaternion.identity).SetConfiguration(_seedConfig);
    }
}
