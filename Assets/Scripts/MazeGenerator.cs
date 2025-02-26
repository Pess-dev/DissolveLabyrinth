using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Events;

public class MazeGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float prefabSize = 3f;

    public static MazeGenerator instance;

    public static UnityEvent mazeGenerated = new UnityEvent();
    

    public GameObject wallPrefab; // Префаб стены
    public GameObject floorPrefab; // Префаб пола
    public Transform mazeParent; // Родительский объект для лабиринта

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

    void Awake(){
        instance = this;
    }

    void Start()
    {
        GenerateMaze();
        //SaveMaze();
        SpawnMaze();

        
    }

    void SpawnMaze()
    {
        if (wallPrefab == null || floorPrefab == null || mazeParent == null)
        {
            Debug.LogError("Префабы стен, пола или родительский объект не назначены!");
            return;
        }

        // Создаем пол
        for (int x = 0; x < width+1; x++)
        {
            for (int y = 0; y < height+1; y++)
            {
                Vector3 position = new Vector3(x- width/2, 0, y - height/2)*prefabSize;
                Instantiate(floorPrefab, position, Quaternion.identity, mazeParent);
            }
        }

        // Создаем стены
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellPosition = new Vector3(x, 0, y)*prefabSize;

                // Северная стена
                if (grid[x, y].north)
                {
                    Vector3 wallPosition = cellPosition + new Vector3(0- width/2, 0, 0.5f- height/2)*prefabSize;
                    Instantiate(wallPrefab, wallPosition, Quaternion.identity, mazeParent);
                }

                // Южная стена
                if (grid[x, y].south)
                {
                    Vector3 wallPosition = cellPosition + new Vector3(0- width/2, 0, -0.5f- height/2)*prefabSize;
                    Instantiate(wallPrefab, wallPosition, Quaternion.identity, mazeParent);
                }

                // Восточная стена
                if (grid[x, y].east)
                {
                    Vector3 wallPosition = cellPosition + new Vector3(0.5f- width/2, 0, - height/2)*prefabSize;
                    Instantiate(wallPrefab, wallPosition, Quaternion.Euler(0, 90, 0), mazeParent);
                }

                // Западная стена
                if (grid[x, y].west)
                {
                    Vector3 wallPosition = cellPosition + new Vector3(-0.5f- width/2, 0, 0- height/2)*prefabSize;
                    Instantiate(wallPrefab, wallPosition, Quaternion.Euler(0, 90, 0), mazeParent);
                }
            }
        }
        //mazeParent.position = new Vector3(-0.5f, 0, -0.5f)*prefabSize*width;
        mazeGenerated.Invoke();
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
    }

    void InitializeGrid()
    {
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell();
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Check North
        if (cell.y < height - 1 && !grid[cell.x, cell.y + 1].visited)
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));

        // Check South
        if (cell.y > 0 && !grid[cell.x, cell.y - 1].visited)
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));

        // Check East
        if (cell.x < width - 1 && !grid[cell.x + 1, cell.y].visited)
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));

        // Check West
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
