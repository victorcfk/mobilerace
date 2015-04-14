using UnityEngine;
using System.Collections;

public class TouchActionAccelerate : TouchAction 
{
    public ApplyPhysics apply;
    
    float accAmt;

    public override void onAction(float thing= 0)
    {
//		return;

		thing *= 1.40f;
		thing -= 0.20f;

		apply.normalizedVal = Mathf.Clamp(thing,0f,1f);
		apply.usingKey = false;
    }

}
