using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStateMachineBehavior : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<HitStateController>()?.OnStartOfHitState();
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<HitStateController>()?.OnEndOfHitState();
    }
}
