using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LuftEngineSystem : MonoBehaviour {

    int i =0;

    [HideInInspector]
    public GameObject controlObj;
    public List<EngineBasic> engines = new List<EngineBasic>();
    
    public float basicAcceleration = 1.0f;
    public float basicMaxSpeed = 10.0f;
    public float basicMaxDegDelta = 90f;
    
    protected float resultantAcceleration;
    protected float resultantMaxSpeed;
    protected float resultantMaxDegDelta;

    public float horizontalMoveVal;
    public float verticalMoveVal;

    public bool allowBackWardsMovement =false;

    // Use this for initialization
    void Start () {

        if (controlObj == null)
            controlObj = GetComponent<PlayerBasic>().gameObject;

        horizontalMoveVal = 0;
        verticalMoveVal = 0;
        
        getAllEngineModifiers();
    }
    
    // Update is called once per frame
    void FixedUpdate () {
        
        getAllEngineModifiers();
        
        turn(horizontalMoveVal);
        accelerate(verticalMoveVal);
        clampMovement();
        
    }
    
    /// <summary>
    /// Based on fed input, move the gameobject left or right
    /// </summary>
    void turn(float input)
    {   
        controlObj.transform.Rotate (new Vector3(0,1,0)//controlObj.transform.up
                                     , -input *  resultantMaxDegDelta * Mathf.Deg2Rad);
        //Quaternion.
        //rigidbody.velocity += rightDir * horizontalMoveVal * resultantAcceleration;
    }

    /// <summary>
    /// Based on fed input, move the gameobject up or down
    /// </summary>
    void accelerate(float input)
    {

            if(Input.GetButton("acc"))
            {
                Vector3 thing = controlObj.transform.forward * input * resultantAcceleration;
                //controlObj.rigidbody.velocity += thing;
                controlObj.rigidbody.AddForce(thing, ForceMode.Acceleration);


                controlObj.rigidbody.useGravity = false;
            }
            else
            {

                //controlObj.rigidbody.useGravity = true;
            }
    }
    
    /// <summary>
    /// Control the maximum speed of movement
    /// </summary>
    void clampMovement()
    {
        controlObj.rigidbody.velocity = Vector3.ClampMagnitude(controlObj.rigidbody.velocity, resultantMaxSpeed);
    }
    
    void getAllEngineModifiers()
    {
        float tempAcc = 0;
        float tempMaxSpd = 0;
        float tempMaxTurn = 0;
        
        foreach (EngineBasic engine in engines)
        {
            tempAcc += engine.moveAccAdd;
            tempMaxSpd += engine.moveSpeedMaxAdd;
            
            tempMaxTurn += engine.turnSpeedDegAdd;
        }
        resultantAcceleration = basicAcceleration + tempAcc;
        resultantMaxSpeed = basicMaxSpeed + tempMaxSpd;
        
        resultantMaxDegDelta = basicMaxDegDelta + tempMaxTurn;
        
    }
    
    
}

