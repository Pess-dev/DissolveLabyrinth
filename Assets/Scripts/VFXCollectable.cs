using UnityEngine;
using UnityEngine.VFX;


[RequireComponent(typeof(VisualEffect))]
public class VFXCollectable : MonoBehaviour
{

    [SerializeField] float velocityAtMin = 2f;
    [SerializeField] float velocityAtMax = 0.01f;

    [SerializeField] float sizeAtMin = 2f;
    [SerializeField] float sizeAtMax = 1f;

    VisualEffect visualEffect;

    void Awake(){
        visualEffect = GetComponent<VisualEffect>();
    }

    public void SetValues(float value){

        float velocity = Mathf.Lerp(velocityAtMin, velocityAtMax, value);
        float size = Mathf.Lerp(sizeAtMin, sizeAtMax, value);

        transform.localScale = new Vector3(size, size, size);
        visualEffect.SetFloat("Velocity", velocity);
    }
}
