using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshBaker : MonoBehaviour{
    NavMeshSurface navMeshSurface;
    void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    void Start(){
        WorldManager.chunksCreated.AddListener(navMeshSurface.BuildNavMesh);
        MazeGenerator.mazeGenerated.AddListener(()=>{transform.parent = transform.parent.GetChild(0);});
    }

}