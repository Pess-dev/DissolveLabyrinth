using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.Collections;

public class MazeGenerator : MonoBehaviour
{
    public int mazeSize = 10;
    public float prefabSize = 3f;

    public static MazeGenerator instance;
    public static UnityEvent mazeGenerated = new UnityEvent();
    public static UnityEvent mazeGeneratedForSpawners = new UnityEvent();

    public GameObject wallPrefab;
    public List<GameObject> wallSpecialPrefabs;
    public GameObject pillarPrefab;
    public GameObject largePillarPrefab;
    public GameObject floorPrefab;
    public List<GameObject> floorSpecialPrefabs;
    public List<GameObject> furniturePrefabs;
    public Transform mazeParent;

    public float largePillarChance = 0.1f;
    public float specialWallChance = 0.3f;
    public float specialFloorChance = 0.1f;
    public float objectChance = 0.1f;

    private Cell[,] grid;

    [System.Serializable]
    private class Cell
    {
        public bool north = true;
        public bool south = true;
        public bool east = true;
        public bool west = true;
        public bool visited = false;
    }

    void Awake()
    {
        instance = this;
        //mazeGenerated = new UnityEvent();
    }

    void OnDestroy(){
        mazeGenerated.RemoveAllListeners();
        mazeGeneratedForSpawners.RemoveAllListeners();
    }

    void Start()
    {
        GenerateMaze();
        SpawnMaze();

    }

