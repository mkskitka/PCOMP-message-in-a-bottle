// a code under construction... to send two value to Unity without delay
//still need to add a handshaking (send a byte back to the microcontroller to request new data)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

//create a map function for C#
//map already been defined in ChangeTimeMapped code so change its name to Mapper
public static class ExtensionMethods
{
    public static float Mapper(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}

public class TwoInputs : MonoBehaviour
{
    private float TimeInput;
    private float RawHour;
    private float RawMinute;
    private int MyHour;
    private int MyMinute;

    SerialPort sp = new SerialPort("COM4", 9600);

    void Start()
    {
        sp.Open();

        StartCoroutine(ReadDataFromSerialPort());
    }

    IEnumerator ReadDataFromSerialPort()
    //only read when seri is avalible and same thing on arduino
    {
        while (true)
        {
            string[] values = sp.ReadLine().Split(',');
            TimeInput = float.Parse(values[0]);
            WeatherInput = float.Parse(values[1]);
            //zInput = float.Parse(values[2]);
            yield return new WaitForSeconds(.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //get input value from Arduino
        string value = sp.ReadLine();
        TimeInput = float.Parse(value);

        //map the time input from -180 to 180 to 0-24
        float RawHour = TimeInput.Mapper(-180, 180, 0, 48);

        //map the hour input to 1440 minutes in a day
        float RawMinute = RawHour.Mapper(0, 24, 0, 1440);

        //round the float input value to integer
        MyHour = Mathf.RoundToInt(RawHour);
        MyMinute = Mathf.RoundToInt(RawMinute);

        //need integer to set time
        EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years,
                                      EnviroSkyMgr.instance.Time.Days,
                                      MyHour,
                                      MyMinute,
                                      EnviroSkyMgr.instance.Time.Seconds);

        Debug.Log("Value: " + value);

    }
}