using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    public Button restartButton;
    public InputField seedInputField;
    public Slider tiltSensitivitySlider;

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
        seedInputField.text = Random.seed.ToString();
        tiltSensitivitySlider.normalizedValue = (TheVehicle as DrivingScriptStraight).turnSensitivity;
	}

    void LateUpdate ()
    {
        MenuManagement(menu);

        CamManagement(CamFollow,CamFollowObject);
    }

    public void MenuManagement(GameObject menuObj)
    {
        if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Space)) 
        {
            //Application.LoadLevel (0);
            if(menuObj.activeInHierarchy)
            {
                menuObj.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                menuObj.SetActive(true);
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
        Application.LoadLevel(0);
        Time.timeScale = 1;
    }

    public void sliderChanged(float value)
    {
//        Debug.Log(value);
        (TheVehicle as DrivingScriptStraight).turnSensitivity = value;
    }

    public void seedInputChanged(string value)
    {
//        Debug.Log(value);
        Random.seed = int.Parse(value);
    }

}
