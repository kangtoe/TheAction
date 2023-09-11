using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EPOOutline;

public class OutlineCheck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outlinable outlinable;

    // Start is called before the first frame update
    void Start()
    {
        outlinable = GetComponent<Outlinable>();
        outlinable.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    Debug.Log(gameObject + " : IsPointerOverGameObject");
        //    outlinable.enabled = true;
        //}
        //else
        //{
        //    Debug.Log(gameObject + " : !IsPointerOverGameObject");
        //    outlinable.enabled = false;
        //}

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Check hit from ray
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            //Debug.Log(hit.collider.name);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log(gameObject + " : OnPointerEnter");
        outlinable.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log(gameObject + " : OnPointerExit");
        outlinable.enabled = false;
    }

    private void OnMouseEnter()
    {
        //Debug.Log("OnMouseEnter");
        //outlinable.enabled = true;
    }

    private void OnMouseExit()
    {
        //Debug.Log("OnMouseExit");
        //outlinable.enabled = false;
    }
}
