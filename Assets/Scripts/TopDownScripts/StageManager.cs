using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour 
{
    //Here is a private reference only this class can access
    private static StageManager _instance;

    public static int combatStageToLoad = 1;

    protected float stageTimer = 0;
	protected float secToNextWave =0 ;
    public int upcomingWave = 0;

    public List<GameObject> spawnWaves;
    public float[] spawnTimingsSeconds;

    public GUIStyle HealthBarStyle;
    public GUIStyle InfoStyle;
    public ParticleSystem pg;

    protected float healthGain = 0;

    public bool isBuildingStage { get { return Application.loadedLevel == 0; } }
    public bool isCurrentWaveClear 
    { 
        get 
        {
            //print("waver " + (upcomingWave - 1));

            if((upcomingWave - 1) < 0 )
                return true;


            return (spawnWaves [upcomingWave - 1].transform.childCount <= 0); 
        } 
    }
    public bool isFinalWave { get { return upcomingWave >= spawnWaves.Count; } }

    public bool areAllWavesClear
    {
        get
        {
            //All waves must have no child game objects
            for (int i=0; i <spawnWaves.Count; i++)
            {
                if( spawnWaves[i].transform.childCount > 0 )
                    return false;
            }
            return true;
        }
    }

    //This is the public reference that other classes will use
    public static StageManager instance
    {
        get
        {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<StageManager>();
            return _instance;
        }
    }

	// Use this for initialization
	void Awake() {

     
	}

    void Start()
    {
        stageTimer = 0;
        upcomingWave = 0;
        
        ShipCoreInfoStore.instance.buildShipNow = true;

        if (!isBuildingStage)
        {
            ShipCoreInfoStore.instance.startFlightNow = true;
            PlayerController.instance.enabled = true;
        } 
        else
        {
            switch(combatStageToLoad)
            {
                case 1:
                    for( int i=0; i <ShipCoreInfoStore.instance.AvailablePartsStage1.Count; i++ )
                    {

                        GameObject g = Instantiate(ShipCoreInfoStore.instance.AvailablePartsStage1[i],
                                    Camera.main.ScreenToWorldPoint( new Vector3(10,Screen.height - 70-i*100,10) ),
                                    Quaternion.LookRotation(Vector3.right, -Vector3.forward)) as GameObject;

//                        if(g)
//                        g.transform.localScale = new Vector3(0.5f,0.5f,0.5f);

                    }
                    break;

                case 2:
                    for( int i=0; i <ShipCoreInfoStore.instance.AvailablePartsStage2.Count; i++ )
                    {
                        
                        GameObject g = Instantiate(ShipCoreInfoStore.instance.AvailablePartsStage2[i],
                                                   Camera.main.ScreenToWorldPoint( new Vector3(10,Screen.height - 70-i*100,10) ),
                                                   Quaternion.LookRotation(Vector3.right, -Vector3.forward)) as GameObject;
                        
                        //                        if(g)
                        //                        g.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                        
                    }
                    break;

                case 3:
                    for( int i=0; i <ShipCoreInfoStore.instance.AvailablePartsStage3.Count; i++ )
                    {
                        
                        GameObject g = Instantiate(ShipCoreInfoStore.instance.AvailablePartsStage3[i],
                                                   Camera.main.ScreenToWorldPoint( new Vector3(10,Screen.height - 70-i*100,10) ),
                                                   Quaternion.LookRotation(Vector3.right, -Vector3.forward)) as GameObject;
                        
                        //                        if(g)
                        //                        g.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                        
                    }
                    break;

                case 4:
                    for( int i=0; i <ShipCoreInfoStore.instance.AvailablePartsStage4.Count; i++ )
                    {
                        
                        GameObject g = Instantiate(ShipCoreInfoStore.instance.AvailablePartsStage4[i],
                                                   Camera.main.ScreenToWorldPoint( new Vector3(10,Screen.height - 70-i*100,10) ),
                                                   Quaternion.LookRotation(Vector3.right, -Vector3.forward)) as GameObject;
                        
                        //                        if(g)
                        //                        g.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                        
                    }
                    break;
                default:
                        break;

            }

        }
    }

    void WarnOfNextWave()
    {
        if(!isFinalWave && secToNextWave < 7)
        {
            for (int j=0; j <spawnWaves[upcomingWave].transform.childCount; j++)
                pg.Emit(spawnWaves [upcomingWave].transform.GetChild(j).position, Vector3.zero, 7 - secToNextWave, 0.1f, Color.red);
        }
    }
	
    void CheckForCheats()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            if (combatStageToLoad < 4)
                ++combatStageToLoad;
            else
                combatStageToLoad = 1;

            Application.LoadLevel(0);
        }
    }

    void FastForwardToUpcomingWave( float secondsToSpawn )
    {
        healthGain = Mathf.Clamp((int)((secToNextWave -secondsToSpawn)/2),0,3);
        PlayerController.instance.playerUnit.health += healthGain;
            
        stageTimer = (spawnTimingsSeconds[upcomingWave] - secondsToSpawn);

    }

    void WaveSpawning()
    {
        for (int i=0; i <spawnTimingsSeconds.Length; i++)
        {
            if(stageTimer >= spawnTimingsSeconds[i])
            {
                ++upcomingWave;
                
                //print("Spawnaaaaaaa!!!!!! " +SpawnTimingsSeconds[i]);
                spawnTimingsSeconds[i] = Mathf.Infinity;
                spawnWaves[i].SetActive(true);
            }
        }
    }

    void LoadNextStage()
    {
        if (combatStageToLoad < 4)
            ++combatStageToLoad;
        else
            combatStageToLoad = 1;

        Application.LoadLevel(0);
    }

	// Update is called once per frame
	void Update () {

        if (isBuildingStage)
            return;

        stageTimer += Time.deltaTime;

        if( !isFinalWave )
            secToNextWave = spawnTimingsSeconds[upcomingWave] - stageTimer;

        //=====================================================
		
        WarnOfNextWave();

        //=====================================================

        WaveSpawning();

        //=====================================================
       
        if (isCurrentWaveClear)
        {
            //++upcomingWave;

            if (secToNextWave > 5)
            {
                FastForwardToUpcomingWave(5);
            }
        }
       
        if(areAllWavesClear)
        {
           LoadNextStage();
        }

        CheckForCheats();
        //=====================================================
	}

    public void OnGUI()
    {
        if (isBuildingStage)
        {
            if (GUI.Button(
            new Rect(Screen.width - 170,
                 20,
                 150,
                 30), "Start"))
            {    
                ShipCoreInfoStore.instance.savePartsAndNewStage = true;
                Application.LoadLevel(combatStageToLoad);

                PlayerController.isAimingWithMouse = true;
            }

            
            //GUI.TextArea

            //Uncommment for gamepad play choice
            //=================================================================
            if (GUI.Button(
                new Rect(Screen.width - 170,
                     100,
                     150,
                     30), "Start With GamePad"))
            {    
                ShipCoreInfoStore.instance.savePartsAndNewStage = true;
                Application.LoadLevel(combatStageToLoad);

                PlayerController.isAimingWithMouse = false;
            }
            //=================================================================

            int i=0;

            //++++++++++++++++++++++++++++++++++++++++++++++++++

            GUI.Label(new Rect(110,
                               40+i*100,
                               350,
                               75
                               ),"Engine| +50% Turn and Move speed",InfoStyle);
            ++i;

            //++++++++++++++++++++++++++++++++++++++++++++++++++
            GUI.Label(new Rect(110,
                               40+i*100,
                               350,
                               75
                               ),"Machine Gun | Rapid-Fire, Inaccurate, MED range, Low DMG",InfoStyle);
            ++i;
            
            //++++++++++++++++++++++++++++++++++++++++++++++++++
            GUI.Label(new Rect(110,
                               40+i*100,
                               350,
                               75
                               ),"Burst Gun | Burst-fire, Accurate, LONG range, MED DMG",InfoStyle);
            ++i;

            if(combatStageToLoad == 2)
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++
                GUI.Label(new Rect(110,
                                   40+i*100,
                                   350,
                                   75
                                   ),"Shot Gun | 35 DegArc, LOW range, MANY LOW DMG bullets",InfoStyle);
                ++i;
                
                //++++++++++++++++++++++++++++++++++++++++++++++++++

                GUI.Label(new Rect(110,
                                   40+i*100,
                                   350,
                                   75
                                   ),"Shield | -0.8 BULLET DMG on hit, +2 health",InfoStyle);
                ++i;
                
            }

            if(combatStageToLoad >= 3)
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++
                GUI.Label(new Rect(110,
                                   40+i*100,
                                   350,
                                   75
                                   ),"Shot Gun | 40 DegArc, LOW range, MANY LOW DMG bullets",InfoStyle);
                ++i;

                //++++++++++++++++++++++++++++++++++++++++++++++++++
                GUI.Label(new Rect(110,
                                   40+i*100,
                                   350,
                                   75
                                   ),"Laser | Penetrating, VHIGH DMG, LOW Rate of fire ",InfoStyle);
                ++i;

                //++++++++++++++++++++++++++++++++++++++++++++++++++
                
                GUI.Label(new Rect(110,
                                   40+i*100,
                                   350,
                                   75
                                   ),"Shield | -0.8 BULLET DMG on hit, +2 health",InfoStyle);
                ++i;
            }


            //=================================================================
            GUI.Label(new Rect(Screen.width - 350,
                               Screen.height /2,
                               300,
                               150),"Left click to cycle through parts \n" +
                      "\n"+
                      "Right click to rotate weapon parts",InfoStyle);

        } 
        else
        {
            if (PlayerController.instance.playerUnit.isPlayerDead)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 15, 100, 30), "Remake Ship"))
                    Application.LoadLevel(0);

                if (GUI.Button(new Rect(Screen.width / 2 - 50, 3 * Screen.height / 4 - 15, 100, 30), "Restart level"))
                    Application.LoadLevel(Application.loadedLevelName);
            } 
            else
            {
                GUI.Box(new Rect((Screen.width - 
                    PlayerController.instance.playerUnit.health / PlayerController.instance.playerUnit.maxHealth * 200) / 2, 
                                 Screen.height - Screen.height/20, 
                         PlayerController.instance.playerUnit.health / PlayerController.instance.playerUnit.maxHealth * 200, Screen.height/20), 
                        PlayerController.instance.playerUnit.health.ToString("#.0"), HealthBarStyle);

//            GUI.Box(new Rect(Screen.width / 2 - 50, 0, 100, 20),
//                             "Time to next wave");

                GUI.Box(new Rect(Screen.width / 2 - 60, 0, 120, 40),
                    "Time to next wave \n" + (secToNextWave / 60).ToString("#00.") + " : " + (secToNextWave % 60).ToString("#00."));

                if (isCurrentWaveClear)
                    GUI.Box(new Rect(Screen.width / 2 - 150, 3 * Screen.height / 4 - 15, 300, 30), "Wave Clear, Health regain: " + healthGain);

            }
        }
    }
}
