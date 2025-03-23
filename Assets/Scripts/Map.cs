using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Terrain[,] GameMap;

    public Terrain this[int x, int y]
    {
        get
        {
            return GameMap[x, y];
        }
        set
        {
            GameMap[x, y] = value;
        }
    }

    public Map() { }

    public void GenerateMap()
    {
        throw new NotImplementedException();
    }
}
