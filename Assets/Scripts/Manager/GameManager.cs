using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TrackBuildRUtil;

public enum ControlSchemes
{
    TILT,
    SLIDER,
    BUTTON
}

public class GameManager : MonoBehaviour
{

	static GameManager _instance;

	//This is the public reference that other classes will use
	public static GameManager instance {
		get {
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if (_instance == null)
				_instance = FindObjectOfType<GameManager> ();
			return _instance;
		}
	}

    //==========================================


    public DrivingScriptStraight playerVehicle;

    public Camera MainCam;
    public Camera SubCam;
	public SmoothFollowCS CamFollow;
	public Transform CamFollowObject;

    public float TurnValue = 0;
    public float TurnSensitivity = 1;

    [Space (10)]

    //==============================================

    public GameObject[] TouchObjs;

    public GameObject menu;
    public GameObject eventSystem;
    public Button restartButton;
    public InputField seedInputField;
    public Slider tiltSensitivitySlider;
    public Slider controlSchemeSlider;

    //==============================================

    public ControlSchemes controlScheme;
    
    public float LeftPower;
    public float RightPower;
    
    public KeyCode LeftTurnCode;
    public KeyCode RightTurnCode;

    public TrackBuildRuntime trbrtPrefab;

    TrackBuildRuntime trbrt;
    GameObject EnvironmentParent;
    public GameObject BoostObj;

    //========================================
    //Cam movement values
    //========================================
	[Range (1,100)]
	public float MinFollowDistance;
	[Range (1,100)]
	public float MaxFollowDistance;

	[Range (-10,100)]
	public float MinFollowHeight;
	[Range (-10,100)]
	public float MaxFollowHeight;

    [Range (0,90)]
    public float MaxRotationAngle;
    //========================================
    float initMaxFollowDist;


    [Space (10)]
    Vector3 CamFollowObjectOrigPosition;

    public int lastKnownSeed;

    public Transform vehStartTransform;

	void Awake ()
	{
		_instance = this;

		if (CamFollowObject != null)
			CamFollow.camFollowTarget = CamFollowObject;

        CamFollowObjectOrigPosition = CamFollowObject.transform.localPosition;  //register the original location of the camObj

        menu.SetActive(false);
        Random.seed = PlayerPrefs.GetInt("Seed",1);

        lastKnownSeed = Random.seed;

        trbrt = GameObject.FindObjectOfType<TrackBuildRuntime>();
        EnvironmentParent = GameObject.Find("EnvironmentParent");

        Debug.Log(Random.seed);
        if (trbrt == null)
        {
            trbrt = Instantiate(trbrtPrefab);
            EnvironmentParent = new GameObject("EnvironmentParent");
            
            trbrt.Init();
            TrackManager.instance.PopulateEnvironment(EnvironmentParent);
            TrackManager.instance.PopulateBoosts(BoostObj);
        }

        Debug.Log(Random.seed);

        //=========================================================

        seedInputField.text = lastKnownSeed.ToString();

        TurnSensitivity = PlayerPrefs.GetFloat("Sensitivity",1);

        tiltSensitivitySlider.normalizedValue = TurnSensitivity;

        switch(PlayerPrefs.GetInt("ControlScheme",1))
        {
            case 1:
                controlScheme = (ControlSchemes.TILT);
                break;
            case 2:
                controlScheme = (ControlSchemes.SLIDER);
                break;
            case 3:
                controlScheme = (ControlSchemes.BUTTON);
                break;
        }

        controlSchemeSlider.value = PlayerPrefs.GetInt("ControlScheme",1);
        //=========================================================

        initMaxFollowDist = MaxFollowDistance;
	}

    void Update ()
    {
        ControlUpdates();
        MenuManagement();
        CamManagement();
    }

