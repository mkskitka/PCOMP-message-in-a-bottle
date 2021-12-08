//this code is not working as we want because...

using System.Collections;
//using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine;
using System.Threading;



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
    
    public static float xInput =0;
    public static float yInput =0;
    public static float zInput=0;
    public static float AccX = 0; 
    public static float AccY = 0; 
    public static float AccZ = 0; 

    private static float firstZInput = -1;
    private static float initialYDegrees = 174;
    private static float RawMinutes = 0;

    private float RawHour;
    private int MyHour;
    private int MyMinute;
    List<EnviroWeatherPreset> presets;
    EnviroWeatherPreset ClearSkyPreset;
    EnviroWeatherPreset HeavyRainPreset;
    private string CLEAR_SKY = "Clear Sky";
    private string HEAVY_RAIN = "Heavy Rain";
    private string CurrentPreset;
    private int numberPastThreshold = 0;
    private float timeFrame = 5;
    private int windowIncrement = 0;
    private float previousAcceleration = 0;
    private float currentAcceleration = 0;
    private float maxAcceleration = 0;
    private float Threshold = 50;
    private float maxHeadingAcc = 0;
    private bool prevTimeDir = true; 

    
    private SerialPort stream;
    private Thread thread;
    private Queue outputQueue;    // From Unity to Arduino
    private Queue inputQueue;    // From Arduino to Unity
    private int baudRate = 9600;
    // WINDOWS change to COM4 
    private string portName = "/dev/tty.usbmodem14401";
    private int timeout = 100;
    private bool loop = true;

    void OnApplicationQuit()
    {
        loop = false; 
        Debug.Log("Application ending after " + Time.time + " seconds");

    }

    void StartThread()
    {
        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());
        // Creates and starts the thread
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    void ThreadLoop()
    {
            // Opens the connection on the serial port
        stream = new SerialPort(portName, baudRate);
        stream.ReadTimeout = timeout;
        stream.Open();
        if (stream.IsOpen)
        {
            Debug.Log("Serial Port Open.");
        }

        // Looping
        while (loop)
        {
            // Send to Arduino
            if (outputQueue.Count != 0)
            {
                string command = outputQueue.Dequeue().ToString();
                SendDataToSerialPort(command);
            }

            // Read from Arduino
            string result = ReadFromSerialPort(timeout);
            if (result != null)
                inputQueue.Enqueue(result);
        }
    }


