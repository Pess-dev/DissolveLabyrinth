using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderUpdater : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetSliderValue(float value){
        slider.value = value;
    }
}