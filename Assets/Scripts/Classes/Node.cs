using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Node : IHeapItem<Node>
{

    public Vector2 worldPosition;
    public int gridX;
    public int gridY;
    public int weigth;
    public bool passible;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Node(int gridX, int gridY, int weigth)
    {
        worldPosition = new Vector2(gridX, gridY);
        this.gridX = gridX;
        this.gridY = gridY;
        this.weigth = weigth;
        passible = weigth != -1;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
