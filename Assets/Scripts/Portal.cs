using FastCampus.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    


    #region IInteractable Interface

    public float distance = 2.0f;
    public float Distance => distance;

    public void Interact(GameObject other)
    {
        Debug.Log(gameObject.name + "Interact with " + other.name);

        float calcDistance = Vector3.Distance(other.transform.position, transform.position);

        Debug.Log("calcDistance: " + calcDistance);

        if (calcDistance > Distance)
        {
            return;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StopInteract(GameObject other)
    { 
    
    }

    #endregion IInteractable Interface

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distance);        
    }
}
