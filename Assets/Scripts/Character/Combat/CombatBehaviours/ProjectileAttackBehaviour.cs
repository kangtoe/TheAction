using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackBehaviour : AttackBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;

    public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
    {
        //Debug.Log("ExecuteAttack");

        GameObject go = Instantiate(projectilePrefab);
        go.transform.position = firePoint.position;
        go.transform.LookAt(target.transform.position + Vector3.up);
        go.GetComponent<Projectile>().damage = damage;

        calcCoolTime = 0.0f;
    }
}
