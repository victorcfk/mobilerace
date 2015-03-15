using System;
using TouchScript.Gestures;
using UnityEngine;
using Random = UnityEngine.Random;

public class TouchActionSpawnUnit : TouchAction
{
	public GameObject UnitToSpawn;

	public override void onAction(float thing= 0)
	{
		Spawn ();
	}
	
	public void Spawn()
	{
		GameObject.Instantiate (UnitToSpawn, transform.position, UnitToSpawn.transform.rotation);
	}
}