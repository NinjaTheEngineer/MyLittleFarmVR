using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;

public class RotateObject : NinjaMonoBehaviour
{
    public Vector3 rotationDirection = Vector3.up;
    public float rotationSpeed = 30f;
    public bool changeDirection = true;
    public float maxChangeInterval = 1f;
    private float changeDirectionTimer = 0f;

    void FixedUpdate()
    {
        if (changeDirection)
        {
            changeDirectionTimer += Time.fixedDeltaTime;

            if (changeDirectionTimer >= maxChangeInterval)
            {
                ChangeDirection();
            }
        }

        transform.Rotate(rotationDirection * rotationSpeed * Time.fixedDeltaTime);
    }
    public void ChangeDirection()
    {
        rotationDirection = Random.insideUnitSphere;

        changeDirectionTimer = 0f;
    }
}