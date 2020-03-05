using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(float damage, bool critical, Weapon.DamageTypesEnum damageType);
    float GetDodgeDefence();
    float GetBlockDefence();
    void Block(Actor attacker);
    void Dodge(Actor attacker);
}
