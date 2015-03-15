using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {

	public float moveSpeed;
	public Vector3 moveDirection;
	
	// Use this for initialization
	void Awake () 
	{
		if (moveDirection == Vector3.zero) 
		{
			moveDirection = Vector3.up;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void StartMoving()
	{
		transform.Translate(
			Time.deltaTime * moveSpeed * moveDirection);
	}
}
