using UnityEngine;
using System.Collections;

public class SpeedBoost : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            SpeedUp(other.GetComponent<DrivingScriptStraight>());
        }
    }

    void SpeedUp(DrivingScriptStraight other)
    {
        other.GainSpeedBoost();
    }
}
