using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;

public class Tourist : MonoBehaviour, IEntity
{
    private readonly float _visionRange = 6f; //TODO: finallize
    private Vector2 _position;
    private readonly float _size; //TODO: finallize
    private GameObject _tourist;
    private int _satisfaction;
    private GameManager _game;

    public bool IsVisible { get => true; set => throw new Exception(); }
    public int Satisfaction { get => _satisfaction; private set => _satisfaction = value; }

    public Tourist(GameObject tourist, ref GameManager game)
    {
        _position = tourist.transform.position;
        _tourist = tourist;
        _game = game;

        //TODO: Change starting satisfaction based on difficulty
        Satisfaction = 50;
    }

    public void Update()
    {
        Move();
    }

    public void Move()
    {
        Vector2 path = GeneratePath();
    }
    public Vector2 GeneratePath()
    {
        throw new NotImplementedException();
    }
}
