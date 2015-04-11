﻿using UnityEngine;
using UnityEngine.UI;
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

	public Material trackMat;
	public Material borderMat;

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

	public Vector3 startPoint;

	public List<GameObject> listOfPieces;

	public GameObject Ground;

	public GameObject LatestPath;
	public GameObject EarliestPath;

	public Text gtext;
	public Text LeftEng;
	public Text RightEng;

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
			if(i== 0 || i == trackPointCount-1 || i== trackPointCount/2-1 || i == 3*trackPointCount/4  || i == 3*trackPointCount/4 - 1 )
				continue;

			if(i < trackPointCount/4)
			{
				Debug.DrawRay(new Vector3(i*20,0,0),Vector3.up*10,Color.green,20);
			}
			else
				if(i > trackPointCount/4 && i< trackPointCount/2)
			{
				int goingup = i - trackPointCount/4; 
				Debug.DrawRay(new Vector3(trackPointCount/4* 20,0,goingup*20),Vector3.up*10,Color.green,20);
			}
			else
				if(i > trackPointCount/2 && i< 3*trackPointCount/4)
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

		for(int i =0; i <trackPointCount; i++)
		{
//			float r =  50;
//			float k = 0;
//			float h = 0;
//			float y;
//			float x;
//			
//			if(i <= trackPointCount/2)
//			{
//				y = i *50;
//				x = Mathf.Sqrt((r*r) - (y-k)*(y-k)) + h;
//			}
//			else
//			{
//				y = trackPointCount - i*50;
//				x = -Mathf.Sqrt((r*r) - (y-k)*(y-k)) + h;
//			}
//		
//			Debug.DrawRay(new Vector3(x,0,y),Vector3.up*10,Color.red,20);
//
			float x = 0 + 10 * Mathf.Cos(i/(float)trackPointCount*360 * Mathf.PI / 180);
			float y = 0 + 10 * Mathf.Sin(i/(float)trackPointCount*360 * Mathf.PI / 180);

			Debug.DrawRay(new Vector3(x,0,y),Vector3.up*30,Color.red,20);
			//p0.position = new Vector3(x,0,y);
		}

		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Space)) 
		{
			Application.LoadLevel (0);
		}

		float t = (TheVehicle.vehRigidbody.velocity.sqrMagnitude - TheVehicle.MinVelocity * TheVehicle.MinVelocity) / (TheVehicle.MaxVelocity * TheVehicle.MaxVelocity - TheVehicle.MinVelocity * TheVehicle.MinVelocity);

		CamFollow.distance = Mathf.Lerp (MinFollowDistance, MaxFollowDistance, t);
		CamFollow.height = Mathf.Lerp (MinFollowHeight, MaxFollowHeight, t);
		//CamFollow.distance = Mathf.Clamp( TheVehicle.rigidbody.velocity.magnitude , MinFollowDistance, MaxFollowDistance);
		//CamFollow.height   = Mathf.Clamp( TheVehicle.rigidbody.velocity.magnitude , MinFollowHeight, MaxFollowHeight);
//		if( TheVehicle.rigidbody.velocity > 0 )
//		{
//				MaxFollowDistance
//		}
	}


	public void Update ()
	{
		gtext.text = TheVehicle.vehRigidbody.velocity.magnitude.ToString("F0");


		//======================================================
		float t1 = TheVehicle.Left.normalizedVal;

		if(t1>0)
			LeftEng.text = (TheVehicle.Left.normalizedVal*100).ToString("F0") +"%";
		else
			LeftEng.text = "Brake";

		LeftEng.color = new Color(t1,(1-t1),0);

		//======================================================

		float t2 = TheVehicle.Right.normalizedVal;

		if(t2>0)
			RightEng.text = (TheVehicle.Right.normalizedVal*100).ToString("F0") +"%";
		else
			RightEng.text = "Brake";

		RightEng.color = new Color(t2,(1-t2),0);


		//======================================================

		GenerateStraightLineObstacleTrack ();
		///===============================================================
	}

	public bool isDoOldTrack;
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
					
			float angle;
			Vector3 axis;
			p0.trackUpQ.ToAngleAxis(out angle, out axis);
			p0.trackUpQ =  Quaternion.AngleAxis(angle + 30,axis);

			p1.trackUpQ.ToAngleAxis(out angle, out axis);
			p1.trackUpQ =  Quaternion.AngleAxis(angle + 30,axis);

			p2.trackUpQ.ToAngleAxis(out angle, out axis);
			p2.trackUpQ =  Quaternion.AngleAxis(angle + 30,axis);

			p3.trackUpQ.ToAngleAxis(out angle, out axis);
			p3.trackUpQ =  Quaternion.AngleAxis(angle + 30,axis);

			track.AddPoint (p0);
			track.AddPoint (p1);
			track.AddPoint (p2);
			track.AddPoint (p3);
		} 
		else 
		{
			int trackPointCount = 50;
			List<Vector3> pointlist = new List<Vector3>();

			float lastknowny = 0;

			track.meshResolution =7;

			for(int i =0; i <trackPointCount; i++)
			{
				float x = 550 + 550 * Mathf.Cos(i/(float)trackPointCount*360 * Mathf.PI / 180) + Random.Range(-2,2);
				float y = 0 + 350 * Mathf.Sin(i/(float)trackPointCount*360 * Mathf.PI / 180)+ Random.Range(-2,2);


				//float t = 0.5; // given example value
				//				float x = (1 - t) * (1 - t) * pointlist[i].x + 2 * (1 - t) * t * pointlist[i+1].x + t * t * pointlist[i+2].x;
				//				float y = (1 - t) * (1 - t) * pointlist[i].y + 2 * (1 - t) * t * pointlist[i+1].y + t * t * pointlist[i+2].y;

				lastknowny += Random.Range (-3, 3);
				pointlist.Add(new Vector3(x,0,y));
			}

			Debug.Log(pointlist.Count);

			for (int i =0; i <pointlist.Count; i++) 
			{
				TrackBuildRPoint bp = track.gameObject.AddComponent<TrackBuildRPoint> ();// ScriptableObject.CreateInstance<TrackBuildRPoint>();
				
				bp.baseTransform = transform;
				bp.position = pointlist[i];
				//bp.crownAngle = -10;
				bp.boundaryHeight = 10;
				bp.width = 50;

				if (i < trackPointCount - 1) 
				{
					bp.forwardControlPoint 	= //pointlist[i+1];//
						((pointlist[i+1] + pointlist[i]) / 2 + pointlist[i])/2;

					bp.leftForwardControlPoint 	= //pointlist[i+1];//
						((pointlist[i+1] + pointlist[i]) / 2 + pointlist[i])/2;

					bp.rightForwardControlPoint 	= //pointlist[i+1];//
						((pointlist[i+1] + pointlist[i]) / 2 + pointlist[i])/2;


				} else 
				{
					bp.forwardControlPoint 	= //pointlist[0];//
						((pointlist[i] + pointlist[0]) / 2) ;

					bp.leftForwardControlPoint 	=
						((pointlist[i] + pointlist[0]) / 2) ;

					bp.rightForwardControlPoint 	=
						((pointlist[i] + pointlist[0]) / 2) ;
				}

				//i++;

				//=============================================
				float angle;
				Vector3 axis;
				bp.trackUpQ.ToAngleAxis(out angle, out axis);
				bp.trackUpQ =  Quaternion.AngleAxis(angle + 30,axis);
				//=============================================



				track.AddPoint(bp);
			}
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

			GameObject.Destroy (listOfPieces [0].gameObject);

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
