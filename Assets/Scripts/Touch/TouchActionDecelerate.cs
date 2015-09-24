using UnityEngine;
using System.Collections;

public class TouchActionDecelerate : TouchAction 
{
//    public DrivingScriptStraight drs;
    public bool isRightSlide;
    
    public override void onAction(float screenY= 0)
    {
        if(isRightSlide)
            GameManager.instance.RightPower = 0;
        else
            GameManager.instance.LeftPower = 0;
    }
}