    void SpawnMaze()
    {
        if (wallPrefab == null || floorPrefab == null || mazeParent == null)
        {
            Debug.LogError("Префабы или родительский объект не назначены!");
            return;
        }

        GameObject floorRandomed = floorPrefab;

        // Создание пола
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                Vector3 position = new Vector3(
                    x - mazeSize / 2f + 0.5f,
                    0,
                    y - mazeSize / 2f + 0.5f
                ) * prefabSize;
                floorRandomed = floorPrefab;
                if (Random.value<= specialWallChance) floorRandomed = floorSpecialPrefabs[Random.Range(0, floorSpecialPrefabs.Count)];
                Instantiate(floorRandomed, position, Quaternion.identity, mazeParent);
            }
        }

        GameObject wallRandomed;


        // Внутренние горизонтальные стены (северные)
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                if (grid[x, y].north)
                {
                    Vector3 wallPosition = new Vector3(
                        (x - mazeSize / 2f + 0.5f) * prefabSize,
                        0,
                        (y - mazeSize / 2f + 1f) * prefabSize
                    );
                    wallRandomed = wallPrefab;
                    if (Random.value <= specialWallChance) {
                        wallRandomed = wallSpecialPrefabs[Random.Range(0, wallSpecialPrefabs.Count)];
                        grid[x, y].north = false;
                        if(y<mazeSize)grid[x, y+1].south = false;
                    }

                    Instantiate(wallRandomed, wallPosition, Quaternion.Euler(0, 90, 0), mazeParent);
                }
            }
        }


        // Внутренние вертикальные стены (восточные)
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                if (grid[x, y].east)
                {
                    Vector3 wallPosition = new Vector3(
                        (x - mazeSize / 2f + 1f) * prefabSize,
                        0,
                        (y - mazeSize / 2f + 0.5f) * prefabSize
                    );
                    wallRandomed = wallPrefab;
                    if (Random.value <= specialWallChance) {wallRandomed = wallSpecialPrefabs[Random.Range(0, wallSpecialPrefabs.Count)];
                        grid[x, y].east = false;
                        if(x<mazeSize)grid[x+1, y].west = false;
                    }
                    Instantiate(wallRandomed, wallPosition, Quaternion.identity, mazeParent);
                }
            }
        }

        // Статичные предметы
        for (int x = 1; x < mazeSize - 1; x++)
        {
            for (int y = 1; y < mazeSize-1; y++)
            {
                GameObject furniturePrefab;
                Vector3 furniturePosition = new Vector3(
                        (x - mazeSize / 2f) * prefabSize,
                        0,
                        (y - mazeSize / 2f) * prefabSize
                );
                if (grid[x, y].north && Random.value<=objectChance){
                    Vector3 offsetPosition = furniturePosition + new Vector3(0.5f,0,1f)*prefabSize;
                    furniturePrefab = furniturePrefabs[Random.Range(0, furniturePrefabs.Count)];
                    Instantiate(furniturePrefab, offsetPosition, Quaternion.Euler(0, 0, 0), mazeParent).name = "north "+x+" "+y;

                }
                if (grid[x, y].south && Random.value<=objectChance)
                {
                    Vector3 offsetPosition = furniturePosition + new Vector3(0.5f,0,0f)*prefabSize;
                    furniturePrefab = furniturePrefabs[Random.Range(0, furniturePrefabs.Count)];
                    Instantiate(furniturePrefab, offsetPosition, Quaternion.Euler(0, 180, 0), mazeParent).name = "south "+x+" "+y;;
                }
                if (grid[x, y].east && Random.value<=objectChance)
                {
                    Vector3 offsetPosition = furniturePosition + new Vector3(1f,0,0.5f)*prefabSize;
                    furniturePrefab = furniturePrefabs[Random.Range(0, furniturePrefabs.Count)];
                    Instantiate(furniturePrefab, offsetPosition, Quaternion.Euler(0, 90, 0), mazeParent).name = "east "+x+" "+y;;
                }
                if (grid[x, y].west && Random.value<=objectChance)
                {
                    Vector3 offsetPosition = furniturePosition + new Vector3(0f,0,0.5f)*prefabSize;
                    furniturePrefab = furniturePrefabs[Random.Range(0, furniturePrefabs.Count)];
                    Instantiate(furniturePrefab, offsetPosition, Quaternion.Euler(0, 270, 0), mazeParent).name = "west "+x+" "+y;;
                }
            }
        }

        SpawnPillars();

        mazeGeneratedForSpawners.Invoke();
        mazeGenerated.Invoke();
    }

    void SpawnPillars()
    {
        if (pillarPrefab == null||largePillarPrefab==null)
        {
            Debug.LogError("Префаб столба не назначен!");
            return;
        }

        for (int x = 0; x <= mazeSize; x++)
        {
            for (int y = 0; y <= mazeSize; y++)
            {
                Vector3 pillarPosition = new Vector3(
                        (x - mazeSize / 2f) * prefabSize,
                        0,
                        (y - mazeSize / 2f) * prefabSize
                    );
                
                if (Random.value<=largePillarChance){   
                    Instantiate(largePillarPrefab, pillarPosition, Quaternion.identity, mazeParent);
                    continue;
                }

            // if (!((grid[x,y].east||grid[x,y].west||grid[x,y].north||grid[x,y].south)&&(x==0 &&!grid[x,y].south||y==0 &&!grid[x,y].west)))
                if ((x!=0 || grid[x,y].south)&&(y!=0 || grid[x,y].west)&&(y!=0||x!=0|| !(grid[x,y].west && grid[x,y].south)))
                {
                    
                    GameObject pillar = Instantiate(pillarPrefab, pillarPosition, Quaternion.identity, mazeParent);

                    pillar.name+=""+grid[x,y].north+grid[x,y].south+grid[x,y].east+grid[x,y].west;
                }

            }
        }
    }

    void GenerateMaze()
    {
        InitializeGrid();
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(0, 0);
        grid[current.x, current.y].visited = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWall(current, next);
                grid[next.x, next.y].visited = true;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }

        // for (int x = 0; x < mazeSize; x++)
        // for (int y = 0; y < mazeSize; y++){
        //     if (x == 0){
        //         grid
        //     }
        // }
    }

    void InitializeGrid()
    {
        grid = new Cell[mazeSize+1, mazeSize+1];
        for (int x = 0; x < mazeSize+1; x++)
        {
            for (int y = 0; y < mazeSize+1; y++)
            {
                grid[x, y] = new Cell();
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (cell.y <= mazeSize - 1 && !grid[cell.x, cell.y + 1].visited)
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));

        if (cell.y > 0 && !grid[cell.x, cell.y - 1].visited)
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));

        if (cell.x <= mazeSize - 1 && !grid[cell.x + 1, cell.y].visited)
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));

        if (cell.x > 0 && !grid[cell.x - 1, cell.y].visited)
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y));

        return neighbors;
    }

    void RemoveWall(Vector2Int current, Vector2Int next)
    {
        int dx = next.x - current.x;
        int dy = next.y - current.y;

        if (dx == 1)
        {
            grid[current.x, current.y].east = false;
            grid[next.x, next.y].west = false;
        }
        if (dx == -1)
        {
            grid[current.x, current.y].west = false;
            grid[next.x, next.y].east = false;
        }
        if (dy == 1)
        {
            grid[current.x, current.y].north = false;
            grid[next.x, next.y].south = false;
        }
        if (dy == -1)
        {
            grid[current.x, current.y].south = false;
            grid[next.x, next.y].north = false;
        }
    }
}