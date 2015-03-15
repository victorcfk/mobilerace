using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]

public class CameraFollow : MonoBehaviour {

    public Transform followTarget;
    public float smoothTime = 0.1f;
    public float maxSpeed = Mathf.Infinity;

    public Vector3 currentVeloc;

    //private Vector3 currentPos;
    private Vector3 velocity = Vector3.zero;

    public float maxXClamp = 45;
    public float maxYClamp = 85;

	// Use this for initialization
	void Start () {
	   
	}

    void FixedUpdate()
    {
//        velocity = this.rigidbody.velocity;

        //followTarget.rigidbody.velocity;

        float intendedx = Mathf.Clamp(followTarget.transform.position.x + followTarget.transform.forward.x * 1, -maxXClamp, maxXClamp);
        float intendedy = Mathf.Clamp(followTarget.transform.position.y + followTarget.transform.forward.y * 1, -maxYClamp, maxYClamp);
        float intendedz = transform.position.z;

        Vector3 wantedPos = new Vector3(
            intendedx,
            intendedy,
            intendedz);

        this.transform.position = Vector3.SmoothDamp( transform.position, wantedPos, ref velocity,smoothTime);
    }
}
