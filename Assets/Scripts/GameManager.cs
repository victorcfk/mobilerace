using UnityEngine;
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
	public Transform CamFollowObject;

	public bool isDoOldTrack;

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

	public Text gtext;
	public Text LeftEng;
	public Text RightEng;

	void Awake ()
	{
		_instance = this;

		Screen.orientation = ScreenOrientation.Landscape;

		if(CamFollowObject != null)	CamFollow.camFollowTarget = CamFollowObject;

		Random.seed = System.DateTime.Now.Hour;

	}
        
	// Update is called once per frame
	void LateUpdate ()
	{

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

	List<int> SLR = new List<int>();
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
	}

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
			//int trackPointCount = 80;
			int numOfInterval = 30;
			int trackInterval = 20;	//must be even

			float lastknownx = 0;
			float lastknowny = 0;
			float lastknownz = 0;

			List<Vector3> pointlist = new List<Vector3>();

//			for(int i =0; i <trackPointCount; i++)
//			{
//				float x = 550 + 550 * Mathf.Cos(i/(float)trackPointCount*360 * Mathf.PI / 180) + Random.Range(-2,2);
//				float y = 0 + 350 * Mathf.Sin(i/(float)trackPointCount*360 * Mathf.PI / 180)+ Random.Range(-2,2);
//
//				lastknowny += Random.Range (-3, 3);
//				pointlist.Add(new Vector3(x,0,y));
//			}

			//===================================================
			//Decide straight or curved
			//===================================================

			Vector3 dirAtEnd 			= Vector3.forward;
			Vector3 lastPointAtInterval = Vector3.zero;

			int straightleftright = 0;

			for(int j =0; j <numOfInterval; j++)
			{
				if(straightleftright == 1 || straightleftright ==0)
					straightleftright = 2;
				else
					straightleftright = Random.Range(0,3);

				if(straightleftright == 0)
				{
					pointlist.AddRange(GenerateRightCurve(lastPointAtInterval,dirAtEnd,trackInterval,200,5));
				}

				if(straightleftright == 1)
				{
					pointlist.AddRange(GenerateLeftCurve(lastPointAtInterval,dirAtEnd,trackInterval,200,5));
				}

				if(straightleftright == 2)
				{
					pointlist.AddRange(GenerateStraight(lastPointAtInterval,dirAtEnd,trackInterval/2,30));
				}

				lastPointAtInterval = pointlist[pointlist.Count-1];//current last point
				dirAtEnd 			= (lastPointAtInterval - pointlist[pointlist.Count-2]).normalized;

				Debug.DrawRay(lastPointAtInterval,dirAtEnd*50,Color.white,5);
				Debug.DrawRay(lastPointAtInterval,Vector3.up*50,Color.red,5);
            }


			//===================================================

			Debug.Log(pointlist.Count +" "+SLR.Count);

			for (int i =0; i <pointlist.Count; i++) 
			{
				TrackBuildRPoint bp = track.gameObject.AddComponent<TrackBuildRPoint> ();// ScriptableObject.CreateInstance<TrackBuildRPoint>();
				
				bp.baseTransform = transform;
				bp.position = pointlist[i] + new Vector3(0,Random.Range(0,-1f),0);

				bp.generateBumpers= true;
				bp.colliderSides = true;
				bp.boundaryHeight = 5;


				bp.extrudeTrackBottom = false;

				if (i < pointlist.Count - 1) 
				{
					bp.forwardControlPoint 	= pointlist[i+1];
//						((pointlist[i+1] + pointlist[i]) / 2 + pointlist[i])/2;
//
//					bp.leftForwardControlPoint 	= //pointlist[i+1];//
//						((pointlist[i+1] + pointlist[i]) / 2 + pointlist[i])/2;
//
//					bp.rightForwardControlPoint 	= //pointlist[i+1];//
//						((pointlist[i+1] + pointlist[i]) / 2 + pointlist[i])/2;
				} 
				else 
				{
					bp.forwardControlPoint 	=   pointlist[i] - (pointlist[i-1] -  pointlist[i]);
//						((pointlist[i] + pointlist[0]) / 2) ;
//
//					bp.leftForwardControlPoint 	=
//						((pointlist[i] + pointlist[0]) / 2) ;
//
//					bp.rightForwardControlPoint 	=
//						((pointlist[i] + pointlist[0]) / 2) ;
				}

				if(SLR[i] == 0)
				{
					bp.width = 50;
					//=============================================
					float angle;
					Vector3 axis;
					bp.trackUpQ.ToAngleAxis(out angle, out axis);
					bp.trackUpQ =  Quaternion.AngleAxis(angle - 20,axis);
					//=============================================
				}
				else
				if(SLR[i] == 1)
				{
					bp.width = 50;
					//=============================================
					float angle;
					Vector3 axis;
					bp.trackUpQ.ToAngleAxis(out angle, out axis);
					bp.trackUpQ =  Quaternion.AngleAxis(angle + 20,axis);
					//=============================================
				}
				else
				{
					bp.width = 70;
					if(i>10 && (i < pointlist.Count-10))
							bp.crownAngle = -7;
				}


				i+=3;

				track.AddPoint(bp);

				track.meshResolution =7;
				track.loop =false;

			}
		}
	}

	Vector3[] GenerateStraight(Vector3 startLoc, Vector3 startDir, int numOfPoints, float intervalBtwnPts)
	{
		Vector3[] vecArray = new Vector3[numOfPoints]; 

		float angle = Vector3.Angle(Vector3.forward,startDir);
		Vector3.Cross(Vector3.forward,startDir);

		Matrix4x4 g = Matrix4x4.TRS(Vector3.zero,
		                            Quaternion.AngleAxis(angle,Vector3.Cross(Vector3.forward,startDir)),
		                            new Vector3(1,1,1));

		for(int i=0; i <numOfPoints; i++)
		{
			float x = 0;
			float y = startDir.y*i;
			float z = i;

			vecArray[i] =
				startLoc +	
					g.MultiplyPoint3x4(
					new Vector3(x,y,z) * 
					intervalBtwnPts);

			SLR.Add(2);
		}

		return vecArray;

	}


	Vector3[] GenerateRightCurve(Vector3 startLoc, Vector3 startDir, int numOfPoints, float intervalBtwnPts, float portionOfCircle = 1)
	{
		Vector3[] vecArray = new Vector3[numOfPoints]; 

		float angle = Vector3.Angle(Vector3.forward,startDir);
		Vector3.Cross(Vector3.forward,startDir);
		
		Matrix4x4 g = Matrix4x4.TRS(Vector3.zero,
		                            Quaternion.AngleAxis(angle,Vector3.Cross(Vector3.forward,startDir)),
		                            new Vector3(1,1,1));
        
		for(int i=0; i <numOfPoints; i++)
		{
			float x =-Mathf.Cos(i/(float)numOfPoints/portionOfCircle*360 * Mathf.PI / 180)+1;//requires the float so the parameter multiplication works
			float y = startDir.y*i;
			float z =Mathf.Sin(i/(float)numOfPoints/portionOfCircle*360 * Mathf.PI / 180);//requires the float so the parameter multiplication works
						
			vecArray[i] =
				startLoc +	
					g.MultiplyPoint3x4(
						new Vector3(x,y,z) * 
                        intervalBtwnPts);
            
			SLR.Add(0);
//            Debug.Log(vecArray[i]);
//			Debug.DrawRay(vecArray[i],Vector3.up*20,Color.white,10);
        }

		return vecArray;

	}

	Vector3[] GenerateLeftCurve(Vector3 startLoc, Vector3 startDir, int numOfPoints, float intervalBtwnPts, float portionOfCircle = 1)
	{
		Vector3[] vecArray = new Vector3[numOfPoints]; 
		
		float angle = Vector3.Angle(Vector3.forward,startDir);
		Vector3.Cross(Vector3.forward,startDir);
		
		Matrix4x4 g = Matrix4x4.TRS(Vector3.zero,
		                            Quaternion.AngleAxis(angle,Vector3.Cross(Vector3.forward,startDir)),
		                            new Vector3(1,1,1));
		
		for(int i=0; i <numOfPoints; i++)
		{
			float x =Mathf.Cos(i/(float)numOfPoints/portionOfCircle*360 * Mathf.PI / 180)-1;//requires the float so the parameter multiplication works
			float y = startDir.y*i;
			float z =Mathf.Sin(i/(float)numOfPoints/portionOfCircle*360 * Mathf.PI / 180);//requires the float so the parameter multiplication works
			
			vecArray[i] =
				startLoc +	
					g.MultiplyPoint3x4(
						new Vector3(x,y,z) * 
						intervalBtwnPts);
            
			SLR.Add(1);
            //			Debug.Log(vecArray[i]);
//			Debug.DrawRay(vecArray[i],Vector3.up*20,Color.white,10);
		}
		
		return vecArray;
	}

//	int i = 1;
//	public GameObject obstaclePrefab;
//	public int numOfThing;
//	public float rangeOfRand;
//
//	public List<GameObject> listOfPieces;
//	
//	public GameObject Ground;
//	
//	public GameObject LatestPath;
//	public GameObject EarliestPath;
//
//	void GenerateStraightLineObstacleTrack ()
//	{
//		if (Vector3.SqrMagnitude (TheVehicle.transform.position - LatestPath.transform.position) < 500000) {
//			i++;
//
//			LatestPath = GenerateGround (LatestPath.transform.position + new Vector3 (0, 0, 500));
//			GenerateObstacles (LatestPath.transform.position, i, LatestPath);
//
//			LatestPath.name = LatestPath.name + i.ToString ();
//
//			GameObject.Destroy (listOfPieces [0].gameObject);
//
//			listOfPieces.Add (LatestPath);
//			listOfPieces.RemoveAt (0);
//		}
//	}
//
//
//	GameObject GenerateGround (Vector3 point)
//	{
//		return GameObject.Instantiate (Ground, point, Quaternion.identity) as GameObject;
//	}
//
//	void GenerateObstacles (Vector3 point, int additional=0, GameObject Parent =null)
//	{
//
//		for (int i=0; i <numOfThing + additional; i++) {
//			Vector2 vec2 = Random.insideUnitCircle * rangeOfRand;
//
//			if (Parent)
//				(GameObject.Instantiate (obstaclePrefab,
//			                       new Vector3 (vec2.x, 0, vec2.y) + point,
//			                       obstaclePrefab.transform.rotation) as GameObject).transform.parent = Parent.transform;
//			else
//				GameObject.Instantiate (obstaclePrefab,
//				                       new Vector3 (vec2.x, 0, vec2.y) + point,
//				                       obstaclePrefab.transform.rotation);
//		}
//	}
}
