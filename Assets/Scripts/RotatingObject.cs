using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 오브젝트 시각효과 : 서서히 회전, 위 아래로 흔들림
public class RotatingObject : MonoBehaviour
{

    //public float shakeAmount = 0.25f; // 흔들리는 정도
    //public float shakeSpeed = 1f;
    public float rotateSpeed = 180f;


    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(ShakeCr());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0), Space.World);
    }

    // 위아래로 흔들기
    //IEnumerator ShakeCr() 
    //{
    //    float CurrentShake = 0;

    //    int multi = 1;
        
    //    while (true)
    //    {
    //        Debug.Log("CurrentShake: " + CurrentShake);

    //        float amount = shakeSpeed * Time.deltaTime * multi;
    //        CurrentShake += amount;
    //        transform.position += new Vector3(0, amount, 0);

    //        // 최대지점
    //        if (CurrentShake >= shakeAmount)
    //        {
    //            Debug.Log("go down");
    //            multi = -1;
    //        }

    //        // 최소지점
    //        else if (CurrentShake <= -shakeAmount)
    //        {
    //            Debug.Log("go up");
    //            multi = 1;
    //        }            

    //        yield return null;
    //    }       
    //}
}
