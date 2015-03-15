using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public PlayerBasic playerUnit;

    private float horizontalMoveInput;
    private float verticalMoveInput;
    private Vector3 LookAtVector;

    private bool isFiringInputActive;

    public static bool isAimingWithMouse = true;

    [HideInInspector]
    //public List<BasicShipPart> engines = new List<BasicShipPart>();

    //public GameObject controlledObj;


    private static PlayerController _instance;
    
    public static PlayerController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PlayerController>();
                
                //Tell unity not to destroy this object when loading a new scene!
                //DontDestroyOnLoad(_instance.gameObject);
            }
            
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


	// Use this for initialization
	void Start ()
    {
        if (playerUnit == null)
            GameObject.FindObjectOfType<PlayerBasic>();

	}
	
	// Update is called once per frame
    void Update()
    {
        //getAllEngineModifiers();

        //This works for game pad as well
        //================================================

        ProcessPlayerMovement();
        ProcessPlayerShooting();

        if(isAimingWithMouse)
            AimWithMouse();
        else
            AimWithGamePad();

        playerUnit.isFiringInputActive = this.isFiringInputActive;
        playerUnit.horizontalMoveInput = this.horizontalMoveInput;
        playerUnit.verticalMoveInput = this.verticalMoveInput;
    }


    void ProcessPlayerMovement()
    {
        this.horizontalMoveInput = Input.GetAxis("Horizontal");
        this.verticalMoveInput = Input.GetAxis("Vertical");
    }

    void ProcessPlayerShooting()
    {
        this.isFiringInputActive = Input.GetButton("Fire1");
    }

    void AimWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ray didn't hit any solid object, so return the 
        // intersection point between the ray and 
        // the Y=0 plane (horizontal plane)
        float t = -ray.origin.z / ray.direction.z;
        Vector3 rel = ray.GetPoint(t);

        //Vector3 currDir = playerUnit.transform.forward;
        //currDir.z = 0;

        Vector3 tarDir = rel - playerUnit.transform.position;
        tarDir.z = 0;

        playerUnit.LookAtVector = tarDir;
        
        //Vector3 newDir = Vector3.RotateTowards(currDir, tarDir, maxRadiansDelta, maxMagnitudeDelta);
        //newDir.z = 0;

        //playerUnit.transform.rotation = Quaternion.LookRotation(newDir, new Vector3(0, 0, -1));    //Force up to be -z to prevent flipping due to quaternion representation

    }

    void AimWithGamePad()
    {
//        if (Input.GetKey(KeyCode.JoystickButton0))
//            print(0);
//        if (Input.GetKey(KeyCode.JoystickButton1))
//            print(1);
//        if (Input.GetKey(KeyCode.JoystickButton2))
//            print(2);
//        if (Input.GetKey(KeyCode.JoystickButton3))
//            print(3);
//        if (Input.GetKey(KeyCode.JoystickButton4))
//            print(4);
//        if (Input.GetKey(KeyCode.JoystickButton5))
//            print(5);
//        if (Input.GetKey(KeyCode.JoystickButton6))
//            print(6);
//        if (Input.GetKey(KeyCode.JoystickButton7))
//            print(7);
//        if (Input.GetKey(KeyCode.JoystickButton8))
//            print(8);
//        if (Input.GetKey(KeyCode.JoystickButton9))
//            print(9);
//        if (Input.GetKey(KeyCode.JoystickButton10))
//            print(10);
//        if (Input.GetKey(KeyCode.JoystickButton11))
//            print(11);
//        if (Input.GetKey(KeyCode.JoystickButton12))
//            print(12);
//        if (Input.GetKey(KeyCode.JoystickButton13))
//            print(13);
//        if (Input.GetKey(KeyCode.JoystickButton14))
//            print(14);
//        if (Input.GetKey(KeyCode.JoystickButton15))
//            print(15);
//        if (Input.GetKey(KeyCode.JoystickButton16))
//            print(16);
//        if (Input.GetKey(KeyCode.JoystickButton17))
//            print(17);
//        if (Input.GetKey(KeyCode.JoystickButton18))
//            print(18);
//        if (Input.GetKey(KeyCode.JoystickButton19))
//            print(19);
//
//        print("X "+Input.GetAxis("AnalogStickX"));
//        print("Y "+Input.GetAxis("AnalogStickY"));
        
        playerUnit.LookAtVector = 
            new Vector3(
                Input.GetAxis("AnalogStickX"),
                -1*Input.GetAxis("AnalogStickY"),
                0);

    }

    //void AimWithKeyboard()
    //{
    //    Vector3 currDir = playerUnit.transform.forward;
    //    currDir.z = 0;

    //    Vector3 tarDir = new Quaternion(0, 0, 0, 0) * playerUnit.transform.forward; 
    //    tarDir.z = 0;

    //    Vector3 newDir = Vector3.RotateTowards(currDir, tarDir, maxRadiansDelta, maxMagnitudeDelta);

    //    //playerUnit.transform.rotation = Quaternion.LookRotation(newDir, new Vector3(0, 0, -1));    //Force up to be -z to prevent flipping due to quaternion representation

    //    playerUnit.transform.Rotate(Vector3.up, 1);
    //}

    //void getAllEngineModifiers()
    //{
    //    foreach (EngineBasic engine in playerUnit.engineSystem.engines)
    //    {
    //        maxRadiansDelta = (maxRadiansDelta + engine.turnSpeedAdd) * engine.turnSpeedMultiplier;
    //        maxMagnitudeDelta = (maxMagnitudeDelta + engine.turnSpeedAdd) * engine.turnSpeedMultiplier;
    //    }
    //}
}