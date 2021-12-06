using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Heading : MonoBehaviour
{
    //private float Input;
    //private float xInput;
    private float yInput;
    //private float zInput;

    SerialPort sp = new SerialPort("COM4", 9600);

    //float rotationSpeed = 45;
    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
    }


    // Update is called once per frame
    void Update()
    {

        string value = sp.ReadLine();
        yInput = float.Parse(value);

        //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles = new Vector3(0, yInput, 0);

        //moving the value of the Vector3 into Quanternion.eulerAngle format
        currentRotation.eulerAngles = currentEulerAngles;

        //apply the Quaternion.eulerAngles change to the gameObject
        transform.rotation = currentRotation;

        Debug.Log(transform.rotation);


    }
}

