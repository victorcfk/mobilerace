using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

	static GameManager _instance;

	//This is the public reference that other classes will use
	public static GameManager instance {
		get {
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<GameManager> ();
			return _instance;
		}
	}

	public DrivingScript TheVehicle;
	public SmoothFollowCS CamFollow;

	public GameObject CamFollowObject;

	[Range (1,20)]
	public float
		MinFollowDistance;
	[Range (1,20)]
	public float
		MaxFollowDistance;

	[Range (1,20)]
	public float
		MinFollowHeight;
	[Range (1,20)]
	public float
		MaxFollowHeight;

//	public BezierCurve b;
	public Vector3 startPoint;

	public List<GameObject> listOfPieces;

	public GameObject Ground;

	public GameObject LatestPath;
	public GameObject EarliestPath;

	void Awake ()
	{
		_instance = this;

		Screen.orientation = ScreenOrientation.Landscape;

	}
        
	// Update is called once per frame
	void LateUpdate ()
	{
		int trackPointCount = 51;
		
		for(int i =0; i <trackPointCount; i++)
		{
			
			if(i < trackPointCount/4)
			{
				Debug.DrawRay(new Vector3(i*20,0,0),Vector3.up*10,Color.green,20);
			}
			else
				if(i >= trackPointCount/4 && i< trackPointCount/2)
			{
				int goingup = i - trackPointCount/4; 
				Debug.DrawRay(new Vector3(trackPointCount/4* 20,0,goingup*20),Vector3.up*10,Color.green,20);
			}
			else
				if(i >= trackPointCount/2 && i< 3*trackPointCount/4)
			{
				int goingup = 3*trackPointCount/4 - i-1; 
				Debug.DrawRay(new Vector3(goingup*20,0,trackPointCount/4*20),Vector3.up*10,Color.green,20);
			}
			else
			{
				int goingup = trackPointCount - i-1; 
				Debug.DrawRay(new Vector3(0,0,goingup*20),Vector3.up*10,Color.green,20);
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Space)) {
			Application.LoadLevel (0);
		}

		float t = (TheVehicle.GetComponent<Rigidbody> ().velocity.sqrMagnitude - TheVehicle.MinVelocity * TheVehicle.MinVelocity) / (TheVehicle.MaxVelocity * TheVehicle.MaxVelocity - TheVehicle.MinVelocity * TheVehicle.MinVelocity);

		CamFollow.distance = Mathf.Lerp (MinFollowDistance, MaxFollowDistance, t);
		CamFollow.height = Mathf.Lerp (MinFollowHeight, MaxFollowHeight, t);
		//CamFollow.distance = Mathf.Clamp( TheVehicle.rigidbody.velocity.magnitude , MinFollowDistance, MaxFollowDistance);
		//CamFollow.height   = Mathf.Clamp( TheVehicle.rigidbody.velocity.magnitude , MinFollowHeight, MaxFollowHeight);
//		if( TheVehicle.rigidbody.velocity > 0 )
//		{
//
//				MaxFollowDistance
//		}
	}


	public void Update ()
	{
		///===============================================================
		GenerateStraightLineObstacleTrack ();
		///===============================================================
	}

	public bool isDoOldTrack = false;
	public void GetTrackPoints (TrackBuildRTrack track)
	{
		if (isDoOldTrack) {
			TrackBuildRPoint p0 = track.gameObject.AddComponent<TrackBuildRPoint> ();// ScriptableObject.CreateInstance<TrackBuildRPoint>();
			TrackBuildRPoint p1 = track.gameObject.AddComponent<TrackBuildRPoint> ();//ScriptableObject.CreateInstance<TrackBuildRPoint>();
			TrackBuildRPoint p2 = track.gameObject.AddComponent<TrackBuildRPoint> ();//ScriptableObject.CreateInstance<TrackBuildRPoint>();
			TrackBuildRPoint p3 = track.gameObject.AddComponent<TrackBuildRPoint> ();//ScriptableObject.CreateInstance<TrackBuildRPoint>();
			
			p0.baseTransform = transform;
			p1.baseTransform = transform;
			p2.baseTransform = transform;
			p3.baseTransform = transform;
			
			p0.position = new Vector3 (-20, 0, -20);
			p1.position = new Vector3 (20, 0, -20);
			p2.position = new Vector3 (20, 0, 20);
			p3.position = new Vector3 (-20, 0, 20);
			
			p0.forwardControlPoint = new Vector3 (0, 0, -20);
			p1.forwardControlPoint = new Vector3 (40, 0, -20);
			p2.forwardControlPoint = new Vector3 (0, 0, 20);
			p3.forwardControlPoint = new Vector3 (-40, 0, 20);
			
			p0.leftForwardControlPoint = new Vector3 (-15, 0, -20);
			p1.leftForwardControlPoint = new Vector3 (25, 0, -20);
			p2.leftForwardControlPoint = new Vector3 (5, 0, 20);
			p3.leftForwardControlPoint = new Vector3 (-35, 0, 20);
			
			p0.rightForwardControlPoint = new Vector3 (15, 0, -20);
			p1.rightForwardControlPoint = new Vector3 (55, 0, -20);
			p2.rightForwardControlPoint = new Vector3 (-5, 0, 20);
			p3.rightForwardControlPoint = new Vector3 (-45, 0, 20);
			
			track.AddPoint (p0);
			track.AddPoint (p1);
			track.AddPoint (p2);
			track.AddPoint (p3);
		} else {

			int trackPointCount = 10;
			
			for(int i =0; i <trackPointCount; i++)
			{

				Vector3 temp;
				TrackBuildRPoint p0 = track.gameObject.AddComponent<TrackBuildRPoint> ();// ScriptableObject.CreateInstance<TrackBuildRPoint>();

				if (i < trackPointCount / 2)
					p0.position = new Vector3 (30 * i * i, Random.Range (-30, 30), 100 * i);
				else
					p0.position = new Vector3 (5 * (trackPointCount / 2 - i) * (trackPointCount / 2 - i), Random.Range (-30, 30), 100 * (trackPointCount / 2 - i));

//				if(i < trackPointCount/4)
//				{	temp = new Vector3(i*20,0,0);
//
//					Debug.DrawRay(new Vector3(i*20,0,0),Vector3.up*10,Color.green,20);
//				}
//				else
//					if(i >= trackPointCount/4 && i< trackPointCount/2)
//				{
//					int goingup = i - trackPointCount/4; 
//
//					temp = new Vector3(trackPointCount/4* 20,0,goingup*20);
//					Debug.DrawRay(temp,Vector3.up*10,Color.green,20);
//				}
//				else
//					if(i >= trackPointCount/2 && i< 3*trackPointCount/4)
//				{
//					int goingup = 3*trackPointCount/4 - i-1; 
//
//					temp = new Vector3(goingup*20,0,trackPointCount/4*20);
//					Debug.DrawRay(temp,Vector3.up*10,Color.green,20);
//				}
//				else
//				{
//					int goingup = trackPointCount - i-1; 
//
//					temp = new Vector3(0,0,goingup*20);
//					Debug.DrawRay(temp,Vector3.up*10,Color.green,20);
//				}

				if(Application.isEditor)
				{
					p0.boundaryHeight = 3;
					p0.generateBumpers =true;
					p0.width = Random.Range(10,20);

					p0.extrudeLength = 10;
					p0.extrudeBevel = 5;
				}

				track.AddPoint(p0);
			}

//			for (int i =0; i <trackPointCount; i++) {
//				if (i < trackPointCount - 1) {
//					track.GetPoint (i).forwardControlPoint = (track.GetPoint (i + 1).position + track.GetPoint (i).position) / 2;
//				} else {
//					track.GetPoint(i).forwardControlPoint = track.GetPoint(i).position - (track.GetPoint(i-1).position - track.GetPoint(i).position);
//					//track.GetPoint (i).forwardControlPoint = (track.GetPoint (0).position + track.GetPoint (i).position) / 2;
//				}
//			}

		}
	}


	int i = 1;
	public GameObject obstaclePrefab;
	public int numOfThing;
	public float rangeOfRand;

	void GenerateStraightLineObstacleTrack ()
	{
		if (Vector3.SqrMagnitude (TheVehicle.transform.position - LatestPath.transform.position) < 500000) {
			i++;

			LatestPath = GenerateGround (LatestPath.transform.position + new Vector3 (0, 0, 500));
			GenerateObstacles (LatestPath.transform.position, i, LatestPath);

			LatestPath.name = LatestPath.name + i.ToString ();

			Debug.Log (listOfPieces [0].gameObject);
			GameObject.Destroy (listOfPieces [0].gameObject);
			//Debug.Log("des");

			listOfPieces.Add (LatestPath);
			listOfPieces.RemoveAt (0);
		}
	}


	GameObject GenerateGround (Vector3 point)
	{
		return GameObject.Instantiate (Ground, point, Quaternion.identity) as GameObject;
	}

	void GenerateObstacles (Vector3 point, int additional=0, GameObject Parent =null)
	{
		//GameObject.Instantiate(Ground,point,Quaternion.identity);

		for (int i=0; i <numOfThing + additional; i++) {
			Vector2 vec2 = Random.insideUnitCircle * rangeOfRand;

			if (Parent)
				(GameObject.Instantiate (obstaclePrefab,
			                       new Vector3 (vec2.x, 0, vec2.y) + point,
			                       obstaclePrefab.transform.rotation) as GameObject).transform.parent = Parent.transform;
			else
				GameObject.Instantiate (obstaclePrefab,
				                       new Vector3 (vec2.x, 0, vec2.y) + point,
				                       obstaclePrefab.transform.rotation);
		}
	}
}
