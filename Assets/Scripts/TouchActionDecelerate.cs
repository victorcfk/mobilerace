using UnityEngine;
using System.Collections;

public class TouchActionDecelerate : TouchAction {

    public ApplyPhysics apply;
    
    float accAmt;
    
    void Update()
    {
        accAmt -= Time.deltaTime;
        accAmt = Mathf.Clamp(accAmt,0f,1f);
    }
    public override void onAction(float thing= 0)
    {
        apply.Accelerate = false;
        apply.normalizedVal = 0;
    }
}
