using UnityEngine;
using System.Collections;

public class DestroyVehicleOnHit : MonoBehaviour {

    void OnCollisionEnter()
    {
        Application.LoadLevel(0);
    }
}
