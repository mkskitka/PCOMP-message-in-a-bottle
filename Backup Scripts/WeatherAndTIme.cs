//this code is not working as we want because...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

//create a map function for C#
public static class ExtensionMethods
{
    public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}

public class ChangeTimeMapped : MonoBehaviour
{
    private float Input;
    private float RawHour;
    private float RawMinute;
    private int MyHour;
    private int MyMinute;

    SerialPort sp = new SerialPort("COM4", 9600);

    void Start()
    {
        sp.Open();
        
    }

    // Update is called once per frame
    void Update()
    {
        //get input value from Arduino
        string value = sp.ReadLine();
        Input = float.Parse(value);

        //map the input from -180 to 180 to 0-24
        float RawHour = Input.Map(-180, 180, 0, 24);

        //map the hour input to 1440 minutes in a day
        float RawMinute = RawHour.Map(0, 24, 0, 1440);
        int hours = Mathf.RoundToInt(Mathf.Floor(RawMinute / 60));
        int minutes = Mathf.RoundToInt(RawMinute) - (hours * 60);

        //round the float input value to integer
        //MyHour = Mathf.RoundToInt(RawHour);
        //MyMinute = Mathf.RoundToInt(RawMinute);

        //need integer to set time
        EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years,
                                      EnviroSkyMgr.instance.Time.Days,
                                      hours,
                                      minutes,
                                      EnviroSkyMgr.instance.Time.Seconds);

        Debug.Log("Value: " + value);

    }
}
