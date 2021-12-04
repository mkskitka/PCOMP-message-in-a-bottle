using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Rotate : MonoBehaviour
{
    //private float Input;
    private float xInput;
    private float yInput;
    private float zInput;

    SerialPort sp = new SerialPort("COM4", 9600);

    //float rotationSpeed = 45;
    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    // Start is called before the first frame update


    void Awake()
    {
        
        sp.Open();
        StartCoroutine(ReadDataFromSerialPort());

        //rb = GetComponent<Rigidbody>();

    }

    IEnumerator ReadDataFromSerialPort()
        //only read when seri is avalible and same thing on arduino
    {
        while (true)
        {
            string[] values = sp.ReadLine().Split(',');
            xInput = float.Parse(values[0]);
            yInput = float.Parse(values[1]);
            //zInput = float.Parse(values[2]);
            yield return new WaitForSeconds(.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(xInput, 0, zInput);

        Debug.Log(transform.position);
    }
}
