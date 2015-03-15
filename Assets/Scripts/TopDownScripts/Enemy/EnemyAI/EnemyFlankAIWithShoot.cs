using UnityEngine;
using System.Collections;

public class EnemyFlankAIWithShoot : EnemyFlankAI
{
    public bool isAttacking = true;
    public bool isLeadingTarget = true;

    override protected void Start()
    {
        if (MoveModule == null)
            MoveModule = GetComponent<EnemyMoveModuleBasic>();
        
        if (EnemyUnit == null)
            EnemyUnit = GetComponent<EnemyBasic>();
                
        if (Weapon == null)
            Weapon = GetComponent<WeaponBasic>();

        getToRandomSurroundingPoint();

        InvokeRepeating("getToRandomSurroundingPoint", destinationChangeInterval, destinationChangeInterval);
    }

    Vector3 pointToAttack;

    // Update is called once per frame
    public override void Update()
    {
        if (MoveModule.hasReachedDestination)
        {
            getToRandomSurroundingPoint();
        }
        
        //isAttacking;
        if (isAttacking)
        {
            if (isLeadingTarget)
                pointToAttack = LeadCalculator.FirstOrderInterceptPosition(this.gameObject, Weapon.projectileSpeed, Target);  //Attempt to lead the target
            else
                pointToAttack = Target.transform.position;

            MoveModule.LookToPoint(pointToAttack);

            if (Weapon != null && 
                (isSpamWeaponWhenOutOfRange ||
                isWithinRangeOfTarget)
                &&
                (isSpamWeaponWhenOutOfFacing || 
                MoveModule.isFacingPoint(pointToAttack))
                )
            {
                Weapon.FireWeapon(Target);
            }
        }
    }
}
