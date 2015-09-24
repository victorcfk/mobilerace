using UnityEngine;
using System.Collections;

public class TouchActionAccelerate : TouchAction 
{
//    public ApplyPhysics apply;

//    public DrivingScriptStraight drs;
    public bool isRightSlide;

    public override void onAction(float screenY= 0)
    {
//		screenY *= 1.40f;
//		screenY -= 0.20f;

        if(isRightSlide)
            GameManager.instance.RightPower = Mathf.Clamp(screenY,0f,1f);
        else
            GameManager.instance.LeftPower = Mathf.Clamp(screenY,0f,1f);

    }

}
