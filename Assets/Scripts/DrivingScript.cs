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

	void LateUpdate()
	{
		velocityWeWant = Vector3.SmoothDamp(GetComponent<Rigidbody>().velocity,Mathf.Clamp(GetComponent<Rigidbody>().velocity.magnitude,MinVelocity,MaxVelocity) * transform.forward,ref tempo,Time.deltaTime*rotaVal);
	}

	// Update is called once per frame
	void FixedUpdate () {

//        if(Left.Accelerate && !Right.Accelerate)
//            Left.thingToAccelerate.rigidbody.velocity = Quaternion.AngleAxis(rotaVal*Time.fixedDeltaTime,Vector3.up) * Left.thingToAccelerate.rigidbody.velocity;
//
//        if(Right.Accelerate && !Left.Accelerate)
//            Right.thingToAccelerate.rigidbody.velocity = Quaternion.AngleAxis(-rotaVal*Time.fixedDeltaTime,Vector3.up) * Right.thingToAccelerate.rigidbody.velocity;

		GetComponent<Rigidbody>().velocity = (velocityWeWant);
            //rigidbody.velocity = ;
	}
}
