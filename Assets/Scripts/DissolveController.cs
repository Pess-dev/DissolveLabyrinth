using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] public float fogDensityDeactivated = 0f;
    [SerializeField] public float fogDensityActivated = 0.5f;

    public static DissolveController instance;

    void Awake()
    {
        if (instance) return;
        instance = this;


        Ability.activated.AddListener(Activate);
        Ability.deactivated.AddListener(Deactivate);
        Deactivate();
    }

    [ContextMenu("Activate Dissolve")]
    public void Activate(){
        DOTween.To(() => Shader.GetGlobalVector("_MainParams").y,
            x => Shader.SetGlobalVector("_MainParams",
                new Vector4(Shader.GetGlobalVector("_MainParams").x, x, Shader.GetGlobalVector("_MainParams").z, Shader.GetGlobalVector("_MainParams").w)),
            fogDensityActivated, duration);
        ActivateShader();
    }
    public void ActivateShader(){
        DOTween.To(() => Shader.GetGlobalFloat("_value"), x => Shader.SetGlobalFloat("_value", x), 1, duration);
    }
    
    [ContextMenu("Deactivate Dissolve")]
    public void Deactivate()
    {
        DOTween.To(() => Shader.GetGlobalVector("_MainParams").y,
            x => Shader.SetGlobalVector("_MainParams",
                new Vector4(Shader.GetGlobalVector("_MainParams").x, x, Shader.GetGlobalVector("_MainParams").z, Shader.GetGlobalVector("_MainParams").w)),
            fogDensityDeactivated, duration);
        DeactivateShader();
    }
    public void DeactivateShader(){
        DOTween.To(() => Shader.GetGlobalFloat("_value"), x => Shader.SetGlobalFloat("_value", x), 0, duration);
    }


    public void OnDestroy(){
        Deactivate();
    }
}
