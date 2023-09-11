using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPointerManager : MonoBehaviour
{
    #region Variables
    
    static ClickPointerManager instance;

    public GameObject clickPointer;
    public float surfaceOffset = 0.1f;
    //public float liveTime = 2.0f;

    #endregion Variables

    #region Properties

    public static ClickPointerManager Instance => instance;

    #endregion Properties    

    #region Unity Methods
    private void Awake()
    {
        instance = this;
    }
    #endregion Unity Methods

    public void SetPointer(Vector3 pos)
    {
        GameObject pointer = Instantiate(clickPointer);
        pointer.transform.position = pos + Vector3.up * surfaceOffset;

        //Destroy(pointer, liveTime);
    }
}
