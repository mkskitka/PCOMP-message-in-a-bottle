using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    private float speed = .005f;
    private float up = .001f;
    private float left = .001f;
    bool init = false;
    public CameraPan camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraPan>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ChangeTimeMapped.inUpsidedown && !ChangeTimeMapped.flippingBottle) {
            if(!init) {
                var pos = camera.transform.position;
                transform.position = pos; 
                transform.Translate(1f, 0f, 3f, Space.World);
                init = true; 
            }
            transform.Translate(-left, -up, -speed, Space.World);
        }
        else {
            init = false; 
        }
    }
}
