﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	public GameObject PathPiece;

	[Range (1,20)]
	public float MinFollowDistance;
	[Range (1,20)]
	public float MaxFollowDistance;

	[Range (1,20)]
	public float MinFollowHeight;
	[Range (1,20)]
	public float MaxFollowHeight;

//	public BezierCurve b;
	public Vector3 startPoint;

	public List<GameObject> listOfPieces;

	public GameObject Ground;

	public GameObject LatestPath;
	public GameObject EarliestPath;

	void Awake()
	{
		_instance = this;

		Screen.orientation = ScreenOrientation.Landscape;
	}

	// Use this for initialization
	void Start () {

		//LatestPath = GenerateGround(LatestPath.transform.position + new Vector3(0,0,500));

		//BezierCurve b = new BezierCurve();

//		b.AddPointAt(new Vector3(0,0,0)*40);
//		b.AddPointAt(new Vector3(1,0,1)*40);
//
//		b.AddPointAt(new Vector3(4,0,4)*40);
//		b.AddPointAt(new Vector3(6,0,6)*40);
//
//		b.AddPointAt(new Vector3(7,0,7)*40);
//		b.AddPointAt(new Vector3(7,0,10)*40);
//
//		b.AddPointAt(new Vector3(8,0,15)*40);
//		b.AddPointAt(new Vector3(15,0,8)*40);
//
//		for(int i=0; i<b.pointCount; i++)
//		{
//			BezierPoint g = b[i];
//
//			g.handle1 =  new Vector3(-10,0,0);
//
//			g.handle2 =  new Vector3(10,0,0);
//			//g.position + 
//		}
//
//		b.SetDirty();

//		float h=0;
//		do
//		{
//			GameObject.Instantiate(PathPiece,
//			                       b.GetPointAt(h),Quaternion.identity);
//			h+= 0.01f;
//
//		}while(h<=1);
//
//		GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube),
//		                       b.GetPointAt(0),Quaternion.identity);
//
//		GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube),
//		                       b.GetPointAt(0.5f),Quaternion.identity);
//
//		GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube),
//		                       b.GetPointAt(1),Quaternion.identity);
	}

	// Update is called once per frame
	void LateUpdate () 
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
		{
			Application.LoadLevel(0);
		}

		float t = (TheVehicle.GetComponent<Rigidbody>().velocity.sqrMagnitude - TheVehicle.MinVelocity*TheVehicle.MinVelocity) /  (TheVehicle.MaxVelocity*TheVehicle.MaxVelocity - TheVehicle.MinVelocity*TheVehicle.MinVelocity);

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

	int i =1;
	public void Update()
	{
		if(Vector3.SqrMagnitude(TheVehicle.transform.position - LatestPath.transform.position) < 500000)
		{
			i++;

			LatestPath = GenerateGround(LatestPath.transform.position + new Vector3(0,0,500));
			GenerateObstacles(LatestPath.transform.position,i,LatestPath);

			LatestPath.name = LatestPath.name + i.ToString();

			Debug.Log(listOfPieces[0].gameObject);
			GameObject.Destroy(listOfPieces[0].gameObject);
			//Debug.Log("des");

			listOfPieces.Add(LatestPath);
			listOfPieces.RemoveAt(0);
		}
	}


	GameObject GenerateGround(Vector3 point)
	{
		return GameObject.Instantiate(Ground,point,Quaternion.identity) as GameObject;
	}

//	public GameObject Wall;
//	void GenerateWalls(Vector3 point)
//	{
//		GameObject.Instantiate(Wall,point,Quaternion.identity);
//	}

	public GameObject obstaclePrefab;
	public int numOfThing;
	public float rangeOfRand;
	void GenerateObstacles(Vector3 point, int additional=0, GameObject Parent =null)
	{
		//GameObject.Instantiate(Ground,point,Quaternion.identity);

		for(int i=0; i <numOfThing + additional; i++)
		{
			Vector2 vec2 = Random.insideUnitCircle * rangeOfRand;

			if(Parent)
				(GameObject.Instantiate(obstaclePrefab,
			                       new Vector3(vec2.x,0,vec2.y) + point,
			                       obstaclePrefab.transform.rotation) as GameObject).transform.parent = Parent.transform;
			else
				GameObject.Instantiate(obstaclePrefab,
				                       new Vector3(vec2.x,0,vec2.y) + point,
				                       obstaclePrefab.transform.rotation);
		}
	}
}
