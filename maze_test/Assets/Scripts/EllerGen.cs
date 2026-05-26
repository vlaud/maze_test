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
    Vector2Int offset;
    [SerializeField]
    float wallOffset = 3.5f;

    private UnionFind unionFind;
    private MapSetManager mapSetManager;
    private int[,] maze;

    void Start()
    {
        SetStarts();
        GenerateMaze();
        InstantiateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DestroyMap();
            SetStarts();
            GenerateMaze();
            InstantiateMaze();
        }
    }
    void SetStarts()
    {
        maze = new int[height, width];
        unionFind = new UnionFind(width);
        mapSetManager = new MapSetManager(height, width);
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
        bool close = false;

        // 1 2 4 8 -> 우 후 좌 전
        for (int y = 0; y < height; y++)
        {
            // 맨 오른쪽 -1 까지
            for (int x = 0; x < width - 1; x++)
            {
                // 현재 행 마지막 행이면 무조건 집합 연결
                close = y == height - 1 ? false : Random.value > 0.5f;

                // 현재 칸과 다음 칸의 집합 번호 가져오기
                int cur = mapSetManager.GetCoordSet(y, x);
                int next = mapSetManager.GetCoordSet(y, x + 1);
                
                // 집합이 같거나 닫기로 되어있으면 벽 비트 생성
                if (unionFind.find(cur) == unionFind.find(next) || close)
                {
                    maze[y, x] |= 1; // 오른쪽 벽
                    maze[y, x + 1] |= 4; // 왼쪽 벽
                    continue;
                }

                // 열기로 되어있으면 집합 통합
                unionFind.merge(cur, next);
            }

            // 현재 행이 마지막 행이면 끝내기
            if (y == height - 1) break;

            // 현재 행 모든 열들의 집합 위로 올리기 설정
            for (int x = 0; x < width; x++)
            {
                // 현재 좌표의 집합 번호
                int cur = mapSetManager.GetCoordSet(y, x);

                // 집합 번호의 루트 번호
                int root = unionFind.find(cur);

                // 루트 집합 사이즈
                int size = unionFind.GetSetSize(root);

                close = size == 1 ? false : Random.value > 0.5f;

                // 닫기 설정
                if (close)
                {
                    // 새 집합 번호 생성 = 집합 번호 최대 숫자 + 1
                    mapSetManager.SetCoordSet(y + 1, x, unionFind.MaxNum);
                    unionFind.CreateSet();
                    unionFind.SetSize(root, --size);
                    maze[y, x] |= 8;
                    maze[y + 1, x] |= 2;
                    continue;
                }

                // 열기 설정 시 다음 행 좌표 집합 번호를 루트로 설정
                mapSetManager.SetCoordSet(y + 1, x, root);
            }
        }
    }

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

                ShowNumber shn = Instantiate(numberPrefab, tile.transform).GetComponent<ShowNumber>();
                // 현재 좌표의 집합 번호
                int cur = mapSetManager.GetCoordSet(y, x);

                // 집합 번호의 루트 번호
                int root = unionFind.find(cur);
                shn.SetNumber(cur);
                shn.SetOrder(root);

                // 오른쪽 벽 생성
                if (x == width - 1 || (maze[y, x] & 1) != 0)
                {
                    shn.SetRight(false);
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(wallOffset, 0, 0);
                    obj.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    obj.name = $"Right {x},{y}";
                }

                // 뒷 벽 생성
                if (y == 0 || (maze[y, x] & 2) != 0)
                {
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(0, 0, -wallOffset);
                    obj.name = $"Backward {x},{y}";
                }

                // 왼쪽 벽 생성
                if (x == 0 || (maze[y, x] & 4) != 0)
                {
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(-wallOffset, 0, 0);
                    obj.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    obj.name = $"Left {x},{y}";
                }

                // 앞 벽 생성
                if (y == height - 1 || (maze[y, x] & 8) != 0)
                {
                    shn.SetUP(false);
                    GameObject obj = Instantiate(wallPrefab, tile.transform);
                    obj.transform.localPosition = new Vector3(0, 0, wallOffset);
                    obj.name = $"Forward {x},{y}";
                }
            }
        }
    }
}