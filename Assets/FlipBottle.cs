using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlipBottle : MonoBehaviour
{
    float total_rotation = 0;
    float total_y_movement = 0;
    float target_rotation = 180;
    float target_y_position = 2.5f;
    float rotation_speed = 1; 
    float y_position_speed = .03f; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Flip(true);
    }

    public void StopAnimation() {
        gameObject.GetComponent<Animator>().enabled = false;
    }
    public void PlayAnimation() {
        gameObject.GetComponent<Animator>().enabled = true;;
    }

    public bool Flip(bool down) {
        bool flipping = false;
        if(total_rotation < target_rotation) {
            flipping = true;
            if(down) {
                transform.Rotate(0f, rotation_speed, 0f, Space.Self);
            }
            else {
                transform.Rotate(0f, -rotation_speed, 0f, Space.Self);
            }
            total_rotation += rotation_speed;
        }
        if(total_y_movement < target_y_position) {
            flipping = true;
            if(down) {
                transform.Translate(0f, -y_position_speed, 0f, Space.World);
            }
            else {
                transform.Translate(0f, y_position_speed, 0f, Space.World);
            }
            total_y_movement += y_position_speed; 
        }
        if(!flipping) {
            total_y_movement = 0;
            total_rotation = 0;
        }
        return flipping; 
    }
}
