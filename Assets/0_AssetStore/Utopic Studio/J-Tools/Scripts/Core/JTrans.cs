using UnityEngine;

/// <summary>
/// Struct that simulates a struct that can be assigned or created in game without being a component.
/// </summary>
public struct JTrans
{
    public JTrans(Transform T)
    {
        position = T.position;
        rotation = T.rotation;
        scale = T.localScale;
    }

    /// <summary>
    /// World position
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// World rotation
    /// </summary>
    public Quaternion rotation;

    /// <summary>
    /// Local scale
    /// </summary>
    public Vector3 scale;
}
