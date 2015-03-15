using UnityEngine;
using System.Collections;

public class WeaponRaycast : WeaponBasic
{


    override public void FireWeapon(){

    }

    override public void FireWeapon(GameObject target){

    }

    /// <summary>
    /// Creates a projectile with properties to be initialised later. Raycast and particle weapons.
    /// </summary>
    /// <returns>The instance of the Projectile.</returns>
    protected virtual new GameObject CreateProjectile(Vector3 spawnLocation, Quaternion initialFacing)
    {
        return (GameObject)Instantiate(
            projectilePrefab,
            spawnLocation,
            initialFacing);

        //GameObject projectile;

        //projectile = ObjectPooling.instance.GetObjectForType(projectileName, true);
        //if (projectile == null)
        //{
        //    Debug.Log("SlotsController.cs CreateCards() - Cards Has NULL");
        //    projectile = ObjectPooling.instance.GetObjectForType(projectileName, false);
        //}
        //projectile.transform.position = spawnLocation;
        //projectile.transform.rotation = initialFacing;

        //return projectile;
    }
}
