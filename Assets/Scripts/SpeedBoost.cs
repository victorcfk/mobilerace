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
            float temp = SpeedUp(other.GetComponent<DrivingScriptStraight>());

            StartCoroutine(SlowDown(other.GetComponent<DrivingScriptStraight>(),temp,2));
        }
    }

    float SpeedUp(DrivingScriptStraight other)
    {
        float origSpeed = other.MaxSpeed;
        float newSpeed = other.MaxSpeed * 1.5f;
        other.MaxSpeed = newSpeed;
        
        other.rigidBody.velocity = other.MaxSpeed * other.rigidBody.velocity.normalized;

        return newSpeed - origSpeed;
    }

//    void SlowDown(DrivingScriptStraight other, float slowDownAmt)
//    {
//        other.MaxSpeed -= slowDownAmt;
//    }

    IEnumerator SlowDown(DrivingScriptStraight other, float slowDownAmt, float waitTime) {

        yield return new WaitForSeconds(waitTime);

        other.MaxSpeed -= slowDownAmt;
    }
}
