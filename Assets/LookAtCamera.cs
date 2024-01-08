using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Face the camera rotating only on the Y axis
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                       mainCam.transform.rotation * Vector3.up);

    }
}
