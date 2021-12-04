using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class CubeRotation : MonoBehaviour
{
    private float xInput;
    private int xInt; 

    SerialPort sp = new SerialPort("COM4", 9600);

    //Vector3 currentEulerAngles;
    //Quaternion currentRotation;

    void Start()
    {
        sp.Open();
        EnviroSkyMgr.instance.useDistanceBlur = false; 
    }

    // Update is called once per frame
    void Update()
    {
        string value = sp.ReadLine();
        xInput = float.Parse(value);
        Debug.Log("Value: " + value);
        xInt = Mathf.RoundToInt(xInput);
        //xInput = int.Parse(value);


        //currentEulerAngles = new Vector3(0, 0, xInput);

        //moving the value of the Vector3 into Quanternion.eulerAngle format
        //currentRotation.eulerAngles = currentEulerAngles;

        //apply the Quaternion.eulerAngles change to the gameObject
        //transform.rotation = currentRotation;

        EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years, EnviroSkyMgr.instance.Time.Days, EnviroSkyMgr.instance.Time.Hours, xInt, EnviroSkyMgr.instance.Time.Seconds);



        //Debug.Log(transform.rotation);
    }
}
