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
            Shader.SetGlobalVector("_MainParams", new Vector4(Shader.GetGlobalVector("_MainParams").x,dissolve.fogDensityActivated,0,0));
            Shader.SetGlobalFloat("_value",1);
           // Debug.Log("I cant run in editor! "+DateTime.UtcNow);
            #endif
        }

        if (GUILayout.Button("Deactivate Dissolve"))
        {
            dissolve.Deactivate();
            #if UNITY_EDITOR
            Shader.SetGlobalVector("_MainParams", new Vector4(Shader.GetGlobalVector("_MainParams").x,dissolve.fogDensityDeactivated,0,0));
            Shader.SetGlobalFloat("_value",0);
            //Debug.Log("I cant run in editor! "+DateTime.UtcNow);
            #endif
        }

        if (GUILayout.Button("Disable"))
        {
            dissolve.Deactivate();
            #if UNITY_EDITOR
            Shader.SetGlobalVector("_MainParams", new Vector4(Shader.GetGlobalVector("_MainParams").x,0,0,0));
            Shader.SetGlobalFloat("_value",0);
            //Debug.Log("I cant run in editor! "+DateTime.UtcNow);
            #endif
        }
    }
}
