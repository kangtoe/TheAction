using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStateController : MonoBehaviour
{
    public delegate void OnEnterHitState();
    public OnEnterHitState enterHitHandler;

    public delegate void OnExitHitState();
    public OnExitHitState exitHitHandler;

    public bool IsInHitState
    {
        get;
        private set;
    }

    private void Start()
    {
        // Debug.Log("AttackStateController:Start");
        enterHitHandler = new OnEnterHitState(EnterHitState);
        exitHitHandler = new OnExitHitState(ExitHitState);
    }

    public void OnStartOfHitState()
    {
        // Debug.Log("AttackStateController:OnStartOfAttackState");
        IsInHitState = true;
        enterHitHandler();
    }
    public void OnEndOfHitState()
    {
        // Debug.Log("AttackStateController:OnEndOfAttackState");
        IsInHitState = false;
        exitHitHandler();
    }

    private void EnterHitState()
    {
        // Debug.Log("AttackStateController:EnterAttackState");
    }

    private void ExitHitState()
    {
        // Debug.Log("AttackStateController:ExitAttackState");
    }
}
