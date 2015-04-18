using UnityEngine;
using System.Collections;

public class DrivingScriptBasic : MonoBehaviour {

	public float MinSpeed = 10;
	public float MaxSpeed = 75;

	public Rigidbody accObjRigidBody;

	// Use this for initialization
	void Awake () {

		if(accObjRigidBody == null)
			accObjRigidBody = GetComponent<Rigidbody>();
	}
}
