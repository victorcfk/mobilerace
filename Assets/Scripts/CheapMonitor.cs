using UnityEngine;
using System.Collections;

public class CheapMonitor : MonoBehaviour {

    public ApplyPhysics Left;
    public ApplyPhysics Right;

    public float rotaVal = 3;

	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.Landscape;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

//        if(Left.Accelerate && !Right.Accelerate)
//            Left.thingToAccelerate.rigidbody.velocity = Quaternion.AngleAxis(rotaVal*Time.fixedDeltaTime,Vector3.up) * Left.thingToAccelerate.rigidbody.velocity;
//
//        if(Right.Accelerate && !Left.Accelerate)
//            Right.thingToAccelerate.rigidbody.velocity = Quaternion.AngleAxis(-rotaVal*Time.fixedDeltaTime,Vector3.up) * Right.thingToAccelerate.rigidbody.velocity;

        Vector3 tempo = Vector3.forward;

        rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity,rigidbody.velocity.magnitude * transform.forward,ref tempo,Time.deltaTime*rotaVal);
        //rigidbody.velocity = ;
	}
}
