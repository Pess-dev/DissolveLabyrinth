using DG.Tweening;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public bool reverse = false;
    public float period = 1.0f;

    void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 360 * (reverse?-1:1), 0), period, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);    
    }
}
