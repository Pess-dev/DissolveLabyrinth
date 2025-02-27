using UnityEngine;

public class DistanceCulling : MonoBehaviour
{
    public float cullingDistance = 30f; // Расстояние, на котором стены будут отключаться
    private Transform cameraTransform;
    GameObject firstChild;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        firstChild = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        float distanceToCamera = Vector3.Distance(transform.position, cameraTransform.position);

        if (distanceToCamera > cullingDistance)
        {
            if (firstChild.activeSelf)
                firstChild.SetActive(false); // Отключить стену
        }
        else
        {
            if (!firstChild.activeSelf)
                firstChild.SetActive(true); // Включить стену
        }
    }
}