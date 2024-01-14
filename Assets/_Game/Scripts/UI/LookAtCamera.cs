using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        if(mainCam==null) {
            mainCam = Camera.main;
            return;
        }
        transform.LookAt(mainCam.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);


    }
}
