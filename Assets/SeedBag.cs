using NinjaTools;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class SeedBag : NinjaMonoBehaviour
{
    public Seed seedPrefab;
    public Transform spawnPoint; // The point where seeds will be instantiated
    public float tiltThreshold = 90f;
    public float seedDropDelay = 0.5f;
    public float angle;
    public bool droppingSeed = true;
    private void Start() {
        StartCoroutine(DropSeedsRoutine());
    }
    IEnumerator DropSeedsRoutine() {
        var logId = "DropSeedsRoutine";
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
        Instantiate(seedPrefab, spawnPoint.position, Quaternion.identity);
    }
}
