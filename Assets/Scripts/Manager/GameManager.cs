using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    public GameObject menu;
    public GameObject eventSystem;
    public Button restartButton;
    public InputField seedInputField;
    public Slider tiltSensitivitySlider;

    public ControlSchemes controlScheme;

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

	void Awake ()
	{
		_instance = this;

		if (CamFollowObject != null)
			CamFollow.camFollowTarget = CamFollowObject;

        CamFollowObjectOrigPosition = CamFollowObject.transform.localPosition;  //register the original location of the camObj

        menu.SetActive(false);

        //=========================================================
        if(PlayerPrefs.GetInt("ControlScheme",1) == 1)
            controlScheme = (ControlSchemes.TILT);
        else
            controlScheme = (ControlSchemes.SLIDER);

        seedInputField.text = PlayerPrefs.GetInt("Seed",1).ToString();

        (TheVehicle as DrivingScriptStraight).turnSensitivity = PlayerPrefs.GetFloat("Sensitivity",1);

        tiltSensitivitySlider.normalizedValue = (TheVehicle as DrivingScriptStraight).turnSensitivity;
        //=========================================================
        //seedInputField.text = Random.seed.ToString();
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
            //Application.LoadLevel (0);
            if(menu.activeInHierarchy)
            {
                menu.SetActive(false);
                eventSystem.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                menu.SetActive(true);
                eventSystem.SetActive(true);
                Time.timeScale = 0;
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
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel);
        Time.timeScale = 1;
    }

    public void sliderChanged(float value)
    {
//        Debug.Log(value);
        (TheVehicle as DrivingScriptStraight).turnSensitivity = value;

        PlayerPrefs.SetFloat("Sensitivity",value);
    }

    public void controlSchemeChanged(float value)
    {
        Debug.Log(value);

        if(value == 1)
        {
            controlScheme = (ControlSchemes.TILT);
            PlayerPrefs.SetInt("ControlScheme",1);
        }
        else
        {
            controlScheme = (ControlSchemes.SLIDER);
            PlayerPrefs.SetInt("ControlScheme",2);
        }
    }

    public void seedInputChanged(string value)
    {
//        Debug.Log(value);
        Random.seed = int.Parse(value);

        PlayerPrefs.SetInt("Seed",Random.seed);
    }

}
