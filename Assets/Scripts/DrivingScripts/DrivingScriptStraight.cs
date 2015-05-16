using UnityEngine;
using System.Collections;

public class DrivingScriptStraight : DrivingScriptBasic {

	public GameObject objToAccelerate;

	[HideInInspector, System.NonSerialized]
	public bool Accelerate;

	public float accVal = 75;

    public KeyCode LeftTurnCode;
	public KeyCode RightTurnCode;

	public float turnAngularVelocity;
    public float turnSensitivity = 1;

	public float rotationCorrectionVal = 3;

    public float LeftRightAcc { get; private set;}
	
    public Transform backLeft;
    public Transform backRight;
    public Transform frontLeft;
    public Transform frontRight;
    public LayerMask trackMask;

    bool isBraking;

	Vector3 tempo = Vector3.forward;

	// Update is called once per frame
	void Update () 
	{
		ControlUpdates();
	}

    void MaintainVehOrientation () {

        RaycastHit lr;
        RaycastHit rr;
        RaycastHit lf;
        RaycastHit rf;

        if(
            Physics.Raycast(backLeft.position + Vector3.up, Vector3.down, out lr,100,trackMask) &&
            Physics.Raycast(backRight.position + Vector3.up, Vector3.down, out rr,100,trackMask) &&
            Physics.Raycast(frontLeft.position + Vector3.up, Vector3.down, out lf,100,trackMask) &&
            Physics.Raycast(frontRight.position + Vector3.up, Vector3.down, out rf,100,trackMask)
            )
        {
            Vector3 upDir       = (lr.normal + rr.normal + lf.normal +rf.normal)/4;

            Vector3 leftFwd     = lf.point - lr.point;
            Vector3 rightFwd    = rf.point - rr.point;
            Vector3 avgFwd      = (leftFwd + rightFwd)/2;

            rigidBody.MovePosition(
                (lf.point + lr.point + rf.point +rr.point)/4 +Vector3.up
                );

            rigidBody.MoveRotation(
                    Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(avgFwd,upDir),Time.deltaTime*50)
                );

           

            Debug.DrawRay(rr.point, Vector3.up);
            Debug.DrawRay(lr.point, Vector3.up);
            Debug.DrawRay(lf.point, Vector3.up);
            Debug.DrawRay(rf.point, Vector3.up);
            Debug.DrawRay(transform.position,upDir,Color.magenta,1);
        }
    }

	void FixedUpdate()
	{
       

        rigidBody.angularVelocity = LeftRightAcc * turnSensitivity * turnAngularVelocity * transform.up * Time.deltaTime;

		if(isBraking)
		{
			//If the velocity is in any way orthognal to the vehicle's forward. So we are moving
			if(Vector3.Dot
			   (rigidBody.velocity,
			 transform.forward) > 0)
			{
				rigidBody.AddForceAtPosition(-transform.forward* accVal,transform.position);
			}

		}
		else
		{
			rigidBody.AddForceAtPosition(transform.forward* accVal,transform.position);
		}

		rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity,
		                                    Mathf.Clamp(rigidBody.velocity.magnitude,MinSpeed,MaxSpeed) * transform.forward,								//Add the current downward velocity due to gravity
		                                    ref tempo,
		                                    Time.fixedDeltaTime*rotationCorrectionVal);

        MaintainVehOrientation();
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
