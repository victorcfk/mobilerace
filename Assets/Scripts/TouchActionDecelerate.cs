using UnityEngine;
using System.Collections;

public class TouchActionDecelerate : TouchAction {

    public ApplyPhysics apply;
    
    float accAmt;
    
    public override void onAction(float thing= 0)
    {
        //apply.Accelerate = false;
        apply.normalizedVal = 0;
    }
}
