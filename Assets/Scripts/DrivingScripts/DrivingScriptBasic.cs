using UnityEngine;
using System.Collections;

public class DrivingScriptBasic : MonoBehaviour {

	public float MinSpeed = 10;
	public float MaxSpeed = 75;

	public Rigidbody rigidBody;

	// Use this for initialization
	protected virtual void Awake () {

		if(rigidBody == null)
			rigidBody = GetComponent<Rigidbody>();
	}
}
