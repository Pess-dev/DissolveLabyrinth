using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager instance;
    
    [SerializeField] float minDistanceFromPlayer = 5f;
    [SerializeField] int spawnAttemps = 10;
    [SerializeField] GameObject prefab;

    int collectedCount = 0;
    [SerializeField] int collectableRequired = 5;

    float progress = 0; 

    public UnityEvent collectedAll = new UnityEvent();
    public UnityEvent<float> progressChanged = new UnityEvent<float>();
    
    void Awake()
    {
        instance = this;
        collectedCount = 0;
        MazeGenerator.mazeGeneratedForSpawners.AddListener(SpawnCollectables);
    }
    void Start(){
        CheckCollectables();
    }
    
    void SpawnCollectables(){
        List<GameObject> spawners = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Spawner", spawners);
        
        spawners.RemoveAll(x => Vector3.Distance(x.transform.position, PlayerController.position) < minDistanceFromPlayer);

        List<Vector3> SpawnedAt = new List<Vector3>();

        for (int i = 0; i < collectableRequired; i++){
            Transform bestSpawner = null;
            float minDistnace = 0;
            if ( spawners.Count==0) Debug.LogError("There are no spawners!!!");
            List<GameObject> nonVisited = new List<GameObject>();
            nonVisited.AddRange(spawners);

            if (!bestSpawner){
                int num = Random.Range(0, nonVisited.Count);
                bestSpawner = nonVisited[num].transform;
                nonVisited.RemoveAt(num);
            }
            else 
            for (int j =0; j<spawnAttemps; j++){
                int num = Random.Range(0, nonVisited.Count);
                float minDistanceToOther = SpawnedAt.Min((x)=>Vector3.Distance(x,nonVisited[num].transform.position));
                if (minDistanceToOther>minDistnace){
                    minDistnace = minDistanceToOther;
                    bestSpawner = nonVisited[num].transform;
                    nonVisited.RemoveAt(num);
                }
            }

            if ( !bestSpawner) Debug.LogError("Best spawner is null!!!");
            
            spawners.Remove(bestSpawner.gameObject);

            GameObject CollectableObject = Instantiate(prefab, bestSpawner.position, Quaternion.identity);
            SpawnedAt.Add(CollectableObject.transform.position);
        }
    }

    public void AddCollectable(){
        collectedCount++;
        progress = (float)collectedCount / collectableRequired;
        progressChanged.Invoke(progress);
        CheckCollectables();
    }

    void CheckCollectables(){
        if (collectedCount == collectableRequired) collectedAll.Invoke();
    }
}
