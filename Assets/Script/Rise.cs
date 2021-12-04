using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Rise : MonoBehaviour
{
    private float Input;
    //private float movementSpeed = 5f;
    SerialPort sp = new SerialPort("COM3", 9600);

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();
    }

    // Update is called once per frame
    void Update()
    {

                string value = sp.ReadLine();
                Input = float.Parse(value) / 10;

                //update the position
                //transform.position = transform.position + new Vector3(0, Input * Time.deltaTime * 5, 0);
                transform.position = new Vector3(0, Input, 0);

                //output to log the position change
                Debug.Log(transform.position);

               
    } 
}