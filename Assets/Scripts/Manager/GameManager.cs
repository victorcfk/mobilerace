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

	public DrivingScriptBasic TheVehicle;
	Rigidbody vehRigidBody;

	public SmoothFollowCS CamFollow;
	public Transform CamFollowObject;

	[SerializeField]
	Material
		trackMat;
	[SerializeField]
	Material
		trackMat2;
	[SerializeField]
	Material
		borderMat;
	[SerializeField]
	Material
		groundMat;

	public int Mat;

	[Range (1,100)]
	public float
		MinFollowDistance;
	[Range (1,100)]
	public float
		MaxFollowDistance;

	[Range (1,100)]
	public float
		MinFollowHeight;
	[Range (1,100)]
	public float
		MaxFollowHeight;

	public Text gtext;
	public Text LeftEng;
	public Text RightEng;

	List<float> StraightLeftRight = new List<float> ();
	public List<Vector3> generatedPointList = new List<Vector3> ();
	public TrackBuildRTrack track;

	public Vector3 UpperBounds;
	public Vector3 LowerBounds;

    Vector3 CamFollowObjectOrigPosition;

	void Awake ()
	{
		_instance = this;

		vehRigidBody = TheVehicle.GetComponent<Rigidbody> ();

		if (CamFollowObject != null)
			CamFollow.camFollowTarget = CamFollowObject;

        CamFollowObjectOrigPosition = CamFollowObject.transform.localPosition;

		Random.seed = System.DateTime.Now.Minute;

	}
        
	// Update is called once per frame
	void LateUpdate ()
	{
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Space)) {
			Application.LoadLevel (0);
		}

        CamManagement();
	}

    public void CamManagement()
    {
        float t = (vehRigidBody.velocity.sqrMagnitude - TheVehicle.MinSpeed * TheVehicle.MinSpeed) / (TheVehicle.MaxSpeed * TheVehicle.MaxSpeed - TheVehicle.MinSpeed * TheVehicle.MinSpeed);
        float a = (TheVehicle as DrivingScriptStraight).LeftRightAcc;

        CamFollow.distance = Mathf.Lerp (MinFollowDistance, MaxFollowDistance, t);
        CamFollow.height = Mathf.Lerp (MinFollowHeight, MaxFollowHeight, t);

        CamFollowObject.transform.localPosition = Vector3.MoveTowards(CamFollowObject.transform.localPosition, CamFollowObjectOrigPosition + Vector3.right*a*7,Time.deltaTime*7);

    }

	public void Update ()
	{
		gtext.text = vehRigidBody.velocity.magnitude.ToString ("F0");

		//======================================================
		if (TheVehicle is DrivingScriptTwinEngine) {
			float t1 = (TheVehicle as DrivingScriptTwinEngine).Left.normalizedVal;

			if (t1 > 0)
				LeftEng.text = ((TheVehicle as DrivingScriptTwinEngine).Left.normalizedVal * 100).ToString ("F0") + "%";
			else
				LeftEng.text = "Brake";

			LeftEng.color = new Color (t1, (1 - t1), 0);
		}

		if (TheVehicle is DrivingScriptStraight) {
			LeftEng.text = Input.acceleration.x.ToString ("F0");
		}

		//======================================================
		if (TheVehicle is DrivingScriptTwinEngine) {
			float t2 = (TheVehicle as DrivingScriptTwinEngine).Right.normalizedVal;

			if (t2 > 0)
				RightEng.text = ((TheVehicle as DrivingScriptTwinEngine).Right.normalizedVal * 100).ToString ("F0") + "%";
			else
				RightEng.text = "Brake";

			RightEng.color = new Color (t2, (1 - t2), 0);
		}
		//======================================================
	}

	public void GetTrackPoints (TrackBuildRTrack track)
	{
		//int trackPointCount = 80;
		int numOfInterval = 25;
		int trackInterval = 20;	//must be even

		
		float crownAngle = -5;

		//===================================================
		//Decide straight or curved
		//===================================================

		Vector3 dirAtEnd = Vector3.forward;
		Vector3 lastPointAtInterval = Vector3.zero;

		int straightleftright = 2;

		for (int j =0; j <numOfInterval; j++) {

			straightleftright = Random.Range (0, 10);
           if(j==0)
                straightleftright =2;

			if (straightleftright == 0) {
				generatedPointList.AddRange (GenerateRightCurve (lastPointAtInterval, dirAtEnd, trackInterval * 5, Random.Range (175, 400), Random.Range (0.25f, 0.75f)));
			}

			if (straightleftright == 1) {
				generatedPointList.AddRange (GenerateLeftCurve (lastPointAtInterval, dirAtEnd, trackInterval * 5, Random.Range (175, 400), Random.Range (0.25f, 0.75f)));
			}

			if (straightleftright >= 2) {
				generatedPointList.AddRange (GenerateStraight (lastPointAtInterval, dirAtEnd, trackInterval / 2, Random.Range (20, 30)));
			}

			lastPointAtInterval = generatedPointList [generatedPointList.Count - 1];//current last point
			dirAtEnd = (lastPointAtInterval - generatedPointList [generatedPointList.Count - 2]).normalized;

			Debug.DrawRay (lastPointAtInterval, dirAtEnd * 50, Color.white, 5);
			Debug.DrawRay (lastPointAtInterval, Vector3.up * 50, Color.red, 5);

		}

		//===================================================

		Debug.Log (generatedPointList.Count + " " + StraightLeftRight.Count);

		DropPointsOnArray (generatedPointList, 0.25f, 0.25f);

		for (int i =0; i <generatedPointList.Count; i++) {
			TrackBuildRPoint bp = track.gameObject.AddComponent<TrackBuildRPoint> ();

			bp.baseTransform = transform;
			bp.position = generatedPointList [i];

			//bp.generateBumpers= true;
			bp.colliderSides = true;

			//bp.boundaryHeight = 10;
			bp.renderBounds = false;
			bp.width = 75;

			if (i < generatedPointList.Count - 1) {
				bp.forwardControlPoint = generatedPointList [i + 1];
			} else {
				bp.forwardControlPoint = 
						2 * generatedPointList [i] - generatedPointList [i - 1];
			}

			if (StraightLeftRight [i] > 0) {

				//=============================================
				float angle;
				Vector3 axis;
				bp.trackUpQ.ToAngleAxis (out angle, out axis);

				float multi = 1;
				if (axis.y < 0) {
					multi = -1;
				}

				if (StraightLeftRight [i] < 0.5f) { //left turn
					bp.trackUpQ = Quaternion.AngleAxis (angle + StraightLeftRight [i] * multi * 90f, axis);
					bp.position += new Vector3 (0, StraightLeftRight [i] * 40f, 0);
                    bp.width += StraightLeftRight [i] * 45;

					Debug.DrawRay (bp.position, axis * 10, Color.green, 5);
				} else {
					bp.trackUpQ = Quaternion.AngleAxis (angle + (1 - StraightLeftRight [i]) * multi * 90f, axis);
					bp.position += new Vector3 (0, (1 - StraightLeftRight [i]) * 40f, 0);
                    bp.width += (1 - StraightLeftRight [i]) * 45;

					Debug.DrawRay (bp.position, axis * 10, Color.green, 5);
				}
				//=============================================
			} else
				if (StraightLeftRight [i] < 0) { //right turn

				//=============================================
				float angle;
				Vector3 axis;
				bp.trackUpQ.ToAngleAxis (out angle, out axis);

				float multi = 1;
				if (axis.y < 0) {
					multi = -1;
				}

				if (StraightLeftRight [i] > -0.5f) {
					bp.trackUpQ = Quaternion.AngleAxis (angle + StraightLeftRight [i] * multi * 90f, axis);
					bp.position += new Vector3 (0, -StraightLeftRight [i] * 30f, 0);
                    bp.width += (-StraightLeftRight [i]) * 100;

					Debug.DrawRay (bp.position, axis * 10, Color.green, 5);

				} else {
					
                    bp.trackUpQ = Quaternion.AngleAxis (angle + (-1 - StraightLeftRight [i]) * multi * 90f, axis);
					bp.position += new Vector3 (0, -(-1 - StraightLeftRight [i]) * 30f, 0);
                    bp.width += (1 - (-StraightLeftRight [i])) * 100;

					Debug.DrawRay (bp.position, axis * 10, Color.green, 5);
				}

				//=============================================
			}
			bp.generateBumpers = false;
			bp.extrudeTrackBottom = false;

			if (i > 10 && (i < generatedPointList.Count - 10))
				bp.crownAngle = crownAngle;

			i += 3; //skip over points

			track.AddPoint (bp);
		}

		track.meshResolution = 10;
		track.loop = false; 

		this.track = track;

    }
    
    public Material GetVariedTrackMatToUse ()
	{
		if (Mat > 7)
			Mat = 0;

		if (Mat > 3) {
			Mat++;

			return trackMat;
		} else {
			Mat++;
			return trackMat2;
		}
	}

	public Material GetTrackMatToUse ()
	{
		return trackMat;
	}

	public Material GetBorderMatToUse ()
	{
		return borderMat;
	}
    
	#region Functions for adding the base points to the track object
	Vector3[] GenerateStraight (Vector3 startLoc, Vector3 startDir, int numOfPoints, float intervalBtwnPts)
	{
		Vector3[] vecArray = new Vector3[numOfPoints]; 

		float angle = Vector3.Angle (Vector3.forward, startDir);
		Vector3.Cross (Vector3.forward, startDir);

		//Matrix by which to rotate piece by
		Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
		                            Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
		                            new Vector3 (1, 1, 1));

		for (int i=0; i <numOfPoints; i++) {
			float x = 0;
			float y = startDir.y * i;
			float z = i;

			vecArray [i] =
				startLoc + 
				g.MultiplyPoint3x4 (
					new Vector3 (x, y, z) * 
				intervalBtwnPts);

			StraightLeftRight.Add (0);
		}

		return vecArray;

	}

	Vector3[] GenerateRightCurve (Vector3 startLoc, Vector3 startDir, int numOfPoints, float intervalBtwnPts, float portionOfCircle = 1)
	{
		Vector3[] vecArray = new Vector3[numOfPoints]; 

		float angle = Vector3.Angle (Vector3.forward, startDir);
		Vector3.Cross (Vector3.forward, startDir);

		//Matrix by which to rotate piece by
		Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
		                            Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
		                            new Vector3 (1, 1, 1));

		portionOfCircle = Mathf.Clamp (portionOfCircle, 0.1f, 1);
		for (int i=0; i <numOfPoints; i++) {
			float x = -Mathf.Cos (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180) + 1;//requires the float so the parameter multiplication works
			float y = startDir.y * i;
			float z = Mathf.Sin (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180);//requires the float so the parameter multiplication works
						
			vecArray [i] =
				startLoc + 
				g.MultiplyPoint3x4 (
						new Vector3 (x, y, z) * 
				intervalBtwnPts);
            
			StraightLeftRight.Add (-i / (float)numOfPoints);
		}

		return vecArray;

	}

	Vector3[] GenerateLeftCurve (Vector3 startLoc, Vector3 startDir, int numOfPoints, float intervalBtwnPts, float portionOfCircle = 1)
	{
		Vector3[] vecArray = new Vector3[numOfPoints]; 
		
		float angle = Vector3.Angle (Vector3.forward, startDir);
		Vector3.Cross (Vector3.forward, startDir);

		//Matrix by which to rotate piece by
		Matrix4x4 g = Matrix4x4.TRS (Vector3.zero,
		                            Quaternion.AngleAxis (angle, Vector3.Cross (Vector3.forward, startDir)),
		                            new Vector3 (1, 1, 1));

		portionOfCircle = Mathf.Clamp (portionOfCircle, 0.1f, 1);
		for (int i=0; i <numOfPoints; i++) {
			float x = Mathf.Cos (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180) - 1;//requires the float so the parameter multiplication works
			float y = startDir.y * i;
			float z = Mathf.Sin (i / (float)numOfPoints * portionOfCircle * 360 * Mathf.PI / 180);//requires the float so the parameter multiplication works
			
			vecArray [i] =
				startLoc + 
				g.MultiplyPoint3x4 (
						new Vector3 (x, y, z) * 
				intervalBtwnPts);
            
			StraightLeftRight.Add (i / (float)numOfPoints);
		}
		
		return vecArray;
	}

	/// <summary>
	/// Lowers all the points in the array by a random amount bounded by the provided ranges.
	/// </summary>
	/// <param name="pointlist">Pointlist.</param>
	/// <param name="dropLowerRange">Drop lower range.</param>
	/// <param name="dropUpperRange">Drop upper range.</param>
	void DropPointsOnArray (List<Vector3> pointlist, float dropLowerRange, float dropUpperRange)
	{
		float dropAmount = 0;
		for (int i =0; i <pointlist.Count; i++) {
			dropAmount += Random.Range (-dropLowerRange, -dropUpperRange);
			
			pointlist [i] += new Vector3 (0, dropAmount, 0); //DropPointOnArray(pointlist[i],i,0.1f); 
		}
	}

	#endregion

	#region Functions for creating the ground Quad and buildings

	void ParseTrackBoundsAndCreateQuad (List<Vector3> pointlist)
	{
		UpperBounds = Vector3.zero;
		LowerBounds = Vector3.zero;
		
		for (int i =0; i <pointlist.Count; i++) {
			UpperBounds.x = Mathf.Max (UpperBounds.x, pointlist [i].x);
			UpperBounds.y = Mathf.Max (UpperBounds.y, pointlist [i].y);
			UpperBounds.z = Mathf.Max (UpperBounds.z, pointlist [i].z);
			
			LowerBounds.x = Mathf.Min (LowerBounds.x, pointlist [i].x);
			LowerBounds.y = Mathf.Min (LowerBounds.y, pointlist [i].y);
			LowerBounds.z = Mathf.Min (LowerBounds.z, pointlist [i].z);
		}
		
		GameObject.Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube), UpperBounds, Quaternion.identity);
		GameObject.Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube), LowerBounds, Quaternion.identity);
		
		Vector3 g = (UpperBounds + LowerBounds) / 2;
		g.y = LowerBounds.y;
		
		float width = UpperBounds.x - LowerBounds.x;
		float height = UpperBounds.z - LowerBounds.z;
		
		Vector2 textureScale = new Vector2 (width / Mathf.Min (width, height), height / Mathf.Min (width, height));
		
		Vector3[] vertices = new Vector3[]
		{
			new Vector3 (UpperBounds.x, g.y, UpperBounds.z),
			new Vector3 (UpperBounds.x, g.y, LowerBounds.z),
			new Vector3 (LowerBounds.x, g.y, UpperBounds.z),
			new Vector3 (LowerBounds.x, g.y, LowerBounds.z),
		};

		//QuadCreate(new Vector3(width,height,1),g,textureScale);
		GetMeshWithTexture (vertices, textureScale);
	}

	public GameObject groundQuad;
	/// <summary>
	/// Creates a mesh and binds a texture to it with texturescale
	/// </summary>
	/// <param name="vertices">Vertices.</param>
	/// <param name="textureScale">Texture scale.</param>
	void QuadCreate (Vector3 scale, Vector3 cent, Vector2 textureScale)
	{  
		groundQuad.transform.localScale = scale;

		groundQuad.transform.position = cent;

		groundQuad.GetComponent<MeshRenderer> ().material.SetTextureScale ("_MainTex", textureScale);
	}


	/// <summary>
	/// Creates a mesh and binds a texture to it with texturescale
	/// </summary>
	/// <param name="vertices">Vertices.</param>
	/// <param name="textureScale">Texture scale.</param>
	void GetMeshWithTexture (Vector3[] vertices, Vector2 textureScale)
	{  
		
		// Create object
		Mesh _m1 = CreateMeshFromVertices (vertices);
		var item = (GameObject)new GameObject (
			"GeneratedGround", 
			typeof(MeshRenderer), // Required to render
			typeof(MeshFilter)    // Required to have a mesh
		);

		item.GetComponent<MeshFilter> ().mesh = _m1;
		item.GetComponent<MeshRenderer> ().material = (groundMat);

		item.GetComponent<MeshRenderer> ().material.SetTextureScale ("_MainTex", textureScale);

	}

	// Create a quad mesh
	Mesh CreateMeshFromVertices (Vector3[] vertices)
	{
		
		Mesh mesh = new Mesh ();
		
		Vector2[] uv = new Vector2[]
		{
			new Vector2 (1, 1),
			new Vector2 (1, 0),
			new Vector2 (0, 1),
			new Vector2 (0, 0),
		};
		
		int[] triangles = new int[]
		{
			0, 1, 2,
			2, 1, 3,
		};
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
        
		return mesh;
	}

	public GameObject[] Buildings;
	public void PostTrackBuild()
	{
		ParseTrackBoundsAndCreateQuad (generatedPointList);

		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);

		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);

		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);

		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[0]);


		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);

		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);

		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);
		SphereCastWithinBoundaryForRoom (UpperBounds,LowerBounds,100,Buildings[1]);

	}
	
	void SphereCastWithinBoundaryForRoom (Vector3 UpperBounds, Vector3 LowerBounds, float sphereRadius, GameObject obj)
	{
		Vector3 topCent = (UpperBounds + LowerBounds) / 2;
		topCent.y = UpperBounds.y;

		Vector3 btmCent = (UpperBounds + LowerBounds) / 2;
		btmCent.y = LowerBounds.y;

        while (true) 
		{
			float xTestLoc = Random.Range(LowerBounds.x,UpperBounds.x);
			float zTestLoc = Random.Range(LowerBounds.z,UpperBounds.z);

			Vector3 rayStart = new Vector3(xTestLoc,LowerBounds.y-sphereRadius,zTestLoc);	//want to start below the ground for accuracy, spherecast ignores objects it starts in
			Ray ray = new Ray (rayStart , Vector3.up);

			RaycastHit rch;

			if (
				!Physics.SphereCast (ray,sphereRadius,out rch,(UpperBounds.y - LowerBounds.y) + sphereRadius)
				) 
			{
				Debug.DrawRay (rayStart, Vector3.up, Color.cyan, 5);
				GameObject g = GameObject.Instantiate (obj, rayStart, obj.transform.rotation) as GameObject;
                
				g.transform.localScale = new Vector3(

					g.transform.localScale.x*Random.Range(1,1.5f),
					g.transform.localScale.y*Random.Range(1,1.5f),
					g.transform.localScale.z*Random.Range(1,1.5f));
				return;
			} 
		}
	}

	#endregion


}
