using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehaviour : AttackBehaviour
{
    public ManualCollision attackCollision;

    public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
    {
        //Debug.Log("ExecuteAttack");

        Collider[] colliders = attackCollision?.CheckOverlapBox(targetMask);

        foreach (Collider col in colliders)
        {
            //Debug.Log("col.gameObject: " + col.gameObject.name);
            col.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage, effectPrefab);
        }

        calcCoolTime = 0.0f;
    }
}
