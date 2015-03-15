using UnityEngine;
using System.Collections;

public abstract class UnitBasic : MonoBehaviour {
    
    public float health = 10;
    public float maxHealth = 10;

    public abstract void ApplyDamage(float Damage);

    public abstract void DestroySelf();

}
