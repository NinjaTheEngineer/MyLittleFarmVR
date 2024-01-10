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
    public float tiltThreshold = 90f;
    public float seedDropDelay = 0.5f;
    public float angle;
    public bool droppingSeed = false;
    private void Start() {
        if(spawnPoint==null) {
            spawnPoint = GetComponentInChildren<SeedSpawnPoint>().transform;
        }
        if(seedPrefab==null) {
            seedPrefab = _seedConfig.seedPrefab;
        }
        StartCoroutine(DropSeedsRoutine());
    }

    public void SetSeedConfiguration(SeedConfig seedConfig) {
        _seedConfig = seedConfig;
        seedPrefab = seedConfig.seedPrefab;
    } 
    IEnumerator DropSeedsRoutine() {
        var logId = "UpdateStatusRoutine";
        var waitForSeconds = new WaitForSeconds(seedDropDelay);
        while(true) {
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
        Instantiate(seedPrefab, spawnPoint.position, Quaternion.identity).SetConfiguration(_seedConfig);
    }
}
