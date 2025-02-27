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

    public GameObject wallPrefab;
    public List<GameObject> wallSpecialPrefab;
    public GameObject pillarPrefab;
    public GameObject largePillarPrefab;
    public GameObject floorPrefab;
    public List<GameObject> floorSpecialPrefab;
    public Transform mazeParent;

    public float largePillarChance = 0.1f;
    public float specialWallChance = 0.3f;
    public float specialFloorChance = 0.1f;

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
                Instantiate(floorPrefab, position, Quaternion.identity, mazeParent);
            }
        }

        GameObject wallRandomed = wallPrefab;
        if (Random.value<= specialWallChance) wallRandomed = wallSpecialPrefab[Random.Range(0, wallSpecialPrefab.Count)];

        Vector3 offset = Vector3.zero;//new Vector3(1,0,1)*prefabSize/2;

        // Внутренние горизонтальные стены (северные)
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize - 1; y++)
            {
                if (grid[x, y].north)
                {
                    Vector3 wallPosition = new Vector3(
                        (x - mazeSize / 2f + 0.5f) * prefabSize,
                        0,
                        (y - mazeSize / 2f + 1f) * prefabSize
                    );
                    Instantiate(wallRandomed, wallPosition+offset, Quaternion.Euler(0, 90, 0), mazeParent);
                }
            }
        }

        // // Внешняя верхняя стена
        // for (int x = 0; x < mazeSize; x++)
        // {
        //     if (grid[x, mazeSize - 1].north)
        //     {
        //         Vector3 wallPosition = new Vector3(
        //             (x - mazeSize / 2f + 0.5f) * prefabSize,
        //             0,
        //             (mazeSize - mazeSize / 2f) * prefabSize
        //         );
        //         Instantiate(wallPrefab, wallPosition+offset, Quaternion.Euler(0, 90, 0), mazeParent);
        //     }
        // }

        // // Внешняя нижняя стена
        // for (int x = 0; x < mazeSize; x++)
        // {
        //     if (grid[x, 0].south)
        //     {
        //         Vector3 wallPosition = new Vector3(
        //             (x - mazeSize / 2f + 0.5f) * prefabSize,
        //             0,
        //             (-mazeSize / 2f) * prefabSize
        //         );
        //         Instantiate(wallPrefab, wallPosition+offset, Quaternion.Euler(0, 90, 0), mazeParent);
        //     }
        // }

        // Внутренние вертикальные стены (восточные)
        for (int x = 0; x < mazeSize - 1; x++)
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
                    Instantiate(wallPrefab, wallPosition+offset, Quaternion.identity, mazeParent);
                }
            }
        }

        // // Внешняя правая стена
        // for (int y = 0; y < mazeSize; y++)
        // {
        //     if (grid[mazeSize - 1, y].east)
        //     {
        //         Vector3 wallPosition = new Vector3(
        //             (mazeSize - mazeSize / 2f) * prefabSize,
        //             0,
        //             (y - mazeSize / 2f + 0.5f) * prefabSize
        //         );
        //         Instantiate(wallPrefab, wallPosition+offset, Quaternion.identity, mazeParent);
        //     }
        // }

        // // Внешняя левая стена
        // for (int y = 0; y < mazeSize; y++)
        // {
        //     if (grid[0, y].west)
        //     {
        //         Vector3 wallPosition = new Vector3(
        //             (-mazeSize / 2f) * prefabSize,
        //             0,
        //             (y - mazeSize / 2f + 0.5f) * prefabSize
        //         );
        //         Instantiate(wallPrefab, wallPosition+offset, Quaternion.identity, mazeParent);
        //     }
        // }
        SpawnPillars();

        mazeGenerated.Invoke();
    }

void SpawnPillars()
{
    if (pillarPrefab == null||largePillarPrefab==null)
    {
        Debug.LogError("Префаб столба не назначен!");
        return;
    }

    for (int x = 0; x < mazeSize; x++)
    {
        for (int y = 0; y < mazeSize; y++)
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
        else if (dx == -1)
        {
            grid[current.x, current.y].west = false;
            grid[next.x, next.y].east = false;
        }
        else if (dy == 1)
        {
            grid[current.x, current.y].north = false;
            grid[next.x, next.y].south = false;
        }
        else if (dy == -1)
        {
            grid[current.x, current.y].south = false;
            grid[next.x, next.y].north = false;
        }
    }
}