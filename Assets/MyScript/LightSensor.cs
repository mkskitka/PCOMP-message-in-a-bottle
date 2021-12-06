using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class LightSensor : MonoBehaviour
{
    private float Input;

    Light myLight;

    //private float movementSpeed = 5f;
    SerialPort sp = new SerialPort("COM6", 9600);

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();

        myLight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

        string value = sp.ReadLine();
        Input = float.Parse(value);

        //update the position
        //transform.position = transform.position + new Vector3(0, Input * Time.deltaTime * 5, 0);
        //transform.position = new Vector3(0, Input, 0);
        myLight.intensity = Mathf.PingPong(Time.time, Input);

        //output to log the position change
        Debug.Log(myLight.intensity);


    }
}
