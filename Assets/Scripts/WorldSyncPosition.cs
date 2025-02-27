using UnityEngine;
using UnityEngine.Events;

public class WorldSyncPosition : MonoBehaviour
{
    public UnityEvent onSync = new UnityEvent();
    void FixedUpdate()
    {   
        Vector3 pos = WorldManager.GetNearestPoint(transform.position);

        if (pos!=transform.position) onSync.Invoke();

        transform.position = pos;
    }
}
