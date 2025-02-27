using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    float minDistanceFromPlayer = 10f;
    [SerializeField] List<Tuple<GameObject, int>> enemies = new List<Tuple<GameObject, int>>();
    void Start()
    {
        MazeGenerator.mazeGenerated.AddListener(SpawnEnemies);

    }

    void SpawnEnemies(){
        List<GameObject> spawners = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Spawner", spawners);
        spawners.RemoveAll(x => Vector3.Distance(x.transform.position, PlayerController.position) < minDistanceFromPlayer);
         
        foreach(Tuple<GameObject, int> enemy in enemies){
            for (int i = 0; i < enemy.Item2; i++){
                if (spawners.Count == 0) break;
                int num =UnityEngine.Random.Range(0, spawners.Count);
                GameObject enemyObject = Instantiate(enemy.Item1, 
                spawners[num].transform.position, Quaternion.identity);
                spawners.RemoveAt(num);
            }
        }
    }
}
