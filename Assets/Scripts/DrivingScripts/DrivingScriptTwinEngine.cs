using UnityEngine;
using System.Collections;

public class DrivingScriptTwinEngine : DrivingScriptBasic {

	public ApplyPhysics Left;
	public ApplyPhysics Right;
    public float rotationCorrectionVal = 3;

	Vector3 tempo = Vector3.forward;
	Vector3 velocityWeWant;

	void LateUpdate()
	{
			//We want the drifting to be constrained
			velocityWeWant = Vector3.SmoothDamp(rigidBody.velocity,

			                                    Mathf.Clamp(rigidBody.velocity.magnitude,MinSpeed,MaxSpeed) * transform.forward + 	//Apply the current velocity artificially towards the vehicle's transform
			                                    new Vector3(0,Mathf.Clamp(rigidBody.velocity.y,-10,-20),0),														//Add the current downward velocity due to gravity.

			                                    ref tempo,
			                                    Time.deltaTime*rotationCorrectionVal);


		//make the vehicle go towards the plane
		//=================================================
//		RaycastHit rch;
//		
//		Debug.DrawRay(transform.position,Vector3.down,Color.red,1);
//		
//		if(Physics.Raycast(transform.position,Vector3.down, out rch,5000))
//		{
////			transform.position = Vector3.MoveTowards(transform.position,
////			                                         new Vector3(transform.position.x, rch.point.y+5,transform.position.z),
////			                                         Time.deltaTime*5);
//
////			transform.up = Vector3.MoveTowards(transform.up,
////			                                   rch.normal,
////			                                   Time.deltaTime*5);
//
////			transform.rotation = Quaternion.RotateTowards(transform.rotation,
////			                                         Quaternion.AngleAxis(0,rch.normal),
////			                                         Time.deltaTime*5);
//		}
		//=================================================

	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		rigidBody.velocity = (velocityWeWant);
	}

}
