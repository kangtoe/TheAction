using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToWaypointState : State<EnemyController>
{
    #region Variables

    private Animator animator;
    private CharacterController controller;
    private NavMeshAgent agent;

    //private EnemyController_Patrol patrolController;
    private EnemyController_Patrol_New patrolController;

    private Transform targetWaypoint = null;
    private int waypointIndex = 0;


    private int isMoveHash = Animator.StringToHash("IsMove");
    private int moveSpeedHash = Animator.StringToHash("MoveSpeed");

    float gravity = -9.8f;

    #endregion Variables

    #region Properties

    //private Transform[] Waypoints => ((EnemyController_Patrol)context)?.waypoints;
    private Transform[] Waypoints => ((EnemyController_Patrol_New)context)?.waypoints;

    #endregion Properties

    public override void OnInitialized()
    {
        animator = context.GetComponent<Animator>();
        controller = context.GetComponent<CharacterController>();
        agent = context.GetComponent<NavMeshAgent>();

        //patrolController = context as EnemyController_Patrol;
        patrolController = context as EnemyController_Patrol_New;
    }

    public override void OnEnter()
    {
        agent.stoppingDistance = 0.0f;

        if (targetWaypoint == null)
        {
            FindNextWaypoint();
        }

        if (targetWaypoint)
        {
            animator?.SetBool(isMoveHash, true);
            agent.SetDestination(targetWaypoint.position);
        }
        else
        {
            stateMachine.ChangeState<IdleState>();
        }
    }

    public override void Update(float deltaTime)
    {
        if (context.Target)
        {
            if (context.IsAvailableAttack)
            {
                // check attack cool time
                // and transition to attack state
                stateMachine.ChangeState<AttackState>();
            }
            else
            {
                stateMachine.ChangeState<MoveState>();
            }
        }
        else
        {
            controller.Move(Vector3.up * gravity * Time.deltaTime);

            if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance + 0.1f))
            {
                FindNextWaypoint();
                stateMachine.ChangeState<IdleState>();
            }
            else
            {
                controller.Move(agent.velocity * Time.deltaTime);
                animator.SetFloat(moveSpeedHash, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
            }
        }
    }

    public override void OnExit()
    {
        if (agent && agent.isActiveAndEnabled)
        {
            agent.stoppingDistance = context.AttackRange;
            agent.ResetPath();
        }
        
        animator?.SetBool(isMoveHash, false);        
    }

    public Transform FindNextWaypoint()
    {
        targetWaypoint = null;

        // Returns if no points have been set up
        if (Waypoints != null && Waypoints.Length > 0)
        {

            // Set the agent to go to the currently selected destination.
            targetWaypoint = Waypoints[waypointIndex];

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            waypointIndex = (waypointIndex + 1) % Waypoints.Length;
        }

        return targetWaypoint;
    }
}
