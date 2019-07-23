using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("J/Interactions/DragCancelTrigger")]
public class JDragCancelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        JDraggable CurrentDraggable = other.gameObject.GetComponent<JDraggable>();
        if (CurrentDraggable)
        {
            CurrentDraggable.CancelDragOperation();
        }
    }
}
