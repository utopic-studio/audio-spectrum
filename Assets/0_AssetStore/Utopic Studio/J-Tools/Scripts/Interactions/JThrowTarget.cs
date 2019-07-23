using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnThrowableReachedTargetSignature : UnityEvent<JThrowable>
{
}

/// <summary>
/// Manages a throw area that reacts to specific UThrowable objects being thrown to it.
/// </summary>
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
[AddComponentMenu("J/Interactions/ThrowTarget")]
public class JThrowTarget : MonoBehaviour
{
    /// <summary>
    /// Called when a throwable reaches this target.
    /// </summary>
    public OnThrowableReachedTargetSignature OnThrowableReachedTarget;

    //Cached components
    private Renderer _renderer;
    private Collider _collider;

    /// <summary>
    /// Marks if the throw target has received a throwable object
    /// </summary>
    private bool _success = false;

    /// <summary>
    /// Color to change the material when triggering a success
    /// </summary>
    public Color SuccessColor = new Color(0.0f, 1.0f, 0.0f, 0.75f);
    

    // Start is called before the first frame update
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        JThrowable _throwable = other.GetComponent<JThrowable>();

        if (_throwable)
        {
            _success = true;
            _renderer.material.color = SuccessColor;

            if(OnThrowableReachedTarget != null)
            {
                OnThrowableReachedTarget.Invoke(_throwable);
            }
        }
    }

    /// <summary>
    /// Used by the throwable to validate the throw onto this target or rollback if it didn't reach
    /// </summary>
    /// <param name="InThrowable"></param>
    public void ValidateThrow(JThrowable InThrowable)
    {
        if (!_success)
        {
            InThrowable.RollbackThrow();
        }
    }
}
