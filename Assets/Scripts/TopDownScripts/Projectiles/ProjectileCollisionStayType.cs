using UnityEngine;
using System.Collections;

public class ProjectileCollisionStayType : ProjectileBasic{

    public LayerMask LayersThatAreDamaged;
    public LayerMask LayersThatBlockSelf;

    public float TriggerInterval = 0.1f;
    //private float TriggerTimer = 0;
    private bool isTriggerPossible = true;
    // Use this for initialization
    protected override void Start()
    {
        InvokeRepeating("EnableTrigger", 0, TriggerInterval);
    }

    protected override void Update()
    {
    }

    void EnableTrigger()
    {
        isTriggerPossible = true;
    }


    void OnCollisionEnter(Collision collision)
    {
        if ((LayersThatAreDamaged.value & 1 << collision.gameObject.layer) != 0)
        {
            receiver = collision.collider.gameObject.GetComponent<DamageReceiver>();

//            print("collision "+ collision.gameObject);
//
//            print("collider "+collision.collider.gameObject.name);
//            print("receiver "+receiver.name);

            if (receiver != null)
                receiver.ApplyDamage(damage);
        }

        //Is it able to penetrate the layer?
        if ((LayersThatBlockSelf.value & 1 << collision.gameObject.layer) != 0)
        {
            //No.
            DestroySelf();
        }    
    }

    void OnTriggerEnter(Collider other)
    {
        //print(name + " has triggered " + other.gameObject.name);
        
        //other.gameObject.BroadcastMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
        //tempPart = other.gameObject.GetComponent<BasicShipPart>();
        
        //if (tempPart != null) tempPart.ApplyDamage(Damage);
        //else
        if ((LayersThatAreDamaged.value & 1 << other.gameObject.layer) != 0)
        {
            receiver = other.gameObject.GetComponent<DamageReceiver>();

            if (receiver != null)
                receiver.ApplyDamage(damage);
        }
        
        
        //Is it able to penetrate the layer?
        if ((LayersThatBlockSelf.value & 1 << other.gameObject.layer) != 0)
        {
            //No.
            DestroySelf();
        }
    }


    void OnCollisionStay(Collision collision)
    {
        //print(name+"coli");
       // if (isTriggerPossible)
        {
            //isTriggerPossible = false;

           // print(name + " has collided with " + collision.gameObject.name);
            //tempPart = collision.gameObject.GetComponent<BasicShipPart>();

            //if (tempPart != null) tempPart.ApplyDamage(Damage);
            //else
            //Is it able to penetrate the layer?
            if ((LayersThatAreDamaged.value & 1 << collision.collider.gameObject.layer) != 0)
            {
                //No.
                receiver = collision.collider.gameObject.GetComponent<DamageReceiver>();

                print(receiver.name);

                if (receiver != null)
                    receiver.ApplyDamage(damage);
            }

            //Is it able to penetrate the layer?
            if ((LayersThatBlockSelf.value & 1 << collision.collider.gameObject.layer) != 0)
            {
                //No.
                DestroySelf();
            }

            
        }
    }
    void OnTriggerStay(Collider other)
    {
        //if (isTriggerPossible)
        {
           // isTriggerPossible = false;

            //print(name + " has triggered " + other.gameObject.name);

            //other.gameObject.BroadcastMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
            //tempPart = other.gameObject.GetComponent<BasicShipPart>();

            //if (tempPart != null) tempPart.ApplyDamage(Damage);
            //else
            if ((LayersThatAreDamaged.value & 1 << other.gameObject.layer) != 0)
            {
                receiver = other.gameObject.GetComponent<DamageReceiver>();

                print(receiver.name);

                if (receiver != null)
                    receiver.ApplyDamage(damage);
            }

            //Is it able to penetrate the layer?
            if ((LayersThatBlockSelf.value & 1 << other.gameObject.layer) != 0)
            {
                //No.
                DestroySelf();
            }

            //yield return new WaitForSeconds(TriggerInterval);
        }
    }
}
