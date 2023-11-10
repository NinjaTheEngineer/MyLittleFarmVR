using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBlock : MonoBehaviour
{
    public float Width {  get; private set; }
    public float Length {  get; private set; }

    [SerializeField] GameObject visu;
    private void Awake() {
        Width = visu.transform.localScale.x;
        Length = visu.transform.localScale.z;
    }
}
