using TMPro;
using UnityEngine;

public class DeathsUI : MonoBehaviour
{
    void Start()
    {
        GetComponent<TMP_Text>().text = "Deaths: " + GameManager.instance.deathCount;    
    }

}
