using UnityEngine;
using System.Collections;

public class TeleportVehToStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            other.attachedRigidbody.MovePosition(GameManager.instance.vehStartTransform.position);
            other.attachedRigidbody.MoveRotation(GameManager.instance.vehStartTransform.rotation);

//            other.transform.forward = Vector3.zero;
//            other.attachedRigidbody.velocity = other.attachedRigidbody.velocity.magnitude * Vector3.forward;
        }
    }
}
