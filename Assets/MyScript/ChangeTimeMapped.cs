//this code is not working as we want because...

using System.Collections;
//using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine;
using System.Threading;
using StylizedWater2;
using System.Reflection;
[RequireComponent(typeof(FlipBottle))]
[RequireComponent(typeof(CameraPan))]


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
    public FlipBottle bottle; 
    public CameraPan camera; 

    
    // Inputs from Arduino
    public static float xInput =0;
    public static float yInput =0;
    public static float zInput=0;
    public static float AccX = 0; 
    public static float AccY = 0; 
    public static float AccZ = 0; 

    private static float firstZInput = -1;
    private static float initialYDegrees = 174;
    private static float RawMinutes = 0;

    // Presets 
    List<EnviroWeatherPreset> presets;
    EnviroWeatherPreset ClearSkyPreset;
    EnviroWeatherPreset HeavyRainPreset;
    EnviroWeatherPreset CloudyPreset;
    EnviroWeatherPreset HeavySnowPreset;
    private string CLEAR_SKY = "Clear Sky";
    private string HEAVY_RAIN = "Heavy Rain";
    private string CLOUDY = "Cloudy";
    private string HEAVY_SNOW = "Heavy Snow";
    private string CurrentPreset;

    // Rocking Detection Variables 
    private float timeFrame = 5;
    private int windowIncrement = 0;
    private float Threshold = 100;
    private float rockingPitchThreshold = 50;

    private float previousAcceleration = 0;
    private float currentAcceleration = 0;
    private float maxAcceleration = 0;
    private int numberPastThreshold = 0;

    // Pitch Rocking 
    private float previousPitchAcceleration = 0;
    private float maxPitchAcceleration = 0;
    private int numberPastThresholdPitch = 0;

    // Row Rocking 
    private float previousRowAcceleration = 0;
    private float maxRowAcceleration = 0;
    private int numberPastThresholdRow = 0;
    
    // Time Variables 
    private float maxHeadingAcc = 0;
    private bool prevTimeDir = true; 

    // Serial Info
    private SerialPort stream;
    private Thread thread;
    private Queue outputQueue;    // From Unity to Arduino
    private Queue inputQueue;    // From Arduino to Unity
    private int baudRate = 9600;
    private string portName = "/dev/tty.usbmodem14301";     // WINDOWS change to COM4 
    private int timeout = 100;
    private bool loop = true;
    private GameObject ocean; 

    public static bool flippingBottle = false; 
    private bool flippingCamera = false; 
    public static bool inUpsidedown = false; 
    private Material material;

    void Start()
    {
         StartThread();
        ocean = GameObject.FindGameObjectWithTag("ocean");
        WaterObject water = WaterObject.Find(transform.position, false);
        material = ocean.GetComponent<Renderer>().sharedMaterial;
        material.SetColor("_BaseColor", Color.black);
        material.SetFloatArray("_WaveHeight", new float[] { 1f,2f});
        //material.SetFloat("_Waven", 0f);
        // foreach(var key in material.shaderKeywords) {
        //     Debug.Log(key);
        // }
        // Shader oceanShader = material.shader;

        // Type type = water.GetType();
        // PropertyInfo[] props = type.GetProperties();
        // string str = "{";
        // foreach (var prop in props)
        // {
        // str+= prop.Name+",";
        // }
        // Debug.Log(str);
        // Debug.Log(oceanShader);
        // foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(oceanShader))
        // {
        //     string name = descriptor.Name;
        //     object value = descriptor.GetValue(oceanShader);
        //     Console.WriteLine("{0}={1}", name, value);
        // }
        // {shader:Universal Render Pipeline/FX/Stylized Water 2 (UnityEngine.Shader),
        // color:RGBA(0.000, 0.000, 0.000, 0.000),
        // mainTexture:,
        // mainTextureOffset:(0.0, 0.0),
        // mainTextureScale:(1.0, 1.0),
        // renderQueue:2000,
        // globalIlluminationFlags:None,
        // doubleSidedGI:False,
        // enableInstancing:True,
        // passCount:1,
        // shaderKeywords:System.String[],
        // name:StylizedWater2_Ocean,
        // hideFlags:None
        // }

        CurrentPreset = CLEAR_SKY;
        // Get a list of serial port names
        string[] ports = SerialPort.GetPortNames();
        bottle = GameObject.FindGameObjectWithTag("TagBottle").GetComponent<FlipBottle>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraPan>();
 
        // Display each port name to the console
        // foreach (string port in ports) {
        //     Debug.Log(port);
        // }

    }

    // Update is called once per frame
    void Update()
    {
        var currentWindow = Mathf.Floor(Time.realtimeSinceStartup / timeFrame);
        string input = ReadFromArduino();

        parseInput(input);
        SendToArduino("ready");
        changeTime();
        checkForNewPresets();

        detectRocking(  ref previousAcceleration, 
                        ref currentAcceleration, 
                        ref maxAcceleration, 
                        ref numberPastThreshold);

        detectRocking(  ref previousPitchAcceleration, 
                        ref AccX, 
                        ref maxPitchAcceleration, 
                        ref numberPastThresholdPitch);

        detectRocking(  ref previousRowAcceleration, 
                        ref AccZ, 
                        ref maxRowAcceleration, 
                        ref numberPastThresholdRow);

        if(currentWindow > windowIncrement) {
            Debug.Log("changing ocean color.");
            //Debug.Log(ocean.GetComponent<Renderer>().sharedMaterial._BaseColor.ToString());
            changeWeather();
            checkForRocking();
            numberPastThreshold = 0;  
            numberPastThresholdPitch = 0;  
            numberPastThresholdRow = 0; 
            Debug.Log(windowIncrement.ToString());
            material.SetFloatArray("_WaveHeight", new float[] { (float)windowIncrement,(float)windowIncrement} );    
        }
        if(flippingBottle) {
            flippingBottle = bottle.Flip(inUpsidedown);  
            if(!flippingBottle && !flippingCamera) {
                if(!inUpsidedown) {
                    bottle.PlayAnimation(); 
                    camera.PlayAnimation();
                }
                flippingBottle = false;
            }
        }
        if(flippingCamera) {
            flippingCamera = camera.Flip(inUpsidedown);
            if(!flippingCamera && !flippingBottle) {
                if(!inUpsidedown) {
                    bottle.PlayAnimation(); 
                    camera.PlayAnimation();
                }
                flippingCamera = false;
            }  
        }
    }

    void checkForRocking() {
        Debug.Log("number past threshold: " + numberPastThresholdPitch.ToString());
        if(numberPastThreshold < 10) {
            Debug.Log("wave speed - .5");
            ocean.GetComponent<Renderer>().sharedMaterial.SetColor("_BaseColor", Color.black);
            ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", .5f);
            //material.SetFloatArray("_WaveHeight", new float[] { 1f,2f});
        }
        if(numberPastThreshold > 10 && numberPastThreshold <= 20 ) {
            Debug.Log("wave speed - 1.5");
            ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 1.5f);
            //material.SetFloatArray("_WaveHeight", new float[] { 1f,4f});
        }
        if(numberPastThreshold > 20 && numberPastThreshold <= 30 ) {
            Debug.Log("wave speed - 2");
            ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 2.0f);
            //material.SetFloatArray("_WaveHeight", new float[] { 1f,5f});
        }
        if(numberPastThreshold > 30 && numberPastThreshold <= 40 ) {
            Debug.Log("wave speed - 3");
            ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 3.0f);
            //material.SetFloatArray("_WaveHeight", new float[] { 1f,6f});
        }
        if(numberPastThreshold > 40 && numberPastThreshold <= 50 ) {
            Debug.Log("wave speed - 5");
            ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 5.0f);
            //material.SetFloatArray("_WaveHeight", new float[] { 1f,7f});
        }


        if(numberPastThresholdPitch > rockingPitchThreshold 
            && numberPastThresholdRow < rockingPitchThreshold 
            && !flippingBottle 
            && !flippingCamera) {

            bottle.StopAnimation();
            camera.StopAnimation();
            flippingBottle = true; 
            flippingCamera = true;
            inUpsidedown = !inUpsidedown;
            Debug.Log("Bottle Rocking: Pitch");
        }
        if(numberPastThresholdRow > rockingPitchThreshold 
            && numberPastThresholdPitch < rockingPitchThreshold
            && !flippingBottle 
            && !flippingCamera) {

            bottle.StopAnimation();
            camera.StopAnimation();
            flippingBottle = true; 
            flippingCamera = true;
            inUpsidedown = !inUpsidedown;
            Debug.Log("Bottle Rocking: Row");
        }
    }

    /*******************************************************************************
     *
     *                 Thread Safe Serial Communication API
     *
     ******************************************************************************/

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

    /*******************************************************************************
     *
     *                 HandShaking/ Serial Communication (w/ Arduino)
     *
     ******************************************************************************/

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

    /*******************************************************************************
     *
     *                              Data Processing
     *
     ******************************************************************************/

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
                //("Y Acc: " + AccY.ToString());
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
            //Debug.Log("Input: " + input);
        }
    }

    void checkForNewPresets() {
        presets = EnviroSkyMgr.instance.Weather.weatherPresets;  
        if (presets.Count > 0 && ClearSkyPreset == null)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                //Debug.Log("name: " + presets[i].Name);

                if (presets[i].Name == CLEAR_SKY)
                {
                    ClearSkyPreset = presets[i];
                }
                if (presets[i].Name == HEAVY_SNOW)
                {
                    HeavySnowPreset = presets[i];
                }
                if (presets[i].Name == CLOUDY)
                {
                    CloudyPreset = presets[i];
                }
                if (presets[i].Name == HEAVY_RAIN)
                {
                    HeavyRainPreset = presets[i];
                }
            }
        }
    }


    /*******************************************************************************
     *
     *                              Environment Effects 
     *
     ******************************************************************************/


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



    void detectRocking(ref float prevAcc, ref float currentAcc, ref float maxAcc, ref int numRocks) {
        if(currentAcc > prevAcc)
        {
            maxAcc = currentAcc;
        }
        else
        {
            if(maxAcc != 0)
            {
                if (maxAcc > Threshold)
                {
                    numRocks += 1;
                }
            }
            else
            {
                maxAcc = 0; 
            }

        }
        prevAcc = currentAcc;  
    }

    void changeWeather(){
        
        //Debug.Log("current window: " + currentWindow.ToString() + " : " + windowIncrement.ToString());
       // Debug.Log(Time.realtimeSinceStartup.ToString());

            
        windowIncrement += 1;
        // use Number past Threshold to change the weather and wave height. 
        // if(numberPastThreshold > 200)
        // {
        //     EnviroSkyMgr.instance.ChangeWeather(HeavySnowPreset);
        //     ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 3f);
        // }
        if(numberPastThreshold > 100) {
            //Cloudy or something
            EnviroSkyMgr.instance.ChangeWeather(HeavyRainPreset);
            //ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 2f);
        }
        else if(numberPastThreshold <=100 && numberPastThreshold > 50) {
            EnviroSkyMgr.instance.ChangeWeather(CloudyPreset);
            //ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 1.5f);
            //something else
        }
        else if(numberPastThreshold == 0) {
            EnviroSkyMgr.instance.ChangeWeather(ClearSkyPreset);
            ocean.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 1f);
        }
        // Debug.Log("Reseting. " + windowIncrement.ToString() + ", numberPastTH: " + numberPastThreshold.ToString());
        // Debug.Log("numberPastTH Pitch: " + numberPastThresholdPitch.ToString());
        // Debug.Log("numberPastTH Row: " + numberPastThresholdRow.ToString());
 
    }
}


