using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flagController : MonoBehaviour
{
    bool isRotating = true;

    float accumulatedRotation = 0;
    public int rorationCount = 0;


    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(2))
        {
            isRotating = !isRotating;
        }
        
        if (isRotating == true)
        {
            transform.Rotate(0, 10, 0);
            accumulatedRotation += 10;

            if (accumulatedRotation >= 360f)
            {
                accumulatedRotation = 0;
                rorationCount++;
            }
        }
    }
}
