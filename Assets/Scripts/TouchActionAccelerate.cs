using UnityEngine;
using System.Collections;

public class TouchActionAccelerate : TouchAction 
{

    public ApplyPhysics apply;
    
    float accAmt;
    
    void Update()
    {
        accAmt -= Time.deltaTime;
        accAmt = Mathf.Clamp(accAmt,0f,1f);
    }
    public override void onAction(float thing= 0)
    {
        apply.Accelerate = true;
        apply.normalizedVal = Mathf.Clamp(thing,0f,1f);
    }

    //    public GameObject thingToAcc;
    //    public GameObject Track;
    //    bool isAcc;
    //  
    //    float accAmt;
    //
    //    void Update()
    //    {
    //        accAmt -= Time.deltaTime;
    //        accAmt = Mathf.Clamp(accAmt,0f,1f);
    //    }
    //  // Update is called once per frame
    //  void FixedUpdate () {
    //  
    //        if(accAmt >0)
    //        {
    //            //Track.rigidbody.AddForceAtPosition(Track.transform.forward,Track.transform.position);
    //            
    //            //rigidbody.AddRelativeForce(Track.transform.forward*100,Track.transform.position);
    //            
    //            thingToAcc.rigidbody.AddForceAtPosition(Track.transform.forward*15,Track.transform.position);
    //            //Track.rigidbody.AddForceAtPosition(Track.transform.forward,Track.transform.position);
    //        }
    //        else
    //        {
    //            thingToAcc.rigidbody.AddForceAtPosition(Track.transform.forward*2,Track.transform.position);
    //        }
    //
    //  }
    //
    //    public override void onAction()
    //    {
    //        accAmt++;
    //        accAmt = Mathf.Clamp(accAmt,0f,1f);
    //    }

}
