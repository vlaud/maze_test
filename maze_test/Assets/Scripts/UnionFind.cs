using System.Collections.Generic;

public class UnionFind
{
    public UnionFind(int cols)
    {
        par = new Dictionary<int, int>();
        size = new Dictionary<int, int>();
        for (int i = 0; i < cols; ++i)
        {
            par[i] = i;
            size[i] = 1;
        }
        maxNum = cols;
    }

    public int find(int x)
    {
        while (x != par[x])
        {
            par[x] = par[par[x]];
            x = par[x];
        }
        return x;
    }

    public void merge(int a, int b)
    {
        a = find(a); b = find(b);

        if (a == b) return;

        if (size[a] < size[b]) (a, b) = (b, a);

        par[b] = a;
        size[a] += size[b];
    }

    public void CreateSet()
    {
        par[maxNum] = maxNum;
        size[maxNum] = 1;
        maxNum++;
    }

    public int GetSetSize(int x)
    {
        return size[x];
    }

    public void SetSize(int x, int newSize)
    {
        size[x] = newSize;
    }

    public int MaxNum => maxNum;
    private Dictionary<int,int> par;
    private Dictionary<int,int> size;
    private int maxNum;
}
