using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipCoreInfoStore : MonoBehaviour {

    private static ShipCoreInfoStore _instance;

    public static ShipCoreInfoStore instance
    {
        get
        {
            if (_instance == null)  _instance = GameObject.FindObjectOfType<ShipCoreInfoStore>();

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    public PlayerBasic ShipCore;
    public PlayerBasic ShipCorePrefab;

    public PartBuildController PieceControlPrefab;

    public List<BasicShipPart> listOfPartPrefabs;      //Reference to all the part things, needs to be initialized
    public List<BasicShipPart> AvailablePartsStage1;      //Reference to all the part things, needs to be initialized
    public List<BasicShipPart> AvailablePartsStage2;      //Reference to all the part things, needs to be initialized
    public List<BasicShipPart> AvailablePartsStage3;      //Reference to all the part things, needs to be initialized
    public List<BasicShipPart> AvailablePartsStage4;      //Reference to all the part things, needs to be initialized

    public List<PartInfo> listOfPartsChosen;
    public List<PartInfo> PartsChosenStage1;
    public List<PartInfo> PartsChosenStage2;
    public List<PartInfo> PartsChosenStage3;
    public List<PartInfo> PartsChosenStage4;

    public bool startFlightNow = false;
    public bool buildShipNow = false;
    public bool savePartsAndNewStage = false;

	// Use this for initialization. In dont destroy on load, this is only called once.
	void Start () {

        print("START!!!!!");

        //Destroy all other instances of shipcoreinfostore.
        if (ShipCoreInfoStore._instance != null)
        {
            if(GameObject.FindObjectsOfType<ShipCoreInfoStore>().Length > 0)
            {
                foreach(ShipCoreInfoStore g in GameObject.FindObjectsOfType<ShipCoreInfoStore>())
                {
                    if(g != ShipCoreInfoStore._instance)
                        GameObject.Destroy(g);
                }
            }
        }

        if (ShipCore == null) ShipCore = GameObject.FindObjectOfType<PlayerBasic>();

        //if(listOfPartInfo == null)  listOfPartInfo = new List<PartInfo>();

        DontDestroyOnLoad(this.gameObject);
	}

	// Update is called once per frame
	void Update ()
    {
        int listPtr = StageManager.combatStageToLoad;

        switch(listPtr)
        {
            case 1:
                listOfPartsChosen = PartsChosenStage1;
                listOfPartPrefabs = AvailablePartsStage1;
                break;

            case 2:
                listOfPartsChosen = PartsChosenStage2;
                listOfPartPrefabs = AvailablePartsStage2;
                break;

            case 3:
                listOfPartsChosen = PartsChosenStage3;
                listOfPartPrefabs = AvailablePartsStage3;
                break;

            case 4:
                listOfPartsChosen = PartsChosenStage4;
                listOfPartPrefabs = AvailablePartsStage4;
                break;

            default:
                listOfPartsChosen = PartsChosenStage1;
                listOfPartPrefabs = AvailablePartsStage1;
                break;
        }


        if (savePartsAndNewStage)
        {
            savePartsAndNewStage = !savePartsAndNewStage;

            if (listOfPartsChosen != null) saveAllPartInfo(listOfPartsChosen);
        }

        if (buildShipNow)
        {
            buildShipNow = !buildShipNow;

            if (ShipCore == null) ShipCore = GameObject.FindObjectOfType<PlayerBasic>();
            if (ShipCore == null) ShipCore = Instantiate(ShipCorePrefab) as PlayerBasic;

            if (listOfPartsChosen != null) loadAllPartInfo(listOfPartsChosen,ShipCore.gameObject,Application.loadedLevel == 0);
        }

        if (startFlightNow)
        {
            startFlightNow = false;

            ShipCore.initAllParts();
            ShipCore.initEngineSystem();
            ShipCore.initWeaponsSystem();
            ShipCore.initShieldSystem();
        }
	}

    void saveAllPartInfo(List<PartInfo> partList)
    {
        for (int i=0; i <partList.Count; i++)
        {
            partList[i].savePart(partList[i].partBuildControllerInst);
        }
    }
    
    void loadAllPartInfo(List<PartInfo> partList, GameObject ShipParent, bool isBuilderScene = false)
    {   
        for (int i=0; i <partList.Count; i++)
        {
            partList[i].loadPart(ShipParent,isBuilderScene);
        }
    }

//    void savePartInfoOnShipCore(PlayerBasic ShipCore)
//    {
//        BasicShipPart tempPart;
//
//        if (ShipCore == null) ShipCore = GameObject.FindObjectOfType<PlayerBasic>();
//
//        for (int i = 0; i < ShipCore.transform.childCount; i++)
//        {
//            tempPart = ShipCore.transform.GetChild(i).GetComponent<BasicShipPart>();
//
//            if(tempPart != null && tempPart.partType != ShipPartType.CORE)
//                listOfPartInfo.Add(new PartInfo(tempPart));
//        }
//    }

//    void loadPartInfoOnShipCore(PlayerBasic ShipCore, bool isBuilderScene = false)
//    {
//        //print(listOfPartsToAssignToShipCore.Count + " , " + listOfPartsRotations.Count + " , " + listOfPartPrefabs.Count);
//
//        //find the player, give him the parts;
//        //==========================================================================
//
//        for (int i = 0; i < listOfPartInfo.Count; i++)
//        {
//            BasicShipPart g = listOfPartInfo[i].loadPart();
//
//            if(isBuilderScene)
//            {
//                PartBuildController obj = Instantiate(PieceControlPrefab,listOfPartInfo[i].partLocalPosition,listOfPartInfo[i].partLocalRotation) as PartBuildController;
//                obj.transform.parent = ShipCore.transform;
//                
//                obj.CurrentPart = g;    
//            }
//
//            g.transform.parent = ShipCore.transform;
//
//            g.transform.localPosition = listOfPartInfo[i].partLocalPosition;
//            g.transform.localRotation = listOfPartInfo[i].partLocalRotation;
//
//        }
//
//    }
    
}
