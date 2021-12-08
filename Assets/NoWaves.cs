using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoWaves : MonoBehaviour
{
    Shader waves;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject);
        //waves = Shader.Find("Waves");
        //this.GetComponent<Renderer>().material.SetBool("Enable", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(ChangeTimeMapped.inUpsidedown) {
            gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_WavesOn", 0f);
            gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 0f);
        }
        else {
            gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_WavesOn", 1f);
            gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_WaveSpeed", 1f);
            //gameObject.GetComponent<Renderer>().sharedMaterial.SetRange("_WaveHeight", Range(0.0, 0.5));
        }
        
    }
}
