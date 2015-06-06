using UnityEngine;
using System.Collections;

public class VehRotateOnTurn : MonoBehaviour {

    public DrivingScriptStraight DS;
    public GameObject VehicleCore;
    public float rotateSpeed= 120;

    float TurnValue;


    public float MaxTurnAngle = 90;

	// Update is called once per frame
	void FixedUpdate () {
       
        if(DS == null) return;

        TurnValue = DS.LeftRightAcc;

        transform.localRotation = 
            Quaternion.RotateTowards( transform.localRotation,
                                     Quaternion.AngleAxis(TurnValue*-MaxTurnAngle,Vector3.forward),Time.deltaTime*rotateSpeed);
	}
}
