using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ChangeTime : MonoBehaviour
{
    private float xInput;
    private int xInt; 

    SerialPort sp = new SerialPort("COM4", 9600);

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

        EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years, EnviroSkyMgr.instance.Time.Days, xInt, EnviroSkyMgr.instance.Time.Minutes, EnviroSkyMgr.instance.Time.Seconds);

    }
}
