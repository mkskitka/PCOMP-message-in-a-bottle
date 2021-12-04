using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour
{
	// Launches a projectile in 2 seconds

	public GameObject projectile;

	void Start()
	{
	}

	void Update()
	{
		if (Input.GetKeyDown("space")){

			//make an instance of the game object "projectile"
			GameObject thisProjectile = Instantiate (projectile); 

			//this instance of my projectile will be two units in front of my main camera
			thisProjectile.transform.position = transform.position + Camera.main.transform.forward * 2;

			//make a temporary variable to hold the rigidbody component of this instance of my projectile
			Rigidbody rb = thisProjectile.GetComponent<Rigidbody> (); 

			//add force to that in the forward direction of my camera. 
			rb.AddForce(Camera.main.transform.forward * 1000);

			//destroy after x amount of seconds after it was instantiated
			Destroy(thisProjectile, 2f); 
		}
	}
}