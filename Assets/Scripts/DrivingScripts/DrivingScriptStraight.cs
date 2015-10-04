using UnityEngine;
using System.Collections;

public class DrivingScriptStraight : DrivingScriptBasic {

	public GameObject objToAccelerate;

	public float initAccVal = 75;
    float accVal;

    [Range(0,1)]
    public float LeftRightAcc;
    public bool isBraking;

	public float turnAngularVelocity;

	public float rotationCorrectionVal = 3;
    	
    public Transform backLeft;
    public Transform backRight;
    public Transform frontLeft;
    public Transform frontRight;
    public LayerMask trackMask;

    static float MaxDownwardCastDist = 50;
    static float DistFromGround = 3;
    static float RotationCorrectionRate = 120;
    static float PositionCorrectionRate = 80;

    public ParticleSystem CollisionPsys;

    float DisableAccTimer;
    Vector3 tempo = Vector3.forward;

    public bool isSpeedBoosted
    {
        get
        {
            return remainingSpeedBoostDuration >0;
        }
    }
    float remainingSpeedBoostDuration;

    float initMinSpeed;
    float initMaxSpeed;

    void OnValidate()
    {
        LeftRightAcc = Mathf.Clamp(LeftRightAcc,0, 1);
    }

    protected override void Awake()
    {
        base.Awake();
        accVal = initAccVal;

        initMinSpeed = MinSpeed;
        initMaxSpeed = MaxSpeed;
    }

	// Update is called once per frame
	void Update () 
	{
        if(DisableAccTimer > 0)
        {
            rigidBody.drag =1;
            DisableAccTimer -= Time.deltaTime;
        }
        else
        {
            rigidBody.drag =0;
        }

        remainingSpeedBoostDuration = Mathf.Clamp(remainingSpeedBoostDuration - Time.deltaTime, 0, Mathf.Infinity);
	}

    public void GainSpeedBoost()
    {
        remainingSpeedBoostDuration += 1.5f;
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

        if (isSpeedBoosted)
        {
            MaxSpeed = initMaxSpeed * 2;
            MinSpeed = MaxSpeed;
        } 
        else
        {
            MaxSpeed = initMaxSpeed;
            MinSpeed = initMinSpeed;
        }

        rigidBody.AddForceAtPosition(transform.forward*accVal,transform.position);
        
        rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity,
                                                Mathf.Clamp(rigidBody.velocity.magnitude,MinSpeed,MaxSpeed) * transform.forward,                                //Add the current downward velocity due to gravity
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
