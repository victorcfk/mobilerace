using UnityEngine;
using System.Collections.Generic;

public class BezierTrack : MonoBehaviour
{

	public List<GameObject> listOfPieces;

	public GameObject PathPiece;
	public GameObject LatestPath;
	public GameObject EarliestPath;

	public BezierCurve b;

	public Vector3 startLoc;

	public TrackBuildRTrack track;
	public TrackBuildRGenerator generator;

	// Use this for initialization
	void Start ()
	{
		float randx = startLoc.x;
		float randz = startLoc.z;
		float randy = startLoc.y;

		for (int i=0; i<10; i++) 
		{
			b.AddPointAt (new Vector3 (randx, randy-=0.15f, randz) * 35);

			randx = randx + Random.Range(-i/3,i/3);
			randz = randz + Random.Range(5,10);	//Z must be positive, to keep moving forward
		}

		for (int i=0; i<b.pointCount; i++) 
		{
			BezierPoint g = b [i];
		
			if(i > 0)
				g.handle1 = (b [i-1].position - g.position)/2;
		
			if(i < b.pointCount-1)
				g.handle2 = (b [i+1].position - g.position)/2;
		}
		
		b.SetDirty ();
	}

	public void Init(BezierCurve curve)
	{
		track = gameObject.AddComponent<TrackBuildRTrack>();
		track.InitTextures();
		track.baseTransform = transform;

		for(int i=0; i < curve.pointCount; i++)
		{

			TrackBuildRPoint p0 = gameObject.AddComponent<TrackBuildRPoint>();// ScriptableObject.CreateInstance<TrackBuildRPoint>();

			p0.baseTransform = transform;
			
			p0.position = curve[i].position;
			
			p0.forwardControlPoint = new Vector3(0, 0, -20);

			//BezierPoint g = b [i];

//			p0.leftForwardControlPoint = g.handle1;
//			p0.rightForwardControlPoint = g.handle2;
			
			track.AddPoint(p0);
		}

		generator = gameObject.AddComponent<TrackBuildRGenerator>();
		
		ForceFullRecalculation();
		
		track.diagramMesh = new Mesh();
		track.diagramMesh.vertices = new [] { new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(-1, 0, 1), new Vector3(1, 0, 1)};
		track.diagramMesh.uv = new [] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0,1), new Vector2(1,1)};
		track.diagramMesh.triangles = new []{1,0,2,1,2,3};
		
		track.diagramGO = new GameObject("Diagram");
		track.diagramGO.transform.parent = transform;
		track.diagramGO.transform.localPosition = Vector3.zero;
		track.diagramGO.AddComponent<MeshFilter>().mesh = track.diagramMesh;
		track.diagramMaterial = new Material(Shader.Find("Unlit/Texture"));
		track.diagramGO.AddComponent<MeshRenderer>().material = track.diagramMaterial;
		track.diagramGO.AddComponent<MeshCollider>().sharedMesh = track.diagramMesh;
	}

	public void UpdateRender()
	{
		generator.track = track;
		generator.UpdateRender();
		
		foreach(Transform child in GetComponentsInChildren<Transform>())
		{
			child.gameObject.isStatic = gameObject.isStatic;
		}
	}
	
	public void ForceFullRecalculation()
	{
		int numberOfPoints = track.realNumberOfPoints;
		for (int i = 0; i < numberOfPoints; i++)
			track[i].isDirty = true;
		track.RecalculateCurves();
		UpdateRender();
	}


	void CheapInstantiate()
	{
		float h = 0;
		GameObject temp;
		do {
			temp = (GameObject.Instantiate (PathPiece,
			                                b.GetPointAt (h), Quaternion.identity) as GameObject);
			
			temp.transform.parent = transform;
			
			temp.transform.LookAt(b.GetPointAt (h+ 0.01f));
			
			listOfPieces.Add(temp);
			
			h += 0.01f;
			
		} while(h <= 1f);
	}
}
