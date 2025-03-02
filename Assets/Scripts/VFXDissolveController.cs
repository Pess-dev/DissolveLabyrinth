using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFXDissolveController : MonoBehaviour
{
    VisualEffect vfx;
    
    void Awake(){
        vfx = GetComponent<VisualEffect>();
        Deactivate();
    }
    void Start(){
        Ability.instance.activated.AddListener(Activate);
        Ability.instance.deactivated.AddListener(Deactivate);
        Deactivate();
    }

    void Activate(){
        vfx.SetBool("isActive",true);
    }
    void Deactivate(){
        vfx.SetBool("isActive",false);
    }
}
