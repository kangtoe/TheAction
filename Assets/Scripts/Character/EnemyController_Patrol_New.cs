using FastCampus.QuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController_Patrol_New : EnemyController, IAttackable, IDamagable
{
    #region Variables
    public int id = 0;

    public Collider collider;
    public NavMeshAgent nav;
    public DissolveEffect dissolveEffect;

    public Transform hitPoint;
    public GameObject hitEffect = null;

    public Transform[] waypoints;

    public NPCBattleUI battleUI;

    public float maxHealth = 100f;
    public float health = 100f;

    private int hitTriggerHash = Animator.StringToHash("HitTrigger");
    private int isAliveHash = Animator.StringToHash("IsAlive");

    public override float AttackRange => CurrentAttackBehaviour?.range ?? 6.0f;

    #endregion Variables

    #region Proeprties

    public override bool IsAvailableAttack
    {
        get
        {
            if (!Target)
            {
                return false;
            }

            float distance = Vector3.Distance(transform.position, Target.position);
            return (distance <= AttackRange);
        }
    }

    #endregion Properties

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        stateMachine.AddState(new MoveState());
        stateMachine.AddState(new AttackState());
        stateMachine.AddState(new MoveToWaypointState());
        stateMachine.AddState(new HitState());
        stateMachine.AddState(new DeadState());

        health = maxHealth;

        //battleUI = GetComponent<NPCBattleUI>();
        if (battleUI)
        {
            battleUI.MinimumValue = 0.0f;
            battleUI.MaximumValue = maxHealth;
            battleUI.Value = health;
        }

        InitAttackBehaviour();
    }

    protected override void Update()
    {
        CheckAttackBehaviour();

        base.Update();
    }

    private void OnAnimatorMove()
    {
        // Follow NavMeshAgent
        //Vector3 position = agent.nextPosition;
        //animator.rootPosition = agent.nextPosition;
        //transform.position = position;

        // Follow CharacterController
        Vector3 position = transform.position;
        position.y = agent.nextPosition.y;

        animator.rootPosition = position;
        agent.nextPosition = position;

        // Follow RootAnimation
        //Vector3 position = animator.rootPosition;
        //position.y = agent.nextPosition.y;

        //agent.nextPosition = position;
        //transform.position = position;
    }


    #endregion Unity Methods

    #region Helper Methods

    private void InitAttackBehaviour()
    {
        foreach (AttackBehaviour behaviour in attackBehaviours)
        {
            if (CurrentAttackBehaviour == null)
            {
                CurrentAttackBehaviour = behaviour;
            }

            behaviour.targetMask = TargetMask;
        }
    }

    private void CheckAttackBehaviour()
    {
        //if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
        //{
        //    CurrentAttackBehaviour = null;

        //    foreach (AttackBehaviour behaviour in attackBehaviours)
        //    {
        //        if (behaviour.IsAvailable)
        //        {
        //            if ((CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.priority < behaviour.priority))
        //            {
        //                CurrentAttackBehaviour = behaviour;
        //            }
        //        }
        //    }
        //}

        if (CurrentAttackBehaviour?.IsAvailable == false) CurrentAttackBehaviour = null;
                    
        foreach (AttackBehaviour behaviour in attackBehaviours)
        {
            if (behaviour.IsAvailable)
            {
                if ((CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.priority < behaviour.priority))
                {
                    CurrentAttackBehaviour = behaviour;
                }
            }            
        }
    }

    #endregion Helper Methods

    #region IDamagable interfaces

    public bool IsAlive => (health > 0);

    public void TakeDamage(int damage, GameObject hitEffectPrefab)
    {
        //Debug.Log(gameObject.name + "TakeDamage : " + damage.ToString());

        if (!IsAlive)
        {
            return;
        }

        health -= damage;

        if (battleUI)
        {
            battleUI.Value = health;
            battleUI.TakeDamage(damage);
        }

        if (hitEffectPrefab)
        {
            Instantiate(hitEffectPrefab, hitPoint);
        }

        if (IsAlive)
        {
            //animator?.SetTrigger(hitTriggerHash);
            stateMachine.ChangeState<HitState>();            
        }
        else
        {
            Debug.Log(gameObject.name + " : die");

            if (battleUI != null)
            {
                battleUI.enabled = false;
            }

            QuestManager.Instance.ProcessQuest(QuestsubpointType.DestroyEnemy, id);
            
            // ﾃ豬ｹ ｰﾋｻ・ｲ・
            collider.enabled = false;
            nav.enabled = false;

            // ﾀﾏﾁ､ ｽﾃｰ｣ｵｿｾﾈ ｽﾃｰ｢ﾀ・ｺﾐﾇﾘ ﾈｿｰ・ ﾀﾌﾈﾄ ｻ霖ｦ
            dissolveEffect.Dissolve(6f);
            Destroy(gameObject, 6f);

            stateMachine.ChangeState<DeadState>();
        }
    }
    #endregion IDamagable interfaces

    #region IAttackable Interfaces


    [SerializeField]
    private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

    public AttackBehaviour CurrentAttackBehaviour
    {
        get;
        private set;
    }

    public void OnExecuteAttack(int attackIndex)
    {
        //Debug.Log("ExecuteAttack : target = " + Target.gameObject.name);
        if (CurrentAttackBehaviour != null && Target != null)
        {
            CurrentAttackBehaviour.ExecuteAttack(Target.gameObject);
        }
    }

    #endregion IAttackable Interfaces
}
