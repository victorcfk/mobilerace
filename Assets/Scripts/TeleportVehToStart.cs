using UnityEngine;
using System.Collections;

public class TeleportVehToStart : MonoBehaviour {

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            other.attachedRigidbody.MovePosition(GameManager.instance.vehStartTransform.position);
            other.attachedRigidbody.MoveRotation(GameManager.instance.vehStartTransform.rotation);

            GameManager.instance.CamFollow.gameObject.transform.position = GameManager.instance.vehStartTransform.position;

//            other.transform.forward = Vector3.zero;
//            other.attachedRigidbody.velocity = other.attachedRigidbody.velocity.magnitude * Vector3.forward;
        }
    }
}
