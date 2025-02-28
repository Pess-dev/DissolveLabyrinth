using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] public float fogDensityDeactivated = 0f;
    [SerializeField] public float fogDensityActivated = 0.5f;

    void Start()
    {
        Ability.activated.AddListener(Activate);
        Ability.deactivated.AddListener(Deactivate);
        Deactivate();
    }

    [ContextMenu("Activate Dissolve")]
    public void Activate()
    {
        DOTween.To(() => Shader.GetGlobalVector("_MainParams").y,
            x => Shader.SetGlobalVector("_MainParams",
                new Vector4(Shader.GetGlobalVector("_MainParams").x, x, Shader.GetGlobalVector("_MainParams").z, Shader.GetGlobalVector("_MainParams").w)),
            fogDensityActivated, duration);
        DOTween.To(() => Shader.GetGlobalFloat("_value"), x => Shader.SetGlobalFloat("_value", x), 1, duration);
    }

    [ContextMenu("Deactivate Dissolve")]
    public void Deactivate()
    {
        DOTween.To(() => Shader.GetGlobalVector("_MainParams").y,
            x => Shader.SetGlobalVector("_MainParams",
                new Vector4(Shader.GetGlobalVector("_MainParams").x, x, Shader.GetGlobalVector("_MainParams").z, Shader.GetGlobalVector("_MainParams").w)),
            fogDensityDeactivated, duration);
        DOTween.To(() => Shader.GetGlobalFloat("_value"), x => Shader.SetGlobalFloat("_value", x), 0, duration);
    }

    public void OnDestroy(){
        Deactivate();
    }
}
