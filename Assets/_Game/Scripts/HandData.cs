using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class HandData : NinjaMonoBehaviour {
    public enum HandModelType { Left, Right }
    public HandModelType handType;
    public Transform root;
    public Animator animator;
    public Transform[] fingerBones;
}
