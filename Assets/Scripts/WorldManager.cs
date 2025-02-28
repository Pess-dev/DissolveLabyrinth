using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    float chunkSize;
    static List<Transform> chunkPool = new List<Transform>();
    Transform cornerChunk;
    //Transform currentChunk = null;
    bool ready = false;

    public static Vector3 offset {get; private set;} = Vector3.zero;

    public static UnityEvent chunksCreated = new UnityEvent();

    void Awake()
    {
        chunkSize = MazeGenerator.instance.prefabSize * MazeGenerator.instance.mazeSize;
        MazeGenerator.mazeGenerated.AddListener(InitializeChunkPool);
    }

    void InitializeChunkPool()
    {
        cornerChunk = transform.Find("Chunk");
        chunkPool.Add(cornerChunk);
        
        for (int i = 0; i < 3; i++)
        {
            GameObject chunk = Instantiate(cornerChunk.gameObject, transform);
            chunkPool.Add(chunk.transform);
        }
        PositionAllChunks();
        
        ready = true;
        chunksCreated.Invoke();
    }

    void Update()
    {
        if (ready)
            UpdateChunks();
        // currentChunk = NearestChunk(PlayerController.position);
        
        // if (currentChunk.localPosition != Vector3.zero){
        //     Vector3 delta = -currentChunk.localPosition;
        //     MoveInDelta(delta);
        //     GameObject[] movable = GameObject.FindGameObjectsWithTag("Movable");

        //     foreach (GameObject obj in movable)
        //     {
        //         obj.transform.position += delta;
        //     }
        // }
    }

    Transform NearestChunk(Vector3 position){
        Transform nearest = null;
        foreach (Transform chunk in chunkPool)
        {
            if (!nearest)
                nearest = chunk;
            else
            {
                if (Vector3.Distance(position, chunk.position) < Vector3.Distance(position, nearest.position))
                    nearest = chunk;
            }
        }
        return nearest;
    }

    void MoveInDelta(Vector3 delta)
    {
        delta = Vector3.ProjectOnPlane(delta, Vector3.up) / 2;
        foreach (Transform chunk in chunkPool)
            chunk.position += delta;
    }

    void UpdateChunks()
    {
        Vector3 playerPos = PlayerController.position;
        Vector3 chunkCenter = cornerChunk.position;
        
        float threshold = chunkSize / 2;
        Vector3 offset = playerPos - chunkCenter;
        offset -= new Vector3(1,0,1)* chunkSize / 2;
        //print(offset+" "+chunkCenter + " "+ playerPos+" "+threshold);
        
        int moveX = Mathf.Abs(offset.x) > threshold ? (int)Mathf.Sign(offset.x) : 0;
        int moveZ = Mathf.Abs(offset.z) > threshold ? (int)Mathf.Sign(offset.z) : 0;
        
        if (moveX != 0 || moveZ != 0)
        {
            RepositionChunks(moveX, moveZ);
        }
    }

    void RepositionChunks(int moveX, int moveZ)
    {
        Vector3 moveOffset = new Vector3(moveX * chunkSize, 0, moveZ * chunkSize);
        offset += moveOffset;
        foreach (Transform chunk in chunkPool)
        {
            chunk.position += moveOffset;
        }
        
    }

    void PositionAllChunks()
    {
        Vector3 center = cornerChunk.position;
        int index = 0;

        for (int x = 0; x <= 1; x++)
        {
            for (int z = 0; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue;
                
                chunkPool[index + 1].position = center + new Vector3(
                    x * chunkSize,
                    0,
                    z * chunkSize
                );
                index++;
            }
        }
    }

    public static Vector3 GetNearestPoint(Vector3 pos){
        Vector3 nearest = pos;
        Vector3 playerPos = PlayerController.position;

        Transform nativeChunk = chunkPool.OrderBy((x)=>Vector3.Distance(x.position,pos)).First();
        Vector3 offset = pos - nativeChunk.position;
        Transform targetChunk = chunkPool.OrderBy((x)=>Vector3.Distance(x.position+offset,playerPos)).First();
        
        if (nativeChunk == targetChunk) return pos;

        nearest = targetChunk.position + offset;

        return nearest;
    }
}
