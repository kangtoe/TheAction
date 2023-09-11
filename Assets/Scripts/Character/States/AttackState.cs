using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    private Animator animator;
    private AttackStateController attackStateController;
    private IAttackable attackable;

    protected int attackTriggerHash = Animator.StringToHash("AttackTrigger");
    protected int attackIndexHash = Animator.StringToHash("AttackIndex");

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        attackStateController = context.GetComponent<AttackStateController>();
        attackable = context.GetComponent<IAttackable>();
    }

    public override void OnEnter()
    {
        //Debug.Log("AttackState:OnEnter");
        if (attackable == null || attackable.CurrentAttackBehaviour == null)
        {
            //Debug.Log("AttackState:OnEnter1");
            stateMachine.ChangeState<IdleState>();
            return;
        }

        attackStateController.enterAttackHandler += OnEnterAttackState;
        attackStateController.exitAttackHandler += OnExitAttackState;

        animator?.SetInteger(attackIndexHash, attackable.CurrentAttackBehaviour.animationIndex);
        animator?.SetTrigger(attackTriggerHash);
    }

    public override void Update(float deltaTime)
    {
    }

    public override void OnExit()
    {
        //Debug.Log("AttackState:OnExit");
        attackStateController.enterAttackHandler -= OnEnterAttackState;
        attackStateController.exitAttackHandler -= OnExitAttackState;
    }

    public void OnEnterAttackState()
    {
        // Debug.Log("OnEnterAttackState()");
    }

    public void OnExitAttackState()
    {
        // Debug.Log("OnExitAttackState()");
        stateMachine.ChangeState<IdleState>();
    }
}
