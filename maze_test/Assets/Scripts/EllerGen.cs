using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EllerGen : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject wallPrefab;
    public GameObject numberPrefab;

    [SerializeField]
    bool isShowNum = false;

    [SerializeField]
    bool isShowElem = false;

    [SerializeField]
    Vector2Int offset;
    [SerializeField]
    float wallOffset = 3.5f;

    private int[,] maze;
    private Dictionary<int, int> coordSetNums = new Dictionary<int, int>();
    private Dictionary<int, HashSet<int>> sets = new Dictionary<int, HashSet<int>>();

    void Start()
    {
        GenerateMaze();
        InstantiateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DestroyMap();
            GenerateMaze();
            InstantiateMaze();
        }
    }

    void DestroyMap()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    void GenerateMaze()
    {
        coordSetNums = new Dictionary<int, int>();
        sets = new Dictionary<int, HashSet<int>>();
        maze = new int[width, height];

        GameObject rowNums = new GameObject($"rowNumList");
        rowNums.transform.SetParent(transform);

        GameObject ElemList = new GameObject($"elementsList");
        ElemList.transform.SetParent(transform);

        // 초기화
        for (int x = 0; x < width; x++)
        {
            sets[x] = new HashSet<int> { x };
            coordSetNums[x] = x;
        }

        bool close = false;

        // 미로 생성
        for (int y = 0; y < height; y++)
        {
            // 가로 벽 처리
            for (int x = 0; x < width - 1; x++)
            {
                int curCell = coordSetNums[GetCellOrder(x, y)];
                int nextCell = coordSetNums[GetCellOrder((x + 1), y)];
                close = y < height - 1 ? Random.value > 0.5f : false;
                if (sets[curCell].SetEquals(sets[nextCell]) || close)
                {
                    maze[x, y] |= 1; // 오른쪽 벽 추가
                    maze[x + 1, y] |= 4; // 왼쪽 벽 추가
                }
                else
                {
                    MergeSets(x, x + 1, y);
                    curCell = coordSetNums[GetCellOrder(x, y)];
                    nextCell = coordSetNums[GetCellOrder((x + 1), y)];
                }
            }

            // 새로운 세트
            Dictionary<int, HashSet<int>> newSets = new Dictionary<int, HashSet<int>>();
            Dictionary<int, List<int>> elems = new Dictionary<int, List<int>>();
            int elemIndex = 0;
            // 세로 벽 처리
            if (y < height - 1)
            {
                foreach (var set in sets.Values)
                {
                    int Count = 0;
                    elems[elemIndex] = new List<int>();
                    for (int i = 0; i < set.Count; i++)
                    {
                        int curCell = set.ElementAt(i);
                        elems[elemIndex].Add(curCell);
                        int x = GetCoord(curCell)[0];
                        int upCell = curCell + width;

                        close = Random.value > 0.5f;

                        if (i == set.Count - 1 && Count == 0)
                        {
                            close = false;
                        }

                        if (!close) // 열림
                        {
                            Count++;
                            coordSetNums[upCell] = coordSetNums[curCell];
                        }
                        else // 닫힘
                        {
                            maze[x, y] |= 8; // 아래쪽 벽 추가
                            maze[x, y + 1] |= 2; // 위쪽 벽 추가

                            coordSetNums[upCell] = upCell;
                        }
                        if (!newSets.ContainsKey(coordSetNums[upCell]))
                        {
                            newSets[coordSetNums[upCell]] = new HashSet<int>();
                        }
                        newSets[coordSetNums[upCell]].Add(upCell);
                    }
                    elemIndex++;
                }
                sets.Clear();
                sets = newSets;
            }

            // TODO: 확인 결과 집합 내부의 셀 순서들이 중복됨
            if(isShowElem)
            {
                GameObject go = new GameObject($"Elements {y}");
                go.transform.SetParent(ElemList.transform);
                go.transform.localPosition = new Vector3(0, 0, y * offset.y);
                int count = 0;
                foreach (var e in elems)
                {
                    for (int i = 0; i < e.Value.Count; i++)
                    {
                        GameObject element = Instantiate(numberPrefab, go.transform);
                        element.transform.localPosition = new Vector3(GetCoord(e.Value[i])[0] * offset.x, 0, 0);
                        element.name = $"element {GetCoord(e.Value[i])[0]},{y}";

                        if (element.TryGetComponent(out ShowNumber show))
                        {
                            show.AllOnOff(false);
                            show.SetElement(count);
                        }
                    }
                    count++;
                }
            }

            if (isShowNum)
            {
                GameObject rowNum = new GameObject($"rowNum {y}");
                rowNum.transform.parent = rowNums.transform;
                rowNum.transform.localPosition = new Vector3(0, 0, y * offset.y);

                for (int x = 0; x < width; x++)
                {
                    GameObject num = Instantiate(numberPrefab, rowNum.transform);
                    num.transform.localPosition = new Vector3(x * offset.x, 0, 0);
                    num.name = $"num {x},{y}";

                    if (num.TryGetComponent(out ShowNumber shownum))
                    {
                        shownum.AllOnOff(false);
                        shownum.SetNumber(coordSetNums[GetCellOrder(x, y)]);
                        shownum.SetOrder(GetCellOrder(x, y));

                        if ((maze[x, y] & 1) == 0)
                        {
                            shownum.SetRight(true);
                        }
                        if ((maze[x, y] & 8) == 0)
                        {
                            shownum.SetUP(true);
                        }
                    }
                }
            }
        }
    }

    int GetCellOrder(int x, int y)
    {
        return y * width + x;
    }

    int[] GetCoord(int cell)
    {
        int x = cell % width;
        int y = cell / width;
        int[] coord = new int[2] { x, y };

        return coord;
    }

    void MergeSets(int x1, int x2, int currenY)
    {
        int curCoord = GetCellOrder(x1, currenY);
        int nextCoord = GetCellOrder(x2, currenY);

        int cur = coordSetNums[curCoord];
        int next = coordSetNums[nextCoord];
        // TODO: 수정 필요
        int smaller = Mathf.Min(cur, next);
        int larger = Mathf.Max(cur, next);
        sets[smaller].UnionWith(sets[larger]);
        //sets[larger] = sets[smaller];
        sets[larger].Clear();
        sets.Remove(larger);

        foreach (int i in sets[smaller])
        {
            coordSetNums[i] = smaller;
        }

        cur = coordSetNums[GetCellOrder(x1, currenY)];
        next = coordSetNums[GetCellOrder(x2, currenY)];
    }

    ///// <summary>
    ///// 사용 안함
    ///// </summary>
    ///// <param name="y"></param>
    ///// <returns></returns>
    //Dictionary<int, HashSet<int>> GetUniqueSets(int y)
    //{
    //    Dictionary<int, HashSet<int>> uniqueDic = new Dictionary<int, HashSet<int>>();
    //    HashSet<HashSet<int>> uniqueSets = new HashSet<HashSet<int>>();

    //    for (int x = 0; x < width; x++)
    //    {
    //        int cell = coordSetNums[GetCellOrder(x, y)];
    //        uniqueSets.Add(sets[cell]);
    //    }

    //    int i = 0;

    //    foreach (HashSet<int> set in uniqueSets)
    //    {
    //        uniqueDic.Add(i, set);
    //        i++;
    //    }

    //    return uniqueDic;
    //}

    void InstantiateMaze()
    {
        for (int y = 0; y < height; y++)
        {
            GameObject row = new GameObject($"row {y}");
            row.transform.parent = transform;
            row.transform.localPosition = new Vector3(0, 0, y * offset.y);

            for (int x = 0; x < width; x++)
            {
                GameObject tile = new GameObject($"tile {x},{y}");
                tile.transform.parent = row.transform;
                tile.transform.localPosition = new Vector3(x * offset.x, 0, 0);

                if (x == width - 1 || (maze[x, y] & 1) != 0) // 오른쪽 벽
                {
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(wallOffset, 0, 0);
                    obj.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    obj.name = $"Right {x},{y}";
                }

                if (y == 0 || (maze[x, y] & 2) != 0) // 아래쪽 벽
                {
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(0, 0, -wallOffset);
                    obj.name = $"Down {x},{y}";
                }

                if (x == 0 || (maze[x, y] & 4) != 0) // 왼쪽 벽
                {
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(-wallOffset, 0, 0);
                    obj.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    obj.name = $"Left {x},{y}";
                }

                if (y == height - 1 || (maze[x, y] & 8) != 0) // 위쪽 벽
                {
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(0, 0, wallOffset);
                    obj.name = $"Up {x},{y}";
                }
            }
        }
    }
}