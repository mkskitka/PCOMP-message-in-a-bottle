using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoTrigger : MonoBehaviour
{

	public VideoPlayer videoTrigger;

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("entered trigger " + this.gameObject.name);
		if (videoTrigger != null)
		{
			videoTrigger.Play();
		}
		else
		{
			Debug.Log("You have not assigned a video player in the inspector!");
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Debug.Log("exit trigger" + this.gameObject.name);
		if (videoTrigger != null)
		{
			videoTrigger.Stop();
		}
	}
}
