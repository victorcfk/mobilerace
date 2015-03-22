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

	// Use this for initialization
	void Start ()
	{
		float randx = startLoc.x;
		float randz = startLoc.z;
		float randy = startLoc.y;

	//	 = new BezierCurve ();

		for (int i=0; i<30; i++) 
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

		
//		GameObject.Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube),
//				                       b.GetPointAt (0), Quaternion.identity);
//		
//		GameObject.Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube),
//				                       b.GetPointAt (0.5f), Quaternion.identity);
//		
//		GameObject.Instantiate (GameObject.CreatePrimitive (PrimitiveType.Cube),
//				                       b.GetPointAt (1), Quaternion.identity);
	}
}
