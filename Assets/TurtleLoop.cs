using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleLoop : MonoBehaviour
{
    float timeCounter = 0;

    float speed;
    float width;
    float height;

    float xPreset = -4;
    float yPreset = 2;
    float zPreset = 6;

    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        //speed = 1;
        width = 3;
        height = 4;
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;

        float x = Mathf.Cos(timeCounter) * width + xPreset;
        float y = Mathf.Sin(timeCounter) * height + yPreset;
        float z = 0 + zPreset;

        //transform.position = new Vector3(x, y, z);

        //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles = new Vector3(timeCounter * 50, timeCounter * 20, timeCounter * 30);

        //moving the value of the Vector3 into Quanternion.eulerAngle format
        currentRotation.eulerAngles = currentEulerAngles;

        //apply the Quaternion.eulerAngles change to the gameObject
        transform.rotation = currentRotation;
    }
}
