using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : State<EnemyController>
{
    private Animator animator;
    private HitStateController hitStateController;
    private IDamagable damagable;

    protected int hitTriggerHash = Animator.StringToHash("HitTrigger");

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        hitStateController = context.GetComponent<HitStateController>();
        damagable = context.GetComponent<IDamagable>();
    }

    public override void OnEnter()
    {        
        if (damagable == null)
        {            
            stateMachine.ChangeState<IdleState>();
            return;
        }

        hitStateController.enterHitHandler += OnEnterHitState;
        hitStateController.exitHitHandler += OnExitHitState;

        animator?.SetTrigger(hitTriggerHash);
    }

    public override void Update(float deltaTime)
    {
    }

    public override void OnExit()
    {
        hitStateController.enterHitHandler -= OnEnterHitState;
        hitStateController.exitHitHandler -= OnExitHitState;
    }

    public void OnEnterHitState()
    {        
    }

    public void OnExitHitState()
    {        
        stateMachine.ChangeState<IdleState>();
    }
}
