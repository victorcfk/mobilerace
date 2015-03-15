using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //Vector3 rel = new Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0);


        //Ray rel = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Vector3 point = rel.origin + (rel.direction * distance);
        //Debug.Log( "World point " + point );
        //    .ScreenToViewportPoint(Input.mousePosition);

        //rel = new Vector3(rel.x, rel.y, 0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //.ScreenPointToRay(Input.mousePosition); 


 //       rel = Camera.mainCamera.ViewportToWorldPoint
   //         (rel);


        // ray didn't hit any solid object, so return the 
        // intersection point between the ray and 
        // the Y=0 plane (horizontal plane)
        float t = -ray.origin.z / ray.direction.z;
        Vector3 rel =  ray.GetPoint(t);

        //print(rel);

        this.gameObject.transform.position = rel;


        Debug.
        DrawLine(Vector3.zero, this.gameObject.transform.position);
	
	}

    void OnDrawGizmos()
    {
        Gizmos.
                DrawLine(Vector3.zero, this.gameObject.transform.position);
    }
}
