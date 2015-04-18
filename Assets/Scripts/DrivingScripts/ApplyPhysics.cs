using UnityEngine;
using System.Collections;

public class ApplyPhysics : MonoBehaviour {

    public KeyCode FwdAccCode;
	public KeyCode BckAccCode;

    public GameObject Engine;
    public GameObject objToAccelerate;
	Rigidbody accObjRigidbody;

    [HideInInspector, System.NonSerialized]
    public bool Accelerate;

    public ParticleSystem psys;
	public float pSysMinEmission = 1;
	public float pSysMaxEmission = 50;

    public TrailRenderer trail;
	public float trailMinWidth = 0.01f;
	public float trailMaxWidth = 3;

    public float accVal = 75;
    public float normalizedVal = 0;

	public ApplyPhysics otherApplyPhysics;
	[Range (0,0.5f)]
	public float LeftAndRightInputStickiness = 0.025f;

	[HideInInspector, System.NonSerialized]
	public bool usingKey;



	void Awake()
	{
		if(Engine == null)	Engine = gameObject;
		if(accObjRigidbody == null)	accObjRigidbody = objToAccelerate.GetComponent<Rigidbody>();
	}

	//	Use this for initialization
	void Start () {
		normalizedVal =0;
		usingKey = true;
	}

	// Update is called once per frame
	void FixedUpdate () {

//		if(Input.anyKey) normalizedVal = Mathf.Clamp(normalizedVal + Time.deltaTime,0f,1f);	//0
//		else
//			normalizedVal = Mathf.Clamp(normalizedVal - Time.deltaTime,0f,1f);
        

		//Average out the input values.
		//===========================================================================================
		if(otherApplyPhysics && Mathf.Abs(otherApplyPhysics.normalizedVal-normalizedVal)<LeftAndRightInputStickiness )
		{
			otherApplyPhysics.normalizedVal = normalizedVal = (otherApplyPhysics.normalizedVal+normalizedVal)/2;
		}
		//===========================================================================================

		if(Input.GetKey(FwdAccCode)) usingKey = true;
		if(Input.GetKey(BckAccCode)) usingKey = true;

		if(usingKey)
		{
			if(Input.GetKey(FwdAccCode))	normalizedVal =1;
			else
			if(Input.GetKey(BckAccCode))	normalizedVal =0.2f;
			else
				normalizedVal =0;
		}

		if(normalizedVal > 0 )//(Input.GetKey(FwdAccCode))
        {
			accObjRigidbody.AddForceAtPosition(Engine.transform.forward* accVal * normalizedVal,Engine.transform.position);
        }
//		else
//		if(normalizedVal > 0 )//(Input.GetKey(FwdAccCode))
//		{
//
//			//If the velocity is in any way orthognal to the vehicle's forward.
//			if(Vector3.Dot
//			   (accObjRigidbody.velocity,
//			    transform.forward) > 0)
//			{
//				accObjRigidbody.AddForceAtPosition(-Track.transform.forward* accVal * 0.2f,Track.transform.position);
//			}
//		}
        else
        {
			//If the velocity is in any way orthognal to the vehicle's forward. So we are moving
						if(Vector3.Dot
						   (accObjRigidbody.velocity,
						    transform.forward) > 0)
						{
							accObjRigidbody.AddForceAtPosition(-Engine.transform.forward* accVal * 0.1f,Engine.transform.position);
						}
        }


		//Aesthetics
		//==========================================================================================================
		if(psys)	psys.emissionRate = normalizedVal*(pSysMaxEmission - pSysMinEmission) + pSysMinEmission;
		
		if(trail)
		{
			trail.startWidth = normalizedVal*(trailMaxWidth - trailMinWidth) + trailMinWidth;
			trail.endWidth = normalizedVal*(trailMaxWidth - trailMinWidth) + trailMinWidth;
		}
		//==========================================================================================================

//        if(Input.GetKey(BckAccCode))
//        {
//            //Track.rigidbody.AddForceAtPosition(Track.transform.forward,Track.transform.position);
//            
//            //rigidbody.AddRelativeForce(Track.transform.forward*100,Track.transform.position);
//            
//            rigidbody.AddForceAtPosition(-Track.transform.forward*10,Track.transform.position);
//            //Track.rigidbody.AddForceAtPosition(Track.transform.forward,Track.transform.position);
//        }
	}
}
