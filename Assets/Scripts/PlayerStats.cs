using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	[Space(10)]
	public float Health;

	public float CurrEnergy;
	public float EnergyRegen;
	public float MaxEnergy;

	[Space(10)]

	public GameObject lane1;
	public GameObject lane2;
	public GameObject lane3;
	public GameObject lane4;
	public GameObject lane5;

	[Space(10)]

	public GameObject SmallShoot;
	public GameObject SmallTank;
	public GameObject BigTank;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (CurrEnergy <= MaxEnergy)
			CurrEnergy += Time.deltaTime * EnergyRegen;

	}


	void CreateSmallShooter(int laneNum)
	{

	}

	void CreateSmallTank(int laneNum)
	{
		
	}

	//Created on the left most thing
	void CreateBigTank(int laneNum)
	{
		
	}



}
