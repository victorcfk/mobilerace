using UnityEngine;
using System.Collections.Generic;

public abstract class TouchAction : MonoBehaviour 
{
//    public List<mystruct> thingggg;
//    WeaponBasic weapB;
//
//    public WeaponBasic testo
//    {
//        get
//        {
//            return weapB;
//        }
//
//        set
//        {
//            weapB = value;
//        }
//    }

    abstract public void onAction(float thing = 0);
}
