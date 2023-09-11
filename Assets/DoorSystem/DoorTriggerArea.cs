
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorTriggerArea : MonoBehaviour
{
    public LayerMask TriggerLayers;

    public DoorEventObject doorEventObject;
    public DoorController doorController;

    public bool autoClose = true;

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & TriggerLayers) == 0) return;

        doorEventObject.OpenDoor(doorController.id);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //}

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer & TriggerLayers) == 0) return;

        if (autoClose)
        {
            doorEventObject.CloseDoor(doorController.id);
        }
    }
}