    public void ControlUpdates()
    {
#if UNITY_EDITOR
        if (Input.GetKey(RightTurnCode)) 
            TurnValue = 1;
        else
            if (Input.GetKey(LeftTurnCode)) 
            TurnValue = -1;
        else
            TurnValue = 0;

        TurnSensitivity = 0.75f;
#else
        switch(GameManager.instance.controlScheme)
        {
            case ControlSchemes.TILT:
                TiltControlUpdates();

                TurnSensitivity = 1;
                break;
            case ControlSchemes.SLIDER:
                LeftRightSlideControlUpdates();

                TurnSensitivity = 1;
                break;
            case ControlSchemes.BUTTON:
                LeftRightTapControlUpdates();

                TurnSensitivity = 0.75f;
                break;
        }
#endif
        //        {
        //            playerVehicle.isBraking = Input.anyKey;
//        }



        playerVehicle.LeftRightAcc = TurnSensitivity * TurnValue;
    }

    void TiltControlUpdates()
    {
        TurnValue = Input.acceleration.x;
    }
    
    void LeftRightSlideControlUpdates()
    {
        TurnValue = Mathf.Clamp( LeftPower - RightPower,-1,1);
    }
    
    void LeftRightTapControlUpdates()
    {
        if (RightPower == LeftPower)
        {
            TurnValue = 0;
            return;
        }
        
        if (RightPower > 0 && LeftPower == 0)
        {
            TurnValue = 1;
            return;
        }
        
        if (RightPower == 0 && LeftPower > 0)
        {
            TurnValue = -1;
            return;
        }
        
    }

    float thing = 0;
    float thing2 = 0;
    float intendedFOV;
    public void CamManagement()
    {
        CamFollow.MaxClampHeight = MaxFollowHeight;
        CamFollow.MinClampHeight = MinFollowHeight;

        if (playerVehicle.isSpeedBoosted)
            intendedFOV = 90;
        else
            intendedFOV = 75;

        MainCam.fieldOfView = SubCam.fieldOfView = 
                            Mathf.SmoothDamp( MainCam.fieldOfView,
                                              intendedFOV,
                                              ref thing,
                                               15 * Time.smoothDeltaTime);

        CamFollow.distance = Mathf.SmoothDamp(CamFollow.distance,
                                              Mathf.Lerp(MinFollowDistance,MaxFollowDistance, playerVehicle.rigidBody.velocity.sqrMagnitude/ (playerVehicle.MaxSpeed* playerVehicle.MaxSpeed)),
                                              ref thing2,
                                              15*Time.smoothDeltaTime);

//        CamFollow.MaxRotationAngle = MaxRotationAngle;
    }


    public void MenuManagement()
    {
        if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Space)) 
        {
            if(menu.activeInHierarchy)
            {
                menu.SetActive(false);
                eventSystem.SetActive(false);
                Time.timeScale = 1;

                foreach(GameObject g in TouchObjs)
                {
                    g.SetActive(true);
                }
            }
            else
            {
                menu.SetActive(true);
                eventSystem.SetActive(true);
                Time.timeScale = 0;

                foreach(GameObject g in TouchObjs)
                {
                    g.SetActive(false);
                }
            }
        }
    }

    public void restartButtonPressed()
    {
        Debug.Log(lastKnownSeed + " " + PlayerPrefs.GetInt("Seed",1));

        if (lastKnownSeed == PlayerPrefs.GetInt("Seed",1))
        {
            DontDestroyOnLoad(trbrt.gameObject);
            DontDestroyOnLoad(EnvironmentParent);
        } 
        else
        {
            GameObject.Destroy(trbrt.gameObject);
            GameObject.Destroy(EnvironmentParent);
        }

        Application.LoadLevel(Application.loadedLevel);
        Time.timeScale = 1;
    }

    public void sliderChanged(float value)
    {
        TurnSensitivity = value;

        PlayerPrefs.SetFloat("Sensitivity",value);
    }

    public void controlSchemeChanged(float value)
    {
        switch((int)value)
        {
            case 1:
                controlScheme = (ControlSchemes.TILT);
                break;
            case 2:
                controlScheme = (ControlSchemes.SLIDER);
                break;
            case 3:
                controlScheme = (ControlSchemes.BUTTON);
                break;
        }

        PlayerPrefs.SetInt("ControlScheme",(int)value);

    }

    public void seedInputChanged(string value)
    {
        int temp = int.Parse(value);

        PlayerPrefs.SetInt("Seed", temp);
    }

}
