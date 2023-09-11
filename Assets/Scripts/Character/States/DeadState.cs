using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<EnemyController>
{
    private Animator animator;

    protected int IsAliveHash = Animator.StringToHash("IsAlive");

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator?.SetBool(IsAliveHash, false);
    }

    public override void Update(float deltaTime)
    {
        //if (stateMachine.ElapsedTimeInState > 3.0f)
        //{
        //    GameObject.Destroy(context.gameObject);
        //}
    }

    public override void OnExit()
    {
    }
}
