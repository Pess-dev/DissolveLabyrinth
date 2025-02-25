using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(DissolveController))]
public class DissolveControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DissolveController dissolve = (DissolveController)target;

        if (GUILayout.Button("Activate Dissolve"))
        {
            dissolve.Activate();
            #if UNITY_EDITOR
            Debug.Log("I cant run in editor! "+DateTime.UtcNow);
            #endif
        }

        if (GUILayout.Button("Deactivate Dissolve"))
        {
            dissolve.Deactivate();
            #if UNITY_EDITOR
            Debug.Log("I cant run in editor! "+DateTime.UtcNow);
            #endif
        }
    }
}
