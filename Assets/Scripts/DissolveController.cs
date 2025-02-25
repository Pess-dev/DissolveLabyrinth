using System;
using DG.Tweening;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    [SerializeField] float time = 1f;
    void Start()
    {
        Ability.activated.AddListener(Activate);
        Ability.deactivated.AddListener(Deactivate);
    }


    void Activate(){
        DOTween.To(() => Shader.GetGlobalFloat("_value"), x => Shader.SetGlobalFloat("_value", x), 1, time);
    }

    void Deactivate(){
        DOTween.To(() => Shader.GetGlobalFloat("_value"), x => Shader.SetGlobalFloat("_value", x), 0, time);
    }
}
