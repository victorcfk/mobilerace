using UnityEngine;
using System.Collections;

public class ApplyPhysics : MonoBehaviour {

    public KeyCode FwdAccCode;

    public GameObject Track;
    public GameObject thingToAccelerate;

    [HideInInspector, System.NonSerialized]
    public bool Accelerate;

    public ParticleSystem psys;
	public float pSysMinEmission = 3;
	public float pSysMaxEmission = 20;

    public TrailRenderer trail;
	public float trailMinWidth = 0.05f;
	public float trailMaxWidth = 1;

    public float accVal = 75;
    public float normalizedVal = 0;

	public ApplyPhysics otherApplyPhysics;
	[Range (0,0.5f)]
	public float LeftAndRightInputStickiness = 0.025f;

	[HideInInspector, System.NonSerialized]
	public bool usingTouch;

//    public float rotaVal = 10f;
//	// Use this for initialization
	void Start () {
		normalizedVal =0;
		usingTouch = true;
	}
//	
	// Update is called once per frame
	void FixedUpdate () {

		if(otherApplyPhysics && Mathf.Abs(otherApplyPhysics.normalizedVal-normalizedVal)<LeftAndRightInputStickiness )
		{
			otherApplyPhysics.normalizedVal = normalizedVal = (otherApplyPhysics.normalizedVal+normalizedVal)/2;
		}

		if(Input.GetKey(FwdAccCode)) usingTouch = true;
		if(usingTouch)
		{
			if(Input.GetKey(FwdAccCode))	normalizedVal =1;
			else normalizedVal =0;
		}

		if(normalizedVal > 0 )//(Input.GetKey(FwdAccCode))
        {
			thingToAccelerate.rigidbody.AddForceAtPosition(Track.transform.forward* accVal * normalizedVal,Track.transform.position);
        }
        else
        {
            thingToAccelerate.rigidbody.AddForceAtPosition(Track.transform.forward*5,Track.transform.position);
        }

		if(psys)	psys.emissionRate = normalizedVal*(pSysMaxEmission - pSysMinEmission) + pSysMinEmission;
		
		if(trail)
		{
			trail.startWidth = normalizedVal*(trailMaxWidth - trailMinWidth) + trailMinWidth;
			trail.endWidth = normalizedVal*(trailMaxWidth - trailMinWidth) + trailMinWidth;
		}

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
