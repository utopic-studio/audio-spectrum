using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JResourceIndex))]
public class JResourceIndexInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        JResourceIndex Index = (JResourceIndex)target;
        if (GUILayout.Button("Bake ResourceIndex"))
        {
            Index.Bake();
        }
    }
}
