using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float minDistanceFromPlayer = 20f;
    [SerializeField] List<SpawnObject> enemies = new List<SpawnObject>();

    [SerializeField] bool withAbility = true;

    [Serializable]
    public class SpawnObject{
        [SerializeField]public GameObject prefab;
        [SerializeField]public int count;
    }
    void Awake(){
        MazeGenerator.mazeGeneratedForSpawners.AddListener(SpawnEnemies);
    }

    void SpawnEnemies(){
        List<GameObject> spawners = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("Spawner", spawners);
        //print(spawners.Count);
        spawners.RemoveAll(x => Vector3.Distance(x.transform.position, PlayerController.position) < minDistanceFromPlayer);
        //print(spawners.Count);
        foreach(SpawnObject enemy in enemies){
            for (int i = 0; i < enemy.count; i++){
                if (spawners.Count == 0) break;
                int num =UnityEngine.Random.Range(0, spawners.Count);
                GameObject enemyObject = Instantiate(enemy.prefab, 
                spawners[num].transform.position, Quaternion.identity);
                spawners.RemoveAt(num);
                if (!withAbility)
                    enemyObject.GetComponent<EnemyAI>().abilityRadius = 0;
            }
        }
    }
}
