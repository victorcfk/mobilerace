﻿using UnityEngine;
using System.Collections;

public class ApplyPhysics : MonoBehaviour {

    public KeyCode FwdAccCode;

    public GameObject Track;

    public GameObject thingToAccelerate;

    [HideInInspector, System.NonSerialized]
    public bool Accelerate;

    public ParticleSystem psys;
    public TrailRenderer trail;

    public float accVal = 75;
    public float normalizedVal = 1;

//    public float rotaVal = 10f;
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
	// Update is called once per frame
	void FixedUpdate () {
	
        if(Accelerate || Input.GetKey(FwdAccCode))//(Input.GetKey(FwdAccCode))
        {
            if(Input.GetKey(FwdAccCode))
            {
                normalizedVal =1;
            }
            if(psys)
                psys.emissionRate = 15;

            if(trail)
            {
                trail.startWidth = 0.5f;
                trail.endWidth = 0.5f;
            }

            //Track.rigidbody.AddForceAtPosition(Track.transform.forward,Track.transform.position);
            //rigidbody.AddRelativeForce(Track.transform.forward*100,Track.transform.position);

            //thingToAccelerate.rigidbody.AddForceAtPosition(Track.transform.forward*0.1f,thingToAccelerate.transform.position,ForceMode.Impulse);
            thingToAccelerate.rigidbody.AddForceAtPosition(Track.transform.forward*accVal * normalizedVal,Track.transform.position);

//            thingToAccelerate.rigidbody.velocity = Quaternion.AngleAxis(rotaVal*Time.fixedDeltaTime,Vector3.up) *thingToAccelerate.rigidbody.velocity;
            //Track.rigidbody.AddForceAtPosition(Track.transform.forward,Track.transform.position);
        }
        else
        {
            if(psys)
                psys.emissionRate = 4;

            if(trail)
            {
                trail.startWidth = 0.1f;
                trail.endWidth = 0.1f;
            }

            thingToAccelerate.rigidbody.AddForceAtPosition(Track.transform.forward*5,Track.transform.position);
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
