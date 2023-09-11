using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrailControl : MonoBehaviour
{
    public MeleeWeaponTrail trail;

    public void StartEmitt()
    {
        trail.Emit = true;
    }

    public void StopEmitt()
    {
        trail.Emit = false;
    }

}
