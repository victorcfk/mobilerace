using UnityEngine;
using System.Collections;

public class EnemyAIBasic : MonoBehaviour
{
    public EnemyBasic EnemyUnit;
    public EnemyMoveModuleBasic MoveModule;
    public WeaponBasic Weapon;

    public bool isSpamWeaponWhenOutOfFacing = true;
    public bool isSpamWeaponWhenOutOfRange = true;

    protected bool isWithinRangeOfTarget
    { get { return Weapon.weaponFiringRange*Weapon.weaponFiringRange >= Vector3.SqrMagnitude(Target.transform.position - transform.position);}}

    protected bool isFacingTarget
    { get { return MoveModule.isFacingPoint(Target.transform.position);}}

    protected GameObject _Target;
    public GameObject Target 
    { 
        get {
            if (_Target == null){

                if(FindObjectOfType<PlayerBasic>())
                    _Target = FindObjectOfType<PlayerBasic>().gameObject;
                else
                    _Target =this.gameObject;   //return self to avoid exceptions
            }

            return _Target;
        }
        
        set { _Target = value; }
    }
    
    
    protected virtual void Start()
    {
        if (MoveModule == null)
            MoveModule = GetComponent<EnemyMoveModuleBasic>();

        if (EnemyUnit == null)
            EnemyUnit = GetComponent<EnemyBasic>(); 

        if (MoveModule == null)
            MoveModule = GetComponent<EnemyMoveModuleBasic>();

        if (Weapon == null)
            Weapon = GetComponent<WeaponBasic>();
    }
    
    // Update is called once per frame
    public virtual void Update()
    {
        if (Target != null) 
            HomeTowardsPoint(Target.transform.position);
    }
    
    public void HomeTowardsPoint(Vector3 position)
    {
        MoveModule.MoveToPoint(position);
    }
    
    protected void HomeTowardsTarget(GameObject target)
    {
        HomeTowardsPoint(target.transform.position);
    }
    
}
