using UnityEngine;
using System.Collections;

public class TouchActionAccelerate : TouchAction 
{
    public ApplyPhysics apply;
    
    float accAmt;
    
    void Update()
    {
        //accAmt -= Time.deltaTime;
        //accAmt = Mathf.Clamp(accAmt,0f,1f);


    }


    public override void onAction(float thing= 0)
    {

		//Thing refers to the screen normalized x. We want to make it easier to accelrate and decelerate


    //    apply.Accelerate = true;
//
//		if(thing > 0.5f) thing += 0.15f;
//		else
		thing *= 1.5f;
		thing -= 0.3f;

//		accAmt = thing;

		apply.normalizedVal = Mathf.Clamp(thing,0f,1f);
    }

}
