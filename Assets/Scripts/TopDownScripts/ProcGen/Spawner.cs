using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public List<GameObject> ShipArray;
    public int numOfShips = 5;
    public GameObject ShipPrefab;

    public float coolDown = 1;
    private float timer= 0;

    //public 
	// Use this for initialization
	void Start () {
        ShipArray = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (timer <= 0)
        {
            if (ShipArray.Count < numOfShips)
            {
                ShipArray.Add((GameObject)Instantiate(ShipPrefab, transform.position, ShipPrefab.transform.rotation));

                timer = coolDown;
                //numOfShips++;
            }
            else
                timer = 0;

        }

        timer -= Time.deltaTime;
	}
}
