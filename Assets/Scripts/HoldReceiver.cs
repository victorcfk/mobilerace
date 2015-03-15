using System;
using TouchScript.Gestures;
using UnityEngine;
using FullInspector;

public class HoldReceiver : MonoBehaviour {

    public TouchAction[] listOfTouchActionsPressed;
    public TouchAction[] listOfTouchActionsReleased;

    public TouchAction[] listOfTouchActionsPanStart;
    public TouchAction[] listOfTouchActionsPanContinue;
    public TouchAction[] listOfTouchActionsPanEnd;

    private void OnEnable()
    {
        foreach (var tap in GetComponents<PressGesture>())
            tap.Pressed += pressedHandler;

        foreach (var tap in GetComponents<ReleaseGesture>())
            tap.Released += releasedHandler;


        foreach (var tap in GetComponents<PanGesture>())
        {
            tap.PanStarted += panStartedHandler;
        }

        foreach (var tap in GetComponents<PanGesture>())
        {
            tap.Panned += pannedHandler;
        }

        foreach (var tap in GetComponents<PanGesture>())
        {
            tap.PanCompleted += panCompletedHandler;
        }
    }
    
    private void OnDisable()
    {
        foreach (var tap in GetComponents<PressGesture>())
            tap.Pressed -= pressedHandler;
        
        foreach (var tap in GetComponents<ReleaseGesture>())
            tap.Released -= releasedHandler;
        
        
        foreach (var tap in GetComponents<PanGesture>())
        {
            tap.PanStarted -= panStartedHandler;
        }
        
        foreach (var tap in GetComponents<PanGesture>())
        {
            tap.Panned -= pannedHandler;
        }
        
        foreach (var tap in GetComponents<PanGesture>())
        {
            tap.PanCompleted -= panCompletedHandler;
        }
    }

    private void pressedHandler(object sender, EventArgs eventArgs)
    {
        var tap = sender as PressGesture;
        
        foreach(TouchAction tAction in listOfTouchActionsPressed)
            tAction.onAction(tap.NormalizedScreenPosition.y);
    }

    private void releasedHandler(object sender, EventArgs eventArgs)
    {
        var tap = sender as ReleaseGesture;
        
        foreach(TouchAction tAction in listOfTouchActionsReleased)
            tAction.onAction(tap.NormalizedScreenPosition.y);
    }


    private void panStartedHandler(object sender, EventArgs eventArgs)
    {
        var tap = sender as PanGesture;

        foreach(TouchAction tAction in listOfTouchActionsPanStart)
            tAction.onAction(tap.NormalizedScreenPosition.y);
    }

    private void pannedHandler(object sender, EventArgs eventArgs)
    {
        var tap = sender as PanGesture;
        
        foreach(TouchAction tAction in listOfTouchActionsPanContinue)
            tAction.onAction(tap.NormalizedScreenPosition.y);
    }

    private void panCompletedHandler(object sender, EventArgs eventArgs)
    {
        var tap = sender as PanGesture;
        
        foreach(TouchAction tAction in listOfTouchActionsPanEnd)
            tAction.onAction(tap.NormalizedScreenPosition.y);
    }

}
