using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : NinjaMonoBehaviour
{
    public GameObject prefab, prefab1;

    public void SpawnFirstPrefab() {
        Instantiate(prefab, transform);
    }
    public void SpawnSecondPrefab() {
        Instantiate(prefab1, transform);
    }
}
