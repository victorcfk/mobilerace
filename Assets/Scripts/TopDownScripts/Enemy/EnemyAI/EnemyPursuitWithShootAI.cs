using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(WeaponBasic))]

public class EnemyPursuitWithShootAI : EnemyAIBasic{

    public bool isAttacking = true;
    public bool isLeadingTarget = true;
	
	// Update is called once per frame
    protected virtual new void Update()
    {
        if (Target != null) 
            HomeTowardsPoint(Target.transform.position);

        if (Weapon != null)
        {

            Vector3 pointToAttack;
            if (isLeadingTarget)
                pointToAttack = LeadCalculator.FirstOrderInterceptPosition(this.gameObject, Weapon.projectileSpeed, Target);
            else
                pointToAttack = Target.transform.position;


            //isAttacking;
            if (isAttacking)
            {
                MoveModule.LookToPoint(pointToAttack);  //Attempt to lead the target

                if ((isSpamWeaponWhenOutOfRange ||
                    isWithinRangeOfTarget)
                    &&
                    (isSpamWeaponWhenOutOfFacing || 
                    MoveModule.isFacingPoint(pointToAttack))
                    )
                    Weapon.FireWeapon(Target);
            }
        }
	}

//    protected void CleanUpProjectiles(){
//        foreach (ProjectileBasic proj in Weapon.ExistingProjectiles)
//        Destroy(proj);
//    }
//
//    protected virtual new void DestroySelf()
//    {
//        //ResetGame();
//        CleanUpProjectiles();
//
//        //gameObject.SetActive(false);
//        Destroy(gameObject);
//    }
}
