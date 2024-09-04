using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour
{
    [SerializeField]
    GameObject Tile;

    [SerializeField]
    Vector2Int mazeSize;

    [SerializeField]
    Vector2Int offset;

    [SerializeField]
    List<Cell> board;
    Stack<int> cellStack = new Stack<int>();
    int boardCount = 0;

    [SerializeField]
    int currentCell; // Å½»ö À§Ä¡

    [SerializeField]
    List<Tile> tiles = new List<Tile>();

    private void Start()
    {
        CreateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CreateMaze();
        }
    }

    [ContextMenu("·£´ý ¹Ì·Î »ý¼º")]
    void CreateMaze()
    {
        DestroyMap();
        CreateBoard();
        RecursiveBacktracking();

        for (int y = 0; y < mazeSize.y; ++y)
        {
            GameObject row = new GameObject($"row {y}");
            row.transform.parent = transform;

            for (int x = 0; x < mazeSize.x; ++x)
            {
                GameObject obj = Instantiate(Tile, row.transform);
                obj.transform.localPosition = new Vector3(offset.x * x, 0, offset.y * y);
                obj.name = $"Tile {x},{y}";

                if (obj.TryGetComponent<Tile>(out var newRoom))
                {
                    newRoom.UpdateWalls(board[x + y * mazeSize.x].status);
                    tiles.Add(newRoom);
                }
            }
        }
    }

    void CreateBoard()
    {
        board = new List<Cell>();
        boardCount = 0;

        for (int y = 0; y < mazeSize.y; ++y)
        {
            for (int x = 0; x < mazeSize.x; ++x)
            {
                board.Add(new Cell());
            }
        }

        cellStack = new Stack<int>();
        cellStack.Push(Random.Range(0, board.Count));
    }

    void RecursiveBacktracking()
    {
        if (cellStack.Count <= 0 || boardCount >= board.Count)
        {
            Debug.Log($"{currentCell} finished");
            return;
        }

        if (boardCount == 0) Debug.Log($"firstCell: {currentCell}");

        currentCell = cellStack.Peek();

        if (!board[currentCell].visited)
        {
            board[currentCell].visited = true;
            boardCount++;
        }
        
        int nextCell = GetRandomNeighbour(currentCell);

        if (nextCell >= 0)
        {
            cellStack.Push(nextCell);
            UpdateNeighbours(currentCell, nextCell);
        }
        else
        {
            cellStack.Pop();
        }
        RecursiveBacktracking();
    }

    int GetRandomNeighbour(int current)
    {
        if (board.Count == 0) return 0;

        List<int> neighbours = new List<int>();
        int floor = mazeSize.x * mazeSize.y;

        //check forward neighbor
        if (current + mazeSize.x < floor && !board[current + mazeSize.x].visited)
        {
            neighbours.Add(current + mazeSize.x);
        }
        //check backward neighbor
        if (current - mazeSize.x >= 0 && !board[current - mazeSize.x].visited)
        {
            neighbours.Add(current - mazeSize.x);
        }
        //check left neighbor
        if (current % mazeSize.x != 0 && !board[current - 1].visited)
        {
            neighbours.Add(current - 1);
        }
        //check right neighbor
        if ((current + 1) % mazeSize.x != 0 && !board[current + 1].visited)
        {
            neighbours.Add(current + 1);
        }

        if (neighbours.Count <= 0) return -1;

        return neighbours[Random.Range(0, neighbours.Count)];
    }

    void UpdateNeighbours(int current, int next)
    {
        if (current + mazeSize.x == next) // forward
        {
            board[current].status[(int)Directions.FRONT] = true;
            board[next].status[(int)Directions.BACK] = true;
        }
        else if (current - mazeSize.x == next) // backward
        {
            board[current].status[(int)Directions.BACK] = true;
            board[next].status[(int)Directions.FRONT] = true;
        }
        else if (current - 1 == next) // left
        {
            board[current].status[(int)Directions.LEFT] = true;
            board[next].status[(int)Directions.RIGHT] = true;
        }
        else if (current + 1 == next) // right
        {
            board[current].status[(int)Directions.RIGHT] = true;
            board[next].status[(int)Directions.LEFT] = true;
        }
    }

    [ContextMenu("¸Ê »èÁ¦")]
    void DestroyMap()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}

[System.Serializable]
public class Cell
{
    public bool visited = false;
    public bool[] status = new bool[4];
}