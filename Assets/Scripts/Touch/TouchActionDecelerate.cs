using UnityEngine;
using System.Collections;

public class TouchActionDecelerate : TouchAction 
{
    public DrivingScriptStraight drs;
    public bool isRightSlide;
    
    public override void onAction(float screenY= 0)
    {
        if(isRightSlide)
            drs.RightPower = 0;
        else
            drs.LeftPower = 0;
    }
}
