using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 끌어서 오브젝트 이동 시키기
public class DragMove : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform moveObject; // 이동할 오브젝트

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 m_mousePosition;
    Vector3 mouseDelta; // 마우스와 오브젝트 중심 간 간격;
    Vector3 objectDelta; // 현재 오브젝트와 움직일 물체와의 간격.

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnMouseDown");

        mouseDelta = Input.mousePosition - transform.position;
        objectDelta = moveObject.position - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnMouseDrag");

        m_mousePosition = Input.mousePosition;
        moveObject.position = m_mousePosition - mouseDelta + objectDelta;
    }      

    //private void OnMouseDown()
    //{
    //    Debug.Log("OnMouseDown");

    //    delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    //}

    //private void OnMouseDrag()
    //{
    //    Debug.Log("OnMouseDrag");

    //    m_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    moveObject.position = m_mousePosition - delta;
    //}
}
