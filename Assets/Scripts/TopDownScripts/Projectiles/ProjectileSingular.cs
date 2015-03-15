using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]

public class ProjectileSingular: ProjectileBasic
{
    //internal const float DEFAULT_MAX_DISTANCE = 10000;

    //public int classtype = 1;
    public LayerMask LayersThatAreDamaged;
    public LayerMask LayersThatBlockSelf;

    protected BasicShipPart tempPart;

    //public LayerMask LayersThatDestroyProjectile;
    //public bool IsXPosRelativeToPlayer = false;
    
    //Homing functionality
    //=====================================
    public bool IsHoming = false; 

    public float maxRadiansDelta = 2.0f;
    //=====================================

    //Accelerating Functionality
    //=====================================
    public bool IsAccelerating = false;
    public float Acceleration = 1.1f;
    public float MaxSpeed = 100;
    //=====================================



    //internal CollisionTag TargetTagType;
    //private Transform Target;
    //protected ParticleEmitter[] Emitters;

    // Update is called once per frame
    protected virtual new void Update()
    {
        base.Update();

        if (IsAccelerating) AccelerateProjectile();
        if (IsHoming) HomeTowardsTarget(target);

        if(rigidbody != null)
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, minSpeed > MaxSpeed? minSpeed : MaxSpeed);

        /*
        if(MaxSpeed > MinSpeed)
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity,MaxSpeed);
        else
            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, MinSpeed);
        */

        //rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, MaxSpeed);

       // Target = FindClosestTargetByTag(TargetTagType);
       // HomeToTarget();

       // AccelerateProjectile();

        /*
		float DistanceMoved = Speed * Time.deltaTime;
		transform.position += transform.forward * DistanceMoved;
		Distance -= DistanceMoved;
        */

        /*
		if ( IsHoming == true && HomingDistance > 0 ) {
			HomingDistance -= DistanceMoved;
			if ( HomingDistance <= 0 ) {
				IsHoming = false;
			}
		}
        */
        // Destroy if exceeded maximum distance or out of view
        //if (
        //        (collider.bounds.max.x < Camera.WorldBounds.xMin ||
        //            collider.bounds.min.x > Player.Camera.WorldBounds.xMax ||
        //            collider.bounds.max.z < Player.Camera.WorldBounds.yMin ||
        //            collider.bounds.min.z > Player.Camera.WorldBounds.yMax))
        //{
        //    Kill();
        //}
    }

    void OnCollisionEnter(Collision collision)
    {
        print(name +" has collided with "+collision.gameObject.name);
        //tempPart = collision.gameObject.GetComponent<BasicShipPart>();

        //if (tempPart != null) tempPart.ApplyDamage(Damage);
        //else


        //Is it able to penetrate the layer?
        if ((LayersThatBlockSelf.value & 1 << collision.gameObject.layer) != 0)
        {
            //No.
            DestroySelf();
        }

        if ((LayersThatAreDamaged.value & 1 << collision.gameObject.layer) != 0)
        {
            receiver = collision.collider.gameObject.GetComponent<DamageReceiver>();

            if (receiver != null)
                receiver.ApplyDamage(damage);
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

//            prsint("collision "+ other.gameObject);
//            
//            print("collider "+other.gameObject.name);
//            print("receiver "+receiver.name);

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

    //// On Invisible we delete and remove from memory.
    //void OnBecameInvisible()
    //{
    //    //enabled = false;
    //    DestroySelf();
    //}

    //void OnBecameVisible()
    //{
    //    //enabled = true;
    //}

    protected void AccelerateProjectile()
    {
        rigidbody.AddForce(transform.forward * Acceleration,ForceMode.Acceleration);// *rigidbody.velocity;
    }

    protected void HomeTowardsTarget(GameObject target)
    {
        //rigidbody.velocity += Acceleration * (target.transform.position - gameObject.transform.position).normalized;

        //rigidbody.AddForce((target.transform.position - gameObject.transform.position).normalized * Acceleration , ForceMode.Acceleration);


        /*
        Vector3 currDir = controlledObj.transform.forward;
        currDir.z = 0;

        Vector3 tarDir = -controlledObj.transform.position + rel;
        tarDir.z = 0;*/

        Vector3 newDir = Vector3.RotateTowards(transform.forward, target.transform.position- transform.position, Mathf.Deg2Rad*maxRadiansDelta*Time.deltaTime, 0);
        newDir.z = 0;
        //transform.forward.z = 0;

        transform.rotation = Quaternion.LookRotation(newDir, new Vector3(0,1, 0));    //Force up to be -z to prevent flipping due to quaternion representation

        rigidbody.velocity = transform.forward*rigidbody.velocity.magnitude;
        //controlledObj.transform.localEulerAngles.Set(0, 0, controlledObj.transform.localEulerAngles.z);
    }
    
}