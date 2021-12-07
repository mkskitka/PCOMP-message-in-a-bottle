using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffect : MonoBehaviour
{
    private Vector3 scaleChange, positionChange;
    // Start is called before the first frame update
    void Start()
    {
        scaleChange = new Vector3(10f, 0f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localScale += scaleChange;
    }
}
