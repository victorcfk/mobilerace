using UnityEngine;
using System.Collections;

public class DrivingScriptAI : MonoBehaviour {

	public GameObject Vehicle;

	int i= 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(i < GameManager.instance.track.numberOfPoints-1)
		{
			if(Vector3.SqrMagnitude(Vehicle.transform.position - (GameManager.instance.track.points[i].position+ Vector3.up*12)) < 200)
			{
				i++;
			}

			Vehicle.transform.forward = 
				Vector3.RotateTowards(transform.forward,GameManager.instance.track.points[i].position-transform.position + Vector3.up*12,Mathf.PI* Time.deltaTime,100);

			//Vehicle.transform.LookAt(GameManager.instance.track.points[i].position + Vector3.up*12);
		}
	}
}
