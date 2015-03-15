using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	static GameManager _instance;

	//This is the public reference that other classes will use
	public static GameManager instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<GameManager>();
			return _instance;
		}
	}

	public DrivingScript TheVehicle;
	public SmoothFollowCS CamFollow;

	public GameObject CamFollowObject;

	[Range (1,20)]
	public float MinFollowDistance;
	[Range (1,20)]
	public float MaxFollowDistance;

	[Range (1,20)]
	public float MinFollowHeight;
	[Range (1,20)]
	public float MaxFollowHeight;

	void Awake()
	{
		_instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {

		float t = (TheVehicle.rigidbody.velocity.sqrMagnitude - TheVehicle.MinVelocity*TheVehicle.MinVelocity) /  (TheVehicle.MaxVelocity*TheVehicle.MaxVelocity - TheVehicle.MinVelocity*TheVehicle.MinVelocity);

		CamFollow.distance =	Mathf.Lerp(MinFollowDistance, MaxFollowDistance, t);
		CamFollow.height   = 	Mathf.Lerp(MinFollowHeight, MaxFollowHeight, t);
		//CamFollow.distance = Mathf.Clamp( TheVehicle.rigidbody.velocity.magnitude , MinFollowDistance, MaxFollowDistance);
		//CamFollow.height   = Mathf.Clamp( TheVehicle.rigidbody.velocity.magnitude , MinFollowHeight, MaxFollowHeight);
//		if( TheVehicle.rigidbody.velocity > 0 )
//		{
//
//				MaxFollowDistance
//		}
	}
}
