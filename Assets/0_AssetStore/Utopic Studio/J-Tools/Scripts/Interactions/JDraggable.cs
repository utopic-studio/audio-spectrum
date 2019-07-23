using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A Draggable component activates manually and keeps the target object attached to the user's front view, at a constant distance.
/// It can only be deactivated manually by other scripts.
/// </summary>
[AddComponentMenu("J/Interactions/Draggable")]
public class JDraggable : MonoBehaviour
{
    /// <summary>
    /// Called when we actually apply a drag operation.
    /// </summary>
    public UnityEvent OnDragged;

    /// <summary>
    /// Called when we start a drag operation.
    /// </summary>
    public UnityEvent OnDragStart;

    /// <summary>
    /// Called when we end a drag operation.
    /// </summary>
    public UnityEvent OnDragEnd;

    /// <summary>
    /// Current drag being performed, must be completed before going to other
    /// </summary>
    private static JDraggable CurrentDragOperation = null;

    /// <summary>
    /// Original transform struct, used to undo the movement if canceled.
    /// </summary>
    private JTrans OriginalTransform;

    /// <summary>
    /// Rotation relative to the main camera when first dragging the gameObject, maintained while dragging
    /// </summary>
    private Quaternion CameraRelativeQuat;

    /// <summary>
    /// Distance to put this object in front of the camera when dragging.
    /// </summary>
    public float DragDistance = 1.0f;

    /// <summary>
    /// Offset to put the pivot point of the dragged object, in the DragDistance anchor point.
    /// </summary>
    public Vector3 DragOffset = Vector3.zero;

    /// <summary>
    /// Smooth factor associated to rotating and moving the object
    /// </summary>
    public float LerpFactor = 0.1f;

    /// <summary>
    /// If this draggable behavior is enabled
    /// </summary>
    public bool IsDraggable = true;

    /// <summary>
    /// Optional rigidbody that needs to have its gravity deactivated when dragging.
    /// </summary>
    private Rigidbody _rb;

    /// <summary>
    /// Stored variable for the rigidbody that needs to be recovered after ending the drag operation.
    /// </summary>
    private bool _rbUsesGravity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb)
        {
            _rbUsesGravity = _rb.useGravity;
        }
    }

    public void SetDraggable(bool InDraggable)
    {
        IsDraggable = InDraggable;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentDragOperation == this)
        {
            Vector3 PreviousPosition = transform.position;

            //Process the new location
            Vector3 AnchorPoint = Camera.main.transform.position + Camera.main.transform.forward * DragDistance + DragOffset;
            Quaternion ObjectQuat = Camera.main.transform.rotation * CameraRelativeQuat;

            //We must apply a lerp
            Vector3 TargetPosition = Vector3.Lerp(transform.position, AnchorPoint, LerpFactor);
            Quaternion TargetRotation = Quaternion.Lerp(transform.rotation, ObjectQuat, LerpFactor);
            transform.position = TargetPosition;
            transform.rotation = ObjectQuat;

            if (OnDragged != null)
            {
                OnDragged.Invoke();
            }
        }
    }

    /// <summary>
    /// Starts a new drag operation on the gameobject owner of this component.
    /// Can fail if there's already another drag operation in place.
    /// </summary>
    public void BeginDragOperation()
    {
        if (IsDraggable && CurrentDragOperation == null)
        {
            CurrentDragOperation = this;
            OriginalTransform = new JTrans(gameObject.transform);
            CameraRelativeQuat = Quaternion.Inverse(Camera.main.transform.rotation) * gameObject.transform.rotation;

            if (_rb)
            {
                _rb.useGravity = false;
            }

            if (OnDragStart != null)
            {
                OnDragStart.Invoke();
            }
        }
    }

    /// <summary>
    /// Aborts the drag operation and return the object to its initial location
    /// </summary>
    public void CancelDragOperation()
    {
        if (CurrentDragOperation == this)
        {
            transform.position = OriginalTransform.position;
            transform.rotation = OriginalTransform.rotation;
            transform.localScale = OriginalTransform.scale;

            CurrentDragOperation = null;

            if (_rb)
            {
                _rb.useGravity = _rbUsesGravity;
            }

            if (OnDragEnd != null)
            {
                OnDragEnd.Invoke();
            }
        }
    }

    /// <summary>
    /// Finishes the drag operation, maintaining the latest world transform
    /// </summary>
    public void CommitDragOperation()
    {
        if (CurrentDragOperation == this)
        {
            CurrentDragOperation = null;

            if (_rb)
            {
                _rb.useGravity = _rbUsesGravity;
            }

            if (OnDragEnd != null)
            {
                OnDragEnd.Invoke();
            }
        }
    }
}
