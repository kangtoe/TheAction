using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 마우스 조작으로 카메라 시점을 조작
public class CameraInputControll : MonoBehaviour
{    
    TopdownCamera topdownCamera;

    void Start()
    {
        topdownCamera = GetComponent<TopdownCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        // 휠 조작
        float wheelScrol = Input.mouseScrollDelta.y; 
        if (wheelScrol > 0)
        {
            topdownCamera.height--;
        }   
        else if (wheelScrol < 0)    
        {
            topdownCamera.height++;
        }


        if (Input.GetMouseButton(2))
        {
            // 마우스 스크롤 조작
            float MouseScrolX = Input.GetAxis("Mouse X");
            if (MouseScrolX > 0)
            {
                topdownCamera.angle++;
            }
            else if (MouseScrolX < 0)
            {
                topdownCamera.angle--;
            }
        }
       
    }
}
