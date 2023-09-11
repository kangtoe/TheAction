using FastCampus.Core;
using FastCampus.InventorySystem.Inventory;
using FastCampus.InventorySystem.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
//using EPOOutline;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour, IAttackable, IDamagable
{
    //[SerializeField]
    //private InventoryObject equipment;

    [SerializeField]
    private InventoryObject inventory;

    #region Variables    

    private CharacterController controller;
    [SerializeField]
    private LayerMask groundLayerMask;

    [SerializeField]
    private LayerMask InteractableLayerMask;

    //internal void TakeDamage(int v)
    //{
    //    throw new NotImplementedException();
    //}

    private NavMeshAgent agent;
    private Camera camera;

    [SerializeField]
    private Animator animator;

    readonly int moveHash = Animator.StringToHash("Move");
    readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    readonly int fallingHash = Animator.StringToHash("Falling");
    readonly int attackTriggerHash = Animator.StringToHash("AttackTrigger");
    readonly int attackIndexHash = Animator.StringToHash("AttackIndex");
    readonly int hitTriggerHash = Animator.StringToHash("HitTrigger");
    readonly int isAliveHash = Animator.StringToHash("IsAlive");

    float gravity = -9.8f;

    [SerializeField]
    private LayerMask targetMask;
    public Transform target;

    public bool IsInAttackState => GetComponent<AttackStateController>()?.IsInAttackState ?? false;

    bool isAttackIntercept = false;
    Vector3? reservedPoint;
    Transform reserveTarget;

    [SerializeField]
    private Transform hitPoint;

    [SerializeField]
    public StatsObject playerStats;

    #endregion

    #region Main Methods
    // Start is called before the first frame update
    void Start()
    {
        inventory.OnUseItem += OnUseItem;

        controller = GetComponent<CharacterController>();

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = true;

        camera = Camera.main;
        
        playerStats.Health = playerStats.MaxHealth;
        playerStats.Mana = playerStats.MaxMana;

        InitAttackBehaviour();
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsAlive)
        {
            return;
        }

        CheckAttackBehaviour();
        
        bool isOnUI = false;

        // Make ray from screen to world
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        bool isPointerOver = EventSystem.current.IsPointerOverGameObject();
        if (!Physics.Raycast(ray, 100, InteractableLayerMask))
        {
            if (isPointerOver) isOnUI = true;
        }

        //Debug.Log("isOnUI: " + isOnUI);

        // 마우스 입력 처리
        if (!isOnUI && Input.GetMouseButtonDown(0)) //&& !IsInAttackState
        {
            //Debug.Log("Input.GetMouseButtonDown(0)");

            // Check hit from ray
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, groundLayerMask))
            {
                SetPicker(hit.point);

                //Debug.Log("We hit " + hit.collider.name + " " + hit.point);

                if (IsInAttackState )
                {
                    SetReservedPoint(hit.point);
                }
                else
                {                    
                    RemoveTarget();

                    // Move our player to what we hit
                    agent.SetDestination(hit.point);
                }
            }
        }
        else if (!isOnUI && Input.GetMouseButtonDown(1))
        {

            /*//단일 콜라이더 검사 -> 다중 콜라이더 검사로 개선
            // Check hit from ray
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log("We hit " + hit.collider.name + " " + hit.point);

                IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                if (damagable != null && damagable.IsAlive)
                {
                    SetTarget(hit.collider.transform, CurrentAttackBehaviour?.range ?? 0);

                    SetPicker(hit.collider.transform.position);
                }

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    SetTarget(hit.collider.transform, interactable.Distance);
                }
            }
            */

            RaycastHit[] hits = Physics.RaycastAll(ray, 100);
            foreach (RaycastHit hit in hits)
            {
                //Debug.Log("We hit " + hit.collider.name + " " + hit.point);
                
                // 자기 캐릭터 클릭 시 무시.
                if (hit.collider.gameObject == gameObject) continue;

                IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                if (damagable != null && damagable.IsAlive)
                {
                    SetPicker(hit.collider.transform.position);

                    if (IsInAttackState)
                    {
                        SetReservedTarget(hit.collider.transform);
                    }
                    else SetTarget(hit.collider.transform, CurrentAttackBehaviour?.range ?? 0);

                    break;
                }

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    SetPicker(hit.collider.transform.position);

                    if (IsInAttackState)
                    {
                        SetReservedTarget(hit.collider.transform);
                    }
                    else SetTarget(hit.collider.transform, interactable.Distance);

                    break;
                }
            }
        }

        // 타겟 공격, 상호작용 처리
        //if (target != null)
        if (target != null)
        {
            if (target.GetComponent<IInteractable>() != null)
            {
                float calcDistance = Vector3.Distance(target.position, transform.position);
                float range = target.GetComponent<IInteractable>().Distance;

                if (calcDistance > range)
                {
                    SetTarget(target, range);
                    //Debug.Log("IInteractable target range : " + range);
                }
                FaceToTarget();

            }
            else if (!(target.GetComponent<IDamagable>()?.IsAlive ?? false))
            {
                RemoveTarget();
            }
            else
            {
                float calcDistance = Vector3.Distance(target.position, transform.position);
                float range = CurrentAttackBehaviour?.range ?? 2.0f;

                if (calcDistance > range)
                {
                    SetTarget(target, range);
                }
                FaceToTarget();
            }
        }

        //if (agent.pathPending) Debug.Log("agent.pathPending");
        //Debug.Log("agent.remainingDistance : " + agent.remainingDistance.ToString());

        // 이동 처리
        if (agent.pathPending || (agent.remainingDistance > agent.stoppingDistance + 0.1f))
        //if ((agent.pathPending || (agent.remainingDistance > agent.stoppingDistance + 0.01f)) && !IsInAttackState)
        {
            controller.Move(agent.velocity * Time.deltaTime);
            animator.SetFloat(moveSpeedHash, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
            animator.SetBool(moveHash, true);
        }
        else
        {
            //controller.Move(agent.velocity * Time.deltaTime);
            controller.Move(Vector3.zero);

            //if (!agent.pathPending)
            //{
                animator.SetFloat(moveSpeedHash, 0);
                animator.SetBool(moveHash, false);
                agent.ResetPath();
            //}

            if (target != null)
            {
                if (target.GetComponent<IInteractable>() != null)
                {
                    
                    IInteractable interactable = target.GetComponent<IInteractable>();
                    interactable.Interact(gameObject);
                }
                else if (target.GetComponent<IDamagable>() != null)
                {
                    AttackTarget();
                }
            }
            else
            {
                //RemovePicker();
            }
        }

        // 중력 적용
        controller.Move(Vector3.up * gravity * Time.deltaTime);
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

    void OnDestroy()
    {
        inventory.OnUseItem -= OnUseItem;
    }
    #endregion Main Methods

    #region Helper Methods

    private void InitAttackBehaviour()
    {
        foreach (AttackBehaviour behaviour in attackBehaviours)
        {
            behaviour.targetMask = targetMask;
        }

        GetComponent<AttackStateController>().enterAttackHandler += OnEnterAttackState;
        GetComponent<AttackStateController>().exitAttackHandler += OnExitAttackState;
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

    void SetTarget(Transform newTarget, float stoppingDistance)
    {
        //Debug.Log("SetTarget : " + newTarget);

        target = newTarget;

        // xz평면상에서의 거리 측정
        Vector2 zxPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 xzTarget = new Vector2(newTarget.position.x, newTarget.position.z);
        float xzDist = Vector2.Distance(zxPos, xzTarget);
        // 실제 거리 - xz평면상에서의 거리 차이 계산
        float xyzDist = Vector3.Distance(transform.position, newTarget.position);
        float delta = xyzDist - xzDist;

        agent.stoppingDistance = stoppingDistance - 0.05f - delta;
        agent.updateRotation = false;
        agent.SetDestination(newTarget.transform.position);
        
    }
    
    public void RemoveTarget()
    {        
        target = null;
        agent.stoppingDistance = 0.00f;
        agent.updateRotation = true;

        agent.ResetPath();

        animator.ResetTrigger(attackTriggerHash);

        //Debug.Log("RemoveTarget");        
        //Time.timeScale = 0;
        
    }

    void SetReservedTarget(Transform _target)
    {
        Debug.Log("SetReservedTarget: " + _target);

        reserveTarget = _target;
        reservedPoint = null;
    }

    void SetReservedPoint(Vector3 _point)
    {
        Debug.Log("SetReservedPoint: " + _point.ToString());

        reserveTarget = null;
        reservedPoint = _point;        
    }

    private void SetPicker(Vector3 position)
    {
        ClickPointerManager.Instance.SetPointer(position);
    }

    void AttackTarget()
    {
        //Debug.Log("AttackTarget1");

        if (CurrentAttackBehaviour == null)
        {
            return;
        }

        if (target != null && !IsInAttackState && CurrentAttackBehaviour.IsAvailable)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= CurrentAttackBehaviour?.range)
            {
                animator.SetInteger(attackIndexHash, CurrentAttackBehaviour.animationIndex);
                animator.SetTrigger(attackTriggerHash);
                //Debug.Log("animator.SetTrigger(attackTriggerHash)");
            }
        }        
    }

    void FaceToTarget()
    {
        if (target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
        }
    }

    public void OnEnterAttackState()
    {
        UnityEngine.Debug.Log("OnEnterAttackState()");
        playerStats.AddMana(-3);
    }

    public void OnExitAttackState()
    {
        UnityEngine.Debug.Log("OnExitAttackState()");

        RemoveTarget();

        if (reservedPoint != null) agent.SetDestination(reservedPoint.Value);
        if (reserveTarget != null) target = reserveTarget;

        reservedPoint = null;
        reserveTarget = null;
    }

    // 사망 이후 처리
    IEnumerator DieAfter(float time)
    {
        yield return new WaitForSeconds(time);

        UIManager.Instance.DisplayDieUI();
    }

    #endregion Helper Methods

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
        if (CurrentAttackBehaviour != null)
        {
            if(!target.gameObject) Debug.Log("target: " + target + "||gameObject: " + target.gameObject);
            CurrentAttackBehaviour.ExecuteAttack(target.gameObject);
        }
    }

    #endregion IAttackable Interfaces

    #region IDamagable Interfaces

    public bool IsAlive => playerStats.Health > 0;

    public void TakeDamage(int damage, GameObject damageEffectPrefab)
    {
        //Debug.Log(gameObject.name + "TakeDamage : " + damage.ToString());

        if (!IsAlive)
        {
            return;
        }

        playerStats.AddHealth(-damage);

        if (damageEffectPrefab)
        {
            Instantiate<GameObject>(damageEffectPrefab, hitPoint);
        }

        if (IsAlive)
        {
            animator?.SetTrigger(hitTriggerHash);
        }
        else
        {
            animator?.SetBool(isAliveHash, false);

            StartCoroutine(DieAfter(3f));
        }
    }

    #endregion IDamagable Interfaces


    #region Inventory
    private void OnUseItem(ItemObject itemObject)
    {
        foreach (ItemBuff buff in itemObject.data.buffs)
        {
            if (buff.stat == AttributeType.Health)
            {
                Debug.Log("AddHealth : " + buff.value);
                playerStats.AddHealth(buff.value);
            }
        }

        if(itemObject.UsingEffectPrefab) Instantiate(itemObject.UsingEffectPrefab, gameObject.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            if (inventory.AddItem(new Item(item.itemObject), 1))
                Destroy(other.gameObject);
        }
    }

    public bool PickupItem(ItemObject itemObject, int amount = 1)
    {
        if (itemObject != null)
        {
            return inventory.AddItem(new Item(itemObject), amount);
        }

        return false;
    }

    public bool PickupItems(ItemObject[] itemObjects)
    {
        if (itemObjects == null || itemObjects.Length == 0) return false;

        // itemObject 배열 => item 배열 생성
        Item[] _items = new Item[itemObjects.Length];
        for (int i = 0; i < _items.Length; i++)
        {
            _items[i] = itemObjects[i].data;
        }

        return inventory.AddItem(_items);
    }

    #endregion Inventory
}