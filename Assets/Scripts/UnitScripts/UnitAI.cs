using UnityEngine;
using System.Collections;

public class UnitAI : MonoBehaviour {

	public MovementScript move;
	public AttackScript attack;

	public bool IsTargetInRange;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
	
        if (!IsTargetInRange)
        {
            move.StartMoving();

        } 
        else
        {
            attack.StartAttacking();

        }
    }
}
