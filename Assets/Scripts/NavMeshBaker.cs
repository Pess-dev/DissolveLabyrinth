using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour
{
    
    NavMeshSurface navMeshSurface;
    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        MazeGenerator.mazeGenerated.AddListener(navMeshSurface.BuildNavMesh);
        
    }
}