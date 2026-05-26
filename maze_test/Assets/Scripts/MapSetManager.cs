using UnityEngine;

public class MapSetManager
{
    private int[,] map;

    public MapSetManager(int height, int width)
    {
        map = new int[height, width];

        for (int i = 0; i < width; i++)
        {
            map[0, i] = i;
        }
    }

    /// <summary>
    /// 현재 좌표 집합 번호 설정
    /// </summary>
    /// <param name="row">행</param>
    /// <param name="col">열</param>
    /// <param name="num">설정 번호</param>
    public void SetCoordSet(int row, int col, int num)
    {
        map[row, col] = num;
    }

    /// <summary>
    /// 현재 좌표에 해당하는 집합 번호
    /// </summary>
    /// <param name="row">현재 행</param>
    /// <param name="col">현재 열</param>
    /// <returns></returns>
    public int GetCoordSet(int row, int col)
    {
        return map[row, col];
    }
}
