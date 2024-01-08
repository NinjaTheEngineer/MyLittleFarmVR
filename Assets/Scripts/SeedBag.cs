using NinjaTools;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class SeedBag : NinjaMonoBehaviour
{
    [SerializeField] SeedConfig _seedConfig;
    public Seed seedPrefab;
    public Transform spawnPoint; // The point where seeds will be instantiated
    public float tiltThreshold = 90f;
    public float seedDropDelay = 0.5f;
    public float angle;
    public bool droppingSeed = true;
    private void Start() {
        StartCoroutine(DropSeedsRoutine());
    }
    public void SetSeedConfiguration(SeedConfig seedConfig) {
        _seedConfig = seedConfig;
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
        Instantiate(seedPrefab, spawnPoint.position, Quaternion.identity).SetConfiguration(_seedConfig);
    }
}
