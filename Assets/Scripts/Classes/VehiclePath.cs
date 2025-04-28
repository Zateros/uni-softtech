using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


public class VehiclePath : IHeapItem<VehiclePath>
{
    private int _heapIndex;
    private float _satasfaction;
    private int _length;
    private List<Vector2> _path;
    private List<Vector2Int> _nodesSeen;

    public VehiclePath(VehiclePath path, List<Vector2> newPart)
    {
        _nodesSeen = path._nodesSeen;
        _path = path._path;
        _path.AddRange(newPart);
        foreach (Vector2 vec in _path)
        {
            Vector3Int pos = GameManager.Instance.GameTable.WorldToCell(vec);
            Vector2Int cord = new Vector2Int(pos.x, pos.y);
            if (_nodesSeen.Contains(cord))
            {
                _nodesSeen.Remove(cord);
            }
        }
        _nodesSeen.AddRange(path._nodesSeen);
        _satasfaction = CalculateSatasfaction(path);
    }

    private float CalculateSatasfaction(VehiclePath path)
    {
        return _nodesSeen.Count/_length;
    }

    public int HeapIndex
    {
        get
        {
            return _heapIndex;
        }
        set
        {
            _heapIndex = value;
        }
    }

    public int CompareTo(VehiclePath other)
    {
        int compare = _satasfaction.CompareTo(other._satasfaction);
        if (compare == 0)
        {
            compare = _length.CompareTo(other._length);
        }
        return -compare;
    }
}
