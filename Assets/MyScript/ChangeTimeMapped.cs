//this code is not working as we want because...

using System.Collections;
//using System.Diagnostics;
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
    
    public static float xInput;
    public static float yInput;
    public static float zInput;
    private static float firstZInput = -1;
    private static float initialYDegrees = 174;
    private static float RawMinute = 0;

    private float RawHour;
    private int MyHour;
    private int MyMinute;
    List<EnviroWeatherPreset> presets;
    EnviroWeatherPreset ClearSkyPreset;
    EnviroWeatherPreset HeavyRainPreset;
    private string CLEAR_SKY = "Clear Sky";
    private string HEAVY_RAIN = "Heavy Rain";
    //EnviroWeatherPreset CurrentPreset;
    private string CurrentPreset; 
   


    SerialPort sp = new SerialPort("COM4", 9600);

    void Start()
    {
        sp.Open();
        StartCoroutine(ReadDataFromSerialPort());
        CurrentPreset = CLEAR_SKY;



    }

    IEnumerator ReadDataFromSerialPort()
    //only read when seri is avalible and same thing on arduino
    {
        float rawZInput;
        while (true)
        {
            string[] values = sp.ReadLine().Split(',');
            xInput = float.Parse(values[0]);
            yInput = float.Parse(values[1]);
            float z = float.Parse(values[2]);
            if (firstZInput == -1 && z !=0)
            {
                firstZInput = z;  
            }
            rawZInput = z;
            //Debug.Log("firstZ: " + firstZInput.ToString() + ", LatestZ: " + rawZInput.ToString());
            zInput = initialYDegrees + ( firstZInput - rawZInput );
            //zInput = initialYDegrees;
            yield return new WaitForSeconds(.05f);
        }


    }

    // Update is called once per frame
    void Update()
    {

        //presets = EnviroSkyMgr.instance.Weather.weatherPresets;
        //Debug.Log("Presets Count: " + presets.Count.ToString());
        //get input value from Arduino
        //string value = sp.ReadLine();
        //Input = float.Parse(value);

        //map the input from -180 to 180 to 0-24
        //Debug.Log("yInput: " + yInput.ToString());
        presets = EnviroSkyMgr.instance.Weather.weatherPresets;
        //Debug.Log("Presets Count: " + presets.Count.ToString());
        
        
        if (presets.Count > 0 && ClearSkyPreset == null)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                Debug.Log("name: " + presets[i].Name);

                if (presets[i].Name == CLEAR_SKY)
                {
                    ClearSkyPreset = presets[i];
                }
                if (presets[i].Name == HEAVY_RAIN)
                {
                    HeavyRainPreset = presets[i];
                }
            }
        }
        float RawHour = xInput.Map(-180, 180, 0, 24);


        //map the hour input to 1440 minutes in a day
        RawMinute = RawHour.Map(0, 24, 0, 1440);
        //RawMinute = RawMinute + 1; 
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

        // Update Weather ... Clear Sky, Heavy Rain 
        //CurrentPreset = EnviroSkyMgr.instance.Weather.currentActiveWeatherPreset;
        if (presets.Count != 0)
        {

            if (yInput < 0 && CurrentPreset != CLEAR_SKY)
            {
                Debug.Log("changing to clear sky");
                EnviroSkyMgr.instance.ChangeWeather(ClearSkyPreset);
                CurrentPreset = CLEAR_SKY;
            }
            if (yInput >= 0 && CurrentPreset != HEAVY_RAIN)
            {
                Debug.Log("changing to Heavy Rain");
                EnviroSkyMgr.instance.ChangeWeather(HeavyRainPreset);
                CurrentPreset = HEAVY_RAIN;
            }
        }

        // Debug.Log("Value: " + value);

    }
}
