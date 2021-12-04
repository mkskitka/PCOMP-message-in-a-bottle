using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsController : MonoBehaviour
{

    GameObject highlightedGameObject;
    bool haveHitObjectAlready = false;

    public float rayLength = 10f;

    void FixedUpdate()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        
        Debug.DrawRay(transform.position, fwd, Color.green);

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, fwd, out hitInfo, rayLength))
        {
            // if we have hit something this frame...
            Interactable interactionScript = hitInfo.transform.gameObject.GetComponent<Interactable>();
            // this is how we check if object is of type 'interactable' without having to set layers, tags, other confusing concepts...
            if (interactionScript)
            {
                if (haveHitObjectAlready)
                {
                    if (!(hitInfo.transform.gameObject.GetInstanceID() == highlightedGameObject.GetInstanceID()))
                    {
                        // our two objects are different:
                        // set old highlighted object to be inactive
                        highlightedGameObject.GetComponent<Interactable>().setInactive();
                        // then activate the new object
                        interactionScript.setActive(gameObject.transform);
                        // and set it to the current highlighted gameObject:
                        highlightedGameObject = hitInfo.transform.gameObject;
                    }
                }
                else
                {
                    // activate the new object
                    interactionScript.setActive(gameObject.transform);
                    // and set it to the current highlighted gameObject:
                    highlightedGameObject = hitInfo.transform.gameObject;
                    haveHitObjectAlready = true;
                }


            }
        }
        else
        {
            // if we haven't hit anything this frame...
            // set old highlighted object to be inactive
            if (haveHitObjectAlready)
            {
                highlightedGameObject.GetComponent<Interactable>().setInactive();
                haveHitObjectAlready = false;
            }
        }

    }


}
