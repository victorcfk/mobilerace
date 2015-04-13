using UnityEngine;
using System.Collections;

public class DrivingScript : MonoBehaviour {

    public ApplyPhysics Left;
    public ApplyPhysics Right;

	Vector3 tempo = Vector3.forward;
	Vector3 velocityWeWant;

	public float MinVelocity = 10;
	public float MaxVelocity = 75;

    public float rotaVal = 3;

	public Rigidbody vehRigidbody;//dsad

	void Awake()
	{
		vehRigidbody = GetComponent<Rigidbody>();
	}

	void LateUpdate()
	{
		//We want the drifting to be constrained
		velocityWeWant = Vector3.SmoothDamp(vehRigidbody.velocity,

		                                    Mathf.Clamp(vehRigidbody.velocity.magnitude,MinVelocity,MaxVelocity) * transform.forward + 	//Apply the current velocity artificially towards the vehicle's transform
		                                    new Vector3(0,Mathf.Clamp(vehRigidbody.velocity.y,0,-10),0),														//Add the current downward velocity due to gravity.

		                                    ref tempo,
		                                    Time.deltaTime*rotaVal);

		/*
		 * Vector3 idealfwd = new Vector3(transform.forward.x,rigidbody.velocity.y,transform.forward.z).normalized;

			velocityWeWant = Vector3.SmoothDamp(rigidbody.velocity,Mathf.Clamp(rigidbody.velocity.magnitude,MinVelocity,MaxVelocity) * idealfwd,ref tempo,Time.deltaTime*rotaVal);
		*/

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
		vehRigidbody.velocity = (velocityWeWant);
	}


}
