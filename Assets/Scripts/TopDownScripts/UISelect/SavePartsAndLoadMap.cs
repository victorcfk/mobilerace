using UnityEngine;
using System.Collections;

public class SavePartsAndLoadMap : MonoBehaviour {

    //public ShipCoreInfoStore infoStore;

    void Start()
    {
    }


    // Draws 2 buttons, one with an image, and other with a text
	// And print a message when they got clicked.
    void OnGUI()
    {
        //if (!btnTexture)
        //{
        //    Debug.LogError("Please assign a texture on the inspector");
        //    return;
        //}
        //if (GUI.Button(new Rect(10, 10, 50, 50), btnTexture))
        //    Debug.Log("Clicked the button with an image");



        if (GUI.Button(
            new Rect(Screen.width - 80,
                    20,
                    60,
                    30), "Start"))
        { 
            
            Debug.Log("Clicked the button with text");

            ShipCoreInfoStore.instance.savePartsAndNewStage = true;

        }

        //if (GUI.Button(
        //new Rect(Screen.width - 80,
        //    50,
        //    60,
        //    30), "Test"))
        //{

        //    Debug.Log("Clicked the button with text");

        //    infoStore.GetPartsAndNewStage = true;

        //}


    }
}
