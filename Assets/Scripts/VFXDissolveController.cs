using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFXDissolveController : MonoBehaviour
{
    VisualEffect vfx;
    
    void Start(){
        vfx = GetComponent<VisualEffect>();
        Ability.activated.AddListener(Activate);
        Ability.deactivated.AddListener(Deactivate);
        Deactivate();
    }

    void Activate(){
        vfx.SetBool("isActive",true);
    }
    void Deactivate(){
        vfx.SetBool("isActive",false);
    }
}
