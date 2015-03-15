using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(DamageReceiver))]

public class BasicShipPart : MonoBehaviour {

    public ShipPartType partType;
    public bool isApplyToUnit = true;

    public UnitBasic shipCore;

    protected ParticleSystem activationPS;

	// Use this for initialization
    protected virtual void Start()
    {
        if (shipCore == null)
        {
            shipCore = this.GetComponent<UnitBasic>();
        }

        if (shipCore == null && transform.parent)
        {
            shipCore = transform.parent.GetComponent<UnitBasic>();
        }

        if (activationPS == null)
            activationPS = this.GetComponent<ParticleSystem>();
	}


    public virtual void ApplyDamage(float Damage = 1)
    {
        if (!isApplyToUnit)
            return;

        //print("APPLY!");
        if (shipCore != null) {
            shipCore.ApplyDamage(Damage);
        }

    }


    //void OnCollisionStay(Collision collision)
    //{
    //    //Is it able to penetrate the layer?
    //    //if ((LayersThatDestroyThis.value & 1 << collision.gameObject.layer) != 0)
    //    //{
    //        print(name + " has collided with " + collision.gameObject.name);

    //        collision.gameObject.BroadcastMessage("ApplyDamage", collisionDamage, SendMessageOptions.DontRequireReceiver);
    //    //}

    //}


    //void OnTriggerStay(Collider other)
    //{
    //    //Is it able to penetrate the layer?
    //    //if ((LayersThatDestroyThis.value & 1 << other.gameObject.layer) != 0)
    //    //{
    //        print(name + " has triggered " + other.gameObject.name);

    //        other.gameObject.BroadcastMessage("ApplyDamage", collisionDamage, SendMessageOptions.DontRequireReceiver);
    //    //}

    //}
}
