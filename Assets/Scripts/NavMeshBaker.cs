using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour
{
    
    NavMeshSurface navMeshSurface;
    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        MazeGenerator.mazeGenerated.AddListener(()=>{transform.parent = transform.parent.GetChild(0);});
        WorldManager.chunksCreated.AddListener(navMeshSurface.BuildNavMesh);
        
    }

}