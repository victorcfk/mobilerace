using UnityEngine;
using System.Collections;

public class DrivingScriptStraight : DrivingScriptBasic {

	public GameObject objToAccelerate;

	public float initAccVal = 75;
    float accVal;
    
    public float LeftRightAcc;
    public bool isBraking;

	public float turnAngularVelocity;

	public float rotationCorrectionVal = 3;
    	
    public Transform backLeft;
    public Transform backRight;
    public Transform frontLeft;
    public Transform frontRight;
    public LayerMask trackMask;

    //controls
    //============================

    //============================

    static float MaxDownwardCastDist = 50;
    static float DistFromGround = 3;
    static float RotationCorrectionRate = 60;
    static float PositionCorrectionRate = 80;

    public ParticleSystem CollisionPsys;

    float DisableAccTimer;
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
            accVal = 0;
            DisableAccTimer -= Time.deltaTime;
        }
        else
        {
            accVal = initAccVal;
        }


//        switch(GameManager.instance.controlScheme)
//        {
//            case ControlSchemes.TILT:
//                TiltControlUpdates();
//                break;
//            case ControlSchemes.SLIDER:
//                LeftRightSlideControlUpdates();
//                break;
//            case ControlSchemes.BUTTON:
//                LeftRightTapControlUpdates();
//                break;
//
//        }
	}

    void MaintainVehOrientation () {

        RaycastHit lr;
        RaycastHit rr;
        RaycastHit lf;
        RaycastHit rf;

        if(
            Physics.Raycast(backLeft.position ,     backLeft.forward,   out lr,MaxDownwardCastDist,trackMask) &&
            Physics.Raycast(backRight.position,     backRight.forward,  out rr,MaxDownwardCastDist,trackMask) &&
            Physics.Raycast(frontLeft.position,     frontLeft.forward,  out lf,MaxDownwardCastDist,trackMask) &&
            Physics.Raycast(frontRight.position,    frontRight.forward, out rf,MaxDownwardCastDist,trackMask)
            )
        {
            Vector3 upDir       = (lr.normal + rr.normal + lf.normal +rf.normal)/4;

            Vector3 leftFwd     = lf.point - lr.point;
            Vector3 rightFwd    = rf.point - rr.point;
            Vector3 avgFwd      = (leftFwd + rightFwd)/2;

            rigidBody.MovePosition(
                Vector3.MoveTowards(transform.position,
                                (lf.point + lr.point + rf.point +rr.point)/4 + upDir*DistFromGround,
                                Time.deltaTime*PositionCorrectionRate)
                );

            rigidBody.MoveRotation(
                Quaternion.RotateTowards(transform.rotation,
                                     Quaternion.LookRotation(avgFwd,upDir),
                                     Time.deltaTime*RotationCorrectionRate)
                );

            Debug.DrawLine(lr.point, backLeft.position);
            Debug.DrawLine(rr.point, backRight.position);

            Debug.DrawLine(lf.point, frontLeft.position);
            Debug.DrawLine(rf.point, frontRight.position);

            Debug.DrawRay(transform.position,upDir,Color.magenta,1);

            rigidBody.useGravity= false;
        }
        else
            rigidBody.useGravity= true;
    }

	void FixedUpdate()
	{
        rigidBody.angularVelocity = LeftRightAcc * turnAngularVelocity * transform.up * Time.deltaTime;

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

    void OnCollisionStay()
    {
        CollisionPsys.Play();
        
        DisableAccTimer = Mathf.Clamp(DisableAccTimer+0.5f,0,0.5f);
    }

}
