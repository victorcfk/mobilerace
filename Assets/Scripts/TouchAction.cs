using UnityEngine;
using System.Collections.Generic;

public abstract class TouchAction : MonoBehaviour 
{
    abstract public void onAction(float thing = 0);

}