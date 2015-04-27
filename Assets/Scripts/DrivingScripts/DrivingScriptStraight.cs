using UnityEngine;
using System.Collections;

public class DrivingScriptStraight : DrivingScriptBasic {

	public GameObject objToAccelerate;

	[HideInInspector, System.NonSerialized]
	public bool Accelerate;

	public float accVal = 75;
	public float normalizedVal = 1;

	public KeyCode LeftTurnCode;
	public KeyCode RightTurnCode;

	public float turnAngularVelocity;

	public float rotationCorrectionVal = 3;



    public float LeftRightAcc { get; private set;}
	bool isBraking;

	Vector3 tempo = Vector3.forward;

	protected override void Awake () {
		
		base.Awake();
	}

	// Update is called once per frame
	void Update () 
	{
		ControlUpdates();
	}

	void FixedUpdate()
	{
		accObjRigidBody.angularVelocity = LeftRightAcc * transform.up * turnAngularVelocity * Time.deltaTime;

		if(isBraking)
		{
			//If the velocity is in any way orthognal to the vehicle's forward. So we are moving
			if(Vector3.Dot
			   (accObjRigidBody.velocity,
			 transform.forward) > 0)
			{
				accObjRigidBody.AddForceAtPosition(-accObjRigidBody.transform.forward* accVal,accObjRigidBody.transform.position);
			}

		}
		else
		{
			accObjRigidBody.AddForceAtPosition(accObjRigidBody.transform.forward* accVal * normalizedVal,accObjRigidBody.transform.position);
		}

		accObjRigidBody.velocity = Vector3.SmoothDamp(accObjRigidBody.velocity,
		                                    
		                                    Mathf.Clamp(accObjRigidBody.velocity.magnitude,MinSpeed,MaxSpeed) * transform.forward + 	//Apply the current velocity artificially towards the vehicle's transform
		                                    new Vector3(0,Mathf.Clamp(accObjRigidBody.velocity.y,-40,-40),0),														//Add the current downward velocity due to gravity.
		                                    
		                                    ref tempo,
		                                    Time.fixedDeltaTime*rotationCorrectionVal);

	}

	void ControlUpdates()
	{
	
		LeftRightAcc = Input.acceleration.x;

		if(Input.GetKey(RightTurnCode)) 
			LeftRightAcc = 1;
		else
			if(Input.GetKey(LeftTurnCode)) 
				LeftRightAcc = -1;
		else
		{
			isBraking = Input.anyKey;
		}
	}
}
