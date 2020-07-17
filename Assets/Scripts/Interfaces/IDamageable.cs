using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(Weapon.DamageInfo[] damages, bool critical);
    float GetHealth();
    bool GetDodge(float attackValue, Actor attacker);
    bool GetBlock(float attackValue, Actor attacker);
    void Block(Actor attacker);
    void Dodge(Actor attacker);
    void Retaliate(IDamageable target);
}
