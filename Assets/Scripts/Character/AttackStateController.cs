using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    public delegate void OnEnterAttackState();
    public OnEnterAttackState enterAttackHandler;

    public delegate void OnExitAttackState();
    public OnExitAttackState exitAttackHandler;

    public bool IsInAttackState
    {
        get;
        private set;
    }

    private void Start()
    {
        // Debug.Log("AttackStateController:Start");
        enterAttackHandler = new OnEnterAttackState(EnterAttackState);
        exitAttackHandler = new OnExitAttackState(ExitAttackState);
    }

    public void OnStartOfAttackState()
    {
        // Debug.Log("AttackStateController:OnStartOfAttackState");
        IsInAttackState = true;
        enterAttackHandler();
    }

    public void OnEndOfAttackState()
    {
        // Debug.Log("AttackStateController:OnEndOfAttackState");
        IsInAttackState = false;
        exitAttackHandler();
    }

    private void EnterAttackState()
    {   
        // Debug.Log("AttackStateController:EnterAttackState");
    }

    private void ExitAttackState()
    {
        // Debug.Log("AttackStateController:ExitAttackState");
    }

    public void OnCheckAttack(int attackIndex)
    {
        GetComponent<IAttackable>()?.OnExecuteAttack(attackIndex);
    }
}
