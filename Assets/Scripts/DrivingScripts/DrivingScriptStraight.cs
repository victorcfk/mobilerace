using UnityEngine;
using System.Collections;

public class DrivingScriptStraight : DrivingScriptBasic {

	public GameObject objToAccelerate;

	public float initAccVal = 75;
    float accVal;

    public KeyCode LeftTurnCode;
	public KeyCode RightTurnCode;

	public float turnAngularVelocity;
    public float turnSensitivity = 1;

	public float rotationCorrectionVal = 3;
    	
    public Transform backLeft;
    public Transform backRight;
    public Transform frontLeft;
    public Transform frontRight;
    public LayerMask trackMask;

    //controls
    //============================

    public float LeftRightAcc { get; private set;}
    public float LeftPower;
    public float RightPower;

    //============================

    public ParticleSystem CollisionPsys;

    float DisableAccTimer;
    bool isBraking;
    Vector3 tempo = Vector3.forward;

    protected override void Awake()
    {
        base.Awake();
        accVal = initAccVal;
    }
	// Update is called once per frame
	void Update () 
	{
        if(DisableAccTimer > 0)
        {
            accVal = -initAccVal;
            DisableAccTimer -= Time.deltaTime;
        }
        else
        {
            accVal = initAccVal;
        }

        if(GameManager.instance.controlScheme == ControlSchemes.TILT)
        {
		    TiltControlUpdates();
        }
        else
        {
            LeftRightSlideControlUpdates();
        }
	}

    void MaintainVehOrientation () {

        RaycastHit lr;
        RaycastHit rr;
        RaycastHit lf;
        RaycastHit rf;

        if(
            Physics.Raycast(backLeft.position + Vector3.up, Vector3.down, out lr,30,trackMask) &&
            Physics.Raycast(backRight.position + Vector3.up, Vector3.down, out rr,30,trackMask) &&
            Physics.Raycast(frontLeft.position + Vector3.up, Vector3.down, out lf,30,trackMask) &&
            Physics.Raycast(frontRight.position + Vector3.up, Vector3.down, out rf,30,trackMask)
            )
        {
            Vector3 upDir       = (lr.normal + rr.normal + lf.normal +rf.normal)/4;

            Vector3 leftFwd     = lf.point - lr.point;
            Vector3 rightFwd    = rf.point - rr.point;
            Vector3 avgFwd      = (leftFwd + rightFwd)/2;

            rigidBody.MovePosition(
                Vector3.MoveTowards(transform.position,(lf.point + lr.point + rf.point +rr.point)/4 + Vector3.up*4,Time.deltaTime*60)
                );

            rigidBody.MoveRotation(
                    Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(avgFwd,upDir),Time.deltaTime*60)
                );

            Debug.DrawRay(rr.point, Vector3.up);
            Debug.DrawRay(lr.point, Vector3.up);
            Debug.DrawRay(lf.point, Vector3.up);
            Debug.DrawRay(rf.point, Vector3.up);
            Debug.DrawRay(transform.position,upDir,Color.magenta,1);

            rigidBody.useGravity= false;
        }
        else
            rigidBody.useGravity= true;
    }

	void FixedUpdate()
	{
        rigidBody.angularVelocity = LeftRightAcc * turnSensitivity * turnAngularVelocity * transform.up * Time.deltaTime;

//		if(isBraking)
//		{
//			//If the velocity is in any way orthognal to the vehicle's forward. So we are moving
//			if(Vector3.Dot
//			   (rigidBody.velocity,
//			 transform.forward) > 0)
//			{
//                rigidBody.AddForceAtPosition(-transform.forward*accVal,transform.position);
//			}
//
//		}
//		else
		{
            rigidBody.AddForceAtPosition(transform.forward*accVal,transform.position);
		}

		rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity,
		                                    Mathf.Clamp(rigidBody.velocity.magnitude,MinSpeed,MaxSpeed) * transform.forward,								//Add the current downward velocity due to gravity
		                                    ref tempo,
		                                    Time.fixedDeltaTime*rotationCorrectionVal);

        MaintainVehOrientation();
	}

    void OnCollisionEnter()
    {
        CollisionPsys.Play();

        DisableAccTimer = Mathf.Clamp(DisableAccTimer+0.5f,0,0.5f);
    }

	void TiltControlUpdates()
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

    void LeftRightSlideControlUpdates()
    {
        LeftRightAcc = Mathf.Clamp( LeftPower- RightPower,-1,1);
    }

    void LeftRightTapControlUpdates()
    {
        LeftRightAcc = Mathf.Clamp(RightPower - LeftPower,-1,1);
    }
}
