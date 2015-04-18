using UnityEngine;
using System.Collections;

public class TouchActionReverseControls : TouchAction {

    static bool reversed =false;

    public ApplyPhysics left;
    public ApplyPhysics right;
	
	// Update is called once per frame
	void Start () {
	
        if(reversed)
            swap();
	}

    override public void onAction(float thing = 0)
    {
        reversed = !reversed;
        swap();
    }

    void swap()
    {
        GameObject temp = left.Engine.gameObject;

        left.Engine = right.Engine;
        right.Engine = temp;

        /*
         *  ApplyPhysics temp = left.apply;

        left.apply = right.apply;
        right.apply = temp;
        */ //ideal scernario
    }

}