/*
 *  For external use by the MonoScript
 */
    void SendToArduino(string command)
    {
        outputQueue.Enqueue(command);

    }

    string ReadFromArduino()
    {
        if (inputQueue.Count == 0)
        {
            return null;
        }

        return (string)inputQueue.Dequeue();
    }

    void Start()
    {
         StartThread();
        CurrentPreset = CLEAR_SKY;
        // Get a list of serial port names
        string[] ports = SerialPort.GetPortNames();
 
        // Display each port name to the console
        // foreach (string port in ports) {
        //     Debug.Log(port);
        // }

    }

    void SendDataToSerialPort(string command)
    {
        if (stream.IsOpen)
        {
            //Debug.Log("Sending Data to Serial Port.");
            byte[] bites = System.Text.Encoding.UTF8.GetBytes(command);
            stream.Write(bites, 0, 1);


        }
    }

    string ReadFromSerialPort(int timeout)
    //only read when seri is avalible and same thing on arduino
    {
        try {
            string input = stream.ReadLine();
            return input;
        }
        catch(TimeoutException e)
        {
            //Debug.Log("Error: " + e);
            stream.BaseStream.Flush();
            return null;
        }
    }

    void parseInput(string input)
    {
        try
        {
            if (input == null)
            {
                return;
            }
            string[] values = input.Split(',');
            float rawZInput;
            if (values.Length >= 6)
            {
                xInput = float.Parse(values[0]);
                yInput = float.Parse(values[1]);
                float z = float.Parse(values[2]);
                AccX = Mathf.Abs(float.Parse(values[3]));
                AccZ = Mathf.Abs(float.Parse(values[4]));
                AccY = float.Parse(values[5]);
                currentAcceleration = AccX + AccZ;
                Debug.Log("Y Acc: " + AccY.ToString());
                //Debug.Log("xInput " + currentAcceleration.ToString());
                //Debug.Log("current Acceleration: " + currentAcceleration.ToString());

                if (firstZInput == -1 && z != 0)
                {
                    firstZInput = z;
                }
                rawZInput = z;
                //Debug.Log("firstZ: " + firstZInput.ToString() + ", LatestZ: " + rawZInput.ToString());
                zInput = initialYDegrees + (firstZInput - rawZInput);
            }
            else
            {
                Debug.Log("Less than 5 values read into Unity");
            }
        }
        catch(Exception e)
        {
            Debug.Log("Input: " + input);
        }
    }

    void changeTime() 
    {


        float yAccAbs = Mathf.Abs(AccY);
        if(yAccAbs > maxHeadingAcc && yAccAbs > 100) {
            maxHeadingAcc = yAccAbs; 
        }
        int timeAccInMin = Mathf.RoundToInt(maxHeadingAcc.Map(0, 800, 0, 10));

        //Debug.Log("maxHeading: " + maxHeadingAcc.ToString() + ", timeAccInMin:  " + timeAccInMin);

        RawMinutes = (EnviroSkyMgr.instance.Time.Hours * 60) + EnviroSkyMgr.instance.Time.Minutes + timeAccInMin;

        if(RawMinutes >= 1440) {
            RawMinutes = RawMinutes - 1440;
        }
        //Debug.Log("Raw Minutes: " + RawMinutes.ToString());
        int hours = Mathf.RoundToInt(Mathf.Floor(RawMinutes / 60));
        int minutes = Mathf.RoundToInt(RawMinutes) - (hours * 60);

        //need integer to set time
        EnviroSkyMgr.instance.SetTime(EnviroSkyMgr.instance.Time.Years,
                                      EnviroSkyMgr.instance.Time.Days,
                                      hours,
                                      minutes,
                                      EnviroSkyMgr.instance.Time.Seconds);
        if(maxHeadingAcc > 1) {                             
            maxHeadingAcc = maxHeadingAcc * .99f;
        }
    }


    // Update is called once per frame
    void Update()
    {
        string input = ReadFromArduino();
        parseInput(input);
        SendToArduino("ready");
        changeTime();
        changeWeather();

    }

    void changeWeather(){
        var currentWindow = Mathf.Floor(Time.realtimeSinceStartup / timeFrame);
        //Debug.Log("current window: " + currentWindow.ToString() + " : " + windowIncrement.ToString());
       // Debug.Log(Time.realtimeSinceStartup.ToString());
            
        if(currentWindow > windowIncrement)
        {
            windowIncrement += 1;
            // use Number past Threshold to change the weather and wave height. 
            if(numberPastThreshold > 4)
            {
                EnviroSkyMgr.instance.ChangeWeather(HeavyRainPreset);
            }
            else if(numberPastThreshold > 3 && numberPastThreshold <= 4) {
                //Cloudy or something
                EnviroSkyMgr.instance.ChangeWeather(HeavyRainPreset);
            }
            else if(numberPastThreshold <=3 && numberPastThreshold > 0) {
                EnviroSkyMgr.instance.ChangeWeather(HeavyRainPreset);
                //something else
            }
            else if(numberPastThreshold == 0) {
                EnviroSkyMgr.instance.ChangeWeather(ClearSkyPreset);
            }
            //Debug.Log("Reseting. " + windowIncrement.ToString() + ", numberPastTH: " + numberPastThreshold.ToString());
            numberPastThreshold = 0; 
        }
        if(currentAcceleration > previousAcceleration)
        {
            maxAcceleration = currentAcceleration;
        }
        else
        {
            if(maxAcceleration != 0)
            {
                if (maxAcceleration > Threshold)
                {
                    //Debug.Log("Past Threshold: " + numberPastThreshold.ToString());
                    numberPastThreshold += 1;
                }
            }
            else
            {
                maxAcceleration = 0; 
            }

        }
        previousAcceleration = currentAcceleration;
        presets = EnviroSkyMgr.instance.Weather.weatherPresets;       
        
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
    }
}


