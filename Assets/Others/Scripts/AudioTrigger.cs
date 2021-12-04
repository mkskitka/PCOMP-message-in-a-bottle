using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioTrigger : MonoBehaviour
{

	public AudioSource audioTrigger;

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("entered trigger " + this.gameObject.name);
		if (audioTrigger != null && other.tag == "Player")
		{
			//use this to start with no fade
			//audioTrigger.Play();

			//use this to fade in
			StartCoroutine(AudioFader.FadeIn(audioTrigger, 2f));

			


		}
		else
		{
			Debug.Log("You have not assigned an audio source in the inspector!");
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Debug.Log("exit trigger" + this.gameObject.name);
		if (audioTrigger != null)
		{
			//use this to pause with no fade
			//audioTrigger.Pause();

			

			//use this to fade out
			//StopAllCoroutines();
			StartCoroutine(AudioFader.FadeOut(audioTrigger, 2f));


		}
	}

}
