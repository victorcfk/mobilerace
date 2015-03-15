using System;
using TouchScript.Gestures;
using UnityEngine;

public class TouchReceiver : MonoBehaviour {

	public TouchAction[] listOfTouchActionsTapOnce;
	public TouchAction[] listOfTouchActionsTapTwice;

	private void OnEnable()
	{
		foreach (var tap in GetComponents<TapGesture>())
		{
			tap.Tapped += tappedHandler;
		}
	}
	
	private void OnDisable()
	{
		foreach (var tap in GetComponents<TapGesture>())
		{
			tap.Tapped -= tappedHandler;
		}
	}
	
	private void tappedHandler(object sender, EventArgs eventArgs)
	{
		var tap = sender as TapGesture;
		switch (tap.NumberOfTapsRequired)
		{
		case 1:
			// our single tap gesture
			foreach(TouchAction tAction in listOfTouchActionsTapOnce)
				tAction.onAction();
			break;
		case 2:
			// our double tap gesture
			foreach(TouchAction tAction in listOfTouchActionsTapTwice)
				tAction.onAction();
			break;

		default:
			break;
		}
	}
}
