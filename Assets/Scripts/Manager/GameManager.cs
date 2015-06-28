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
				_instance = GameObject.FindObjectOfType<GameManager> ();
			return _instance;
		}
	}

	public DrivingScriptBasic TheVehicle;

	public SmoothFollowCS CamFollow;
	public Transform CamFollowObject;

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
    public TrackBuildRuntime trbrtPrefab;

    TrackBuildRuntime trbrt;
    GameObject EnvironmentParent;

	[Range (1,100)]
	public float
		MinFollowDistance;
	[Range (1,100)]
	public float
		MaxFollowDistance;

	[Range (1,100)]
	public float
		MinFollowHeight;
	[Range (1,100)]
	public float
		MaxFollowHeight;

    [Space (10)]
    Vector3 CamFollowObjectOrigPosition;

    public int lastKnownSeed;

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
        }

        Debug.Log(Random.seed);

        //=========================================================

        seedInputField.text = lastKnownSeed.ToString();

        (TheVehicle as DrivingScriptStraight).turnSensitivity = PlayerPrefs.GetFloat("Sensitivity",1);

        tiltSensitivitySlider.normalizedValue = (TheVehicle as DrivingScriptStraight).turnSensitivity;

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

	}

    void LateUpdate ()
    {
        MenuManagement();

        CamManagement(CamFollow,CamFollowObject);
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

    public void CamManagement(SmoothFollowCS CamFollowScript,
                              Transform CamFollowObj)
    {
        float t = (TheVehicle.rigidBody.velocity.sqrMagnitude - TheVehicle.MinSpeed * TheVehicle.MinSpeed) / (TheVehicle.MaxSpeed * TheVehicle.MaxSpeed - TheVehicle.MinSpeed * TheVehicle.MinSpeed);
        float a = (TheVehicle as DrivingScriptStraight).LeftRightAcc;
        
        CamFollowScript.distance = Mathf.Lerp (MinFollowDistance, MaxFollowDistance, t);
        CamFollowScript.height = Mathf.Lerp (MinFollowHeight, MaxFollowHeight, t);
        
        CamFollowObj.transform.localPosition = Vector3.MoveTowards(CamFollowObj.transform.localPosition, CamFollowObjectOrigPosition + Vector3.right*a*(TheVehicle as DrivingScriptStraight).turnSensitivity*7,Time.deltaTime*7);
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
        (TheVehicle as DrivingScriptStraight).turnSensitivity = value;

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
