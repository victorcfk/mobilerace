using UnityEngine;
using System.Collections;

public abstract class ProjectileBasic : MonoBehaviour {

    public WeaponBasic weapon;

    //[HideInInspector]   //Assigned by weapon
    public float damage = 1;
    
    //[HideInInspector]   //Assigned by weapon
    public float range = 10;

    [HideInInspector]   //Assigned by weapon
    public float minSpeed = 10;

    [HideInInspector]   //Assigned by weapon
    public float lifeTime = 1;		// In seconds

    protected Vector3 prevPos;
    protected float distTravelled = 0;

    protected DamageReceiver receiver;

    public GameObject target;
    public ParticleSystem deathPS;



    // Use this for initialization
    protected virtual void Start()
    {
        prevPos = transform.position;

        if(lifeTime > 0)
            Invoke("DestroySelf", lifeTime);
        //if ( IsHoming == true && HomingLifeTime > 0 ) {
        //    Invoke ( "StopHoming" , HomingLifeTime );
        //}

        //Emitters = GetComponentsInChildren<ParticleEmitter>();
    }

    protected virtual void Update()
    {
        distTravelled += Vector3.Distance(prevPos, transform.position);
        prevPos = transform.position;

        if (distTravelled >= range && range > 0 )
            DestroySelf();
    }


    protected void DestroySelf()
    {
        //print("des");
        CancelInvoke();

        if (weapon != null)
            weapon.ClearUpProjectile(this);

        if (deathPS)
            Instantiate(deathPS, transform.position, transform.rotation);

        if (gameObject != null)
            Destroy(gameObject);

    }

    public float getCenter()
    {
        return this.renderer.bounds.extents.z;
    }

}
