using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponProjectileSingular : WeaponBasic
{
    /*Commenting guidelines, left here for posterity
    /// <summary>
    /// Rotates the vehicle with the specified amount.
    /// </summary>
    /// <param name="Degrees">The amount, in degrees, to rotate the vehicle.</param>
    /// <returns>A Boolean value indicating a successful check.</returns>
    */
    public float angleDeviation = 0;

    //Burst Fire functionality
    //============================
//    public bool isBurstFire = false;
//    public float delayWithinBurst = 0.2f;       //Time spacing between each projectiles firing in burst
//    public int numOfProjectilesInBurst = 3;
    //============================

    //Multi shot functionality
    //============================
//    public bool isMultiShot = false;
//    public float FiringAngle = 90;       //Time spacing between each projectiles firing in burst
//    public int numOfProjectilesInMultiShot = 3;
    //============================

    override public void Awake()
    {
        base.Awake();
    }

    //override
    override public void FireWeapon()
    {
        if (coolDownTimer <= 0)
        {
            {
                LaunchProjectile();
            }

            coolDownTimer = coolDownBetweenShots;
        }
        else
            return;
    }

    public override void FireWeapon(GameObject Target)
    {
        this.target = Target;
        FireWeapon();
    }


    /// <summary>
    /// Create and Fire off the projectile, assuming it is created at the 
    /// Weapon's position and shares the same forward as the weapon
    /// </summary>
    protected void LaunchProjectile()
    {
        //print("centar"+projectilePrefab.getCenter());
        if (projectilePrefab != null)
        {

            for(int i=0; i < LaunchLocations.Length; i++){

                ProjectileBasic instance = CreateProjectile(LaunchLocations[i].position,LaunchLocations[i].rotation);

                //fireDirection = Quaternion.AngleAxis(Random.Range(-angleDeviation,angleDeviation),Vector3.forward) * LaunchLocations[i].forward;

                fireDirection = Vector3.up;
                Debug.Log(fireDirection);

				if(instance.rigidbody)
                	instance.rigidbody.velocity = fireDirection * projectileSpeed;

				if(instance.rigidbody2D)
					instance.rigidbody2D.velocity = fireDirection * projectileSpeed;

                instance.target = target;

            }
        }
    }
}