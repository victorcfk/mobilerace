using UnityEngine;
using System.Collections;

public class CreateObstacle : MonoBehaviour {

    public GameObject obstaclePrefab;

    public int numOfThing;
    public float rangeOfRand;

	// Use this for initialization
	void Start () {

        //
//        Random.Range
//        {collider.bounds.max.y
//
//            );

        for(int i=0; i <numOfThing; i++)
        {
            Vector2 vec2 = Random.insideUnitCircle * rangeOfRand;

            GameObject.Instantiate(obstaclePrefab,
                                   new Vector3(vec2.x,0,vec2.y) +transform.position,
                                   obstaclePrefab.transform.rotation);
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
