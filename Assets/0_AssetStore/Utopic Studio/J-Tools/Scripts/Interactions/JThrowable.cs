using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnValidateThrowSignature : UnityEvent<JThrowable>
{
}

[RequireComponent(typeof(JDraggable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
[AddComponentMenu("J/Interactions/Throwable")]
public class JThrowable : MonoBehaviour
{
    /// <summary>
    /// Called when the movement stops after throwing
    /// </summary>
    public UnityEvent OnThrowMovementStopped;

    /// <summary>
    /// Called after the throw movement stopped and passes this throwable object as an argument for any pending validation.
    /// </summary>
    public OnValidateThrowSignature OnValidateThrow;

    //Cached required variables
    private Rigidbody _rb;
    private JDraggable _draggable;
    private LineRenderer _lr;
    private float g;

    /// <summary>
    /// Impulse force used when throwing
    /// </summary>
    public float Impulse = 5.0f;

    /// <summary>
    /// Amount of time, in seconds, that the script waits before throwing the object after being dragged.
    /// </summary>
    public float Delay = 5.0f;

    /// <summary>
    /// Number of line segments to show for the arc render.
    /// </summary>
    public int RenderResolution = 10;

    /// <summary>
    /// Position before dragging and throwing
    /// </summary>
    private Vector3 OriginalPosition;

    //Control variables
    private bool bDragging = false;
    private float RemainingTime;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _draggable = GetComponent<JDraggable>();
        _lr = GetComponent<LineRenderer>();

        g = Mathf.Abs(Physics.gravity.y);

        if (_draggable)
        {
            _draggable.OnDragStart.AddListener(OnDragStart);
            _draggable.OnDragEnd.AddListener(OnDragEnd);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bDragging)
        {
            RemainingTime -= Time.deltaTime;

            if (RemainingTime <= 0.0f)
            {
                //Should apply force to the rigidbody, first we need to tell the draggable component to stop.
                _draggable.CommitDragOperation();

                _draggable.IsDraggable = false;
                StartCoroutine(MovementStopValidator());

                //Apply force on the direction we're looking
                _rb.AddForce(Camera.main.transform.forward * Impulse, ForceMode.Impulse);
            }
            else
            {
                //We're dragging but not yet ready to throw, show some UI
                RenderArc();
            }
        }
        else
        {
            //Disable line renderer
            _lr.enabled = false;
        }
    }

    void OnDragEnd()
    {
        bDragging = false;
    }

    void OnDragStart()
    {
        RemainingTime = Delay;
        bDragging = true;
        OriginalPosition = transform.position;
    }

    /// <summary>
    /// Cancels the throw and returns the object to where it was "pre-drag"
    /// </summary>
    public void RollbackThrow()
    {
        transform.position = OriginalPosition;
        _draggable.IsDraggable = true;
    }

    IEnumerator MovementStopValidator()
    {
        yield return new WaitForFixedUpdate();

        while (_rb.velocity.sqrMagnitude > 0.1f)
        {
            yield return new WaitForFixedUpdate();
        }

        if (OnThrowMovementStopped != null)
        {
            OnThrowMovementStopped.Invoke();
        }

        if (OnValidateThrow != null)
        {
            OnValidateThrow.Invoke(this);
        }
    }

    void RenderArc()
    {
        _lr.enabled = true;
        _lr.positionCount = RenderResolution + 1;
        _lr.SetPositions(GetArcPoints());
    }

    Vector3[] GetArcPoints()
    {
        Vector3[] Points = new Vector3[RenderResolution + 1];

        //Camera directional references
        Vector3 CameraForward = Camera.main.transform.forward;
        Vector3 CameraHorizontal = (new Vector3(CameraForward.x, 0.0f, CameraForward.z)).normalized;
        float DegAngle = Vector3.SignedAngle(CameraHorizontal, CameraForward, -Camera.main.transform.right);
        float RadAngle = Mathf.Deg2Rad * DegAngle;

        Debug.Log(DegAngle);

        float Velocity = Impulse / _rb.mass;
        Vector3 OffsetFromCamera = transform.position - Camera.main.transform.position;
        Quaternion LookRotation = Quaternion.LookRotation(CameraHorizontal, Vector3.up);

        //Projectile range
        float Distance = Velocity * Velocity * Mathf.Sin(2 * RadAngle) / g;
        Distance = Distance < 5.0f ? 5.0f : Distance;

        for (int i = 0; i <= RenderResolution; i++)
        {
            float t = (float)i / (float)RenderResolution;

            //Calculate on local space
            float x = t * Distance;
            float y = x * Mathf.Tan(RadAngle) - (g * x * x) / (2 * Mathf.Pow(Mathf.Cos(RadAngle) * Velocity, 2));

            //Obtain the point in local space, must be rotated towards the X,Z axis
            Vector3 localPoint = new Vector3(0.0f, y, x);

            Points[i] = LookRotation * localPoint + Camera.main.transform.position + OffsetFromCamera;
        }

        return Points;
    }
}
