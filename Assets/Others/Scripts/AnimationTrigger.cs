using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour {

	public Animator animationTrigger;

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("entered trigger " + this.gameObject.name);
		if (animationTrigger != null)
		{
			animationTrigger.enabled = true;
		}
		else
		{
			Debug.Log("You have not assigned an Animator component in the inspector!");
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Debug.Log("exit trigger" + this.gameObject.name);
		if (animationTrigger != null)
		{
			animationTrigger.enabled = false;
		}
	}
}
