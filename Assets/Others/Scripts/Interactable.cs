using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{

    [Header("Object To Activate")]

    public GameObject objectToActivate;



    // Rotation of the highlighted object
    [Header("Highlight Settings")]
    [ColorUsageAttribute(true, true)]
    public Color highlightColor = new Color(235, 255, 0, 0.5f);

    public bool shouldSpin = true;
    public float rotationSpeed = 0.5f;

    [Header("Pickup Settings")]

    public bool canPickUp = false;
    public float throwForce = 50f;

    [Range(0.01f, 1f)]
    public float followSpeed = 0.25f;


    // private variables:
    private Color unhighlightedColor;

    private bool isActive = false;
    private bool isPickedUp = false;
    private bool isObjectToActivateActive = false;

    private Material myMat;
    private Transform pickUpParent;

    private float distFromPickUpParent;
    private float lastPickUpDropTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!objectToActivate)
        {
            objectToActivate = new GameObject();
        }
        myMat = GetComponent<Renderer>().material;
        unhighlightedColor = myMat.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // on mouse click, drop item regardless of whether we are currently active:
            if (isPickedUp && Time.time - lastPickUpDropTime > 1f)
            {
                DropItem();
            }


            if (isActive)
            {

                // swap whether the objectToActivate will be activated or unactivated:
                isObjectToActivateActive = !isObjectToActivateActive;
                objectToActivate.SetActive(isObjectToActivateActive);

                if (canPickUp && Time.time - lastPickUpDropTime > 1f)
                {
                    PickItem();
                }
            }



        }

        // spin while highlighted
        if (isActive || isPickedUp)
        {
            if (shouldSpin)
            {
                transform.RotateAround(transform.position, new Vector3(0, 1, 0), rotationSpeed);
            }
        }

        moveTowards();



    }

    public void setActive(Transform parent)
    {
        isActive = true;
        pickUpParent = parent;
        // set styling
        myMat.color = highlightColor;
    }

    public void setInactive()
    {
        isActive = false;
        // reset styling
        myMat.color = unhighlightedColor;
    }


    // adapted from: https://www.patrykgalach.com/2020/03/16/pick-up-items-in-unity/
    private void PickItem()
    {
        Debug.Log("Picked up " + name);
        // lets keep track of last pick / drop time so we can do a check before pickup / drop
        lastPickUpDropTime = Time.time;
        isPickedUp = true;

        distFromPickUpParent = Vector3.Distance(transform.position, pickUpParent.position);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            // Disable rigidbody and reset velocities
            rb.isKinematic = true;
        }
    }

    private void moveTowards()
    {
        if (isPickedUp)
        {
            Vector3 desiredPosition = pickUpParent.position + pickUpParent.transform.forward * distFromPickUpParent;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed);
        }
    }

    private void DropItem()
    {
        Debug.Log("Dropped " + name);
        // lets keep track of last pick / drop time so we can do a check before pickup / drop
        lastPickUpDropTime = Time.time;
        // Remove parent
        isPickedUp = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            // Enable rigidbody
            rb.isKinematic = false;
            // Add force to throw item a little bit
            rb.AddForce(pickUpParent.transform.forward * throwForce, ForceMode.VelocityChange);
        }
    }



}
