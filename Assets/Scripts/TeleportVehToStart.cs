using UnityEngine;
using System.Collections;

public class TeleportVehToStart : MonoBehaviour {

    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            other.transform.position = (GameManager.instance.vehStartTransform.position);
            other.transform.rotation = (GameManager.instance.vehStartTransform.rotation);

            GameManager.instance.CamFollow.gameObject.transform.position = GameManager.instance.camStartTransform.position;
            GameManager.instance.CamFollow.gameObject.transform.rotation = GameManager.instance.camStartTransform.rotation;

            GameManager.instance.CamFollow.gameObject.transform.LookAt( other.transform.position );


//            other.transform.forward = Vector3.zero;
//            other.attachedRigidbody.velocity = other.attachedRigidbody.velocity.magnitude * Vector3.forward;
        }
    }
}
