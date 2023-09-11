using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllerChracter : MonoBehaviour
{
    #region Variables

    private CharacterController characterController;
    private NavMeshAgent agent;
    private Camera myCamera;
    private Vector3 calcVelocity = Vector3.zero;

    [SerializeField]
    private float groundCheckDistance = 0.3f;

    [SerializeField]
    private LayerMask groundLayerMask;

    #endregion

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;   // chracterController의 이동 기능을 사용하기 때문에
        agent.updateRotation = true;

        myCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Process mouse left button input
        if(Input.GetMouseButton(0))
        {
            // make ray
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, groundLayerMask))
            {
                Debug.Log(hit.collider.name);
                agent.SetDestination(hit.point);
            }

            if(agent.remainingDistance > agent.stoppingDistance)
            {
                characterController.Move(agent.velocity * Time.deltaTime);
            }
            else
            {
                characterController.Move(Vector3.zero);
            }
        }
    }

    private void LateUpdate() 
    {
        transform.position = agent.nextPosition;
    }

    #endregion Unity Methods
}