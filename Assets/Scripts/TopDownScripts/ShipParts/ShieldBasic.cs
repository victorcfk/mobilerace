using UnityEngine;
using System.Collections;

public class ShieldBasic : DamageReceiver
{
    public float dmgHardReduction = 1;
    public float dmgModifier = 0.5f;
    public float healthBoost = 5;

    public override void ApplyDamage(float Damage)
    {
        float temp = (Damage - dmgHardReduction);

        if(temp > 0)
            ParentReceiver.ApplyDamage( temp * dmgModifier);

        print("emit");
        particleSystem.Emit(5);
    }

}
