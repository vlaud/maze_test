using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    LEFT,
    RIGHT,
    FRONT,
    BACK,
}

public class Tile : MonoBehaviour
{
    public GameObject[] walls;
    public bool[] wallStatus;
    public void UpdateWalls(bool[] status)
    {
        wallStatus = status;

        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].SetActive(!wallStatus[i]);
        }
    }
}
