using UnityEngine;
using System;

public class Tourist : MonoBehaviour, IEntity
{
    private int _visionRange;
    private Vector2 _position;
    private int _size;

    public bool IsVisible {  get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Satisfaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Move()
    {
        throw new NotImplementedException();
    }
    public void GeneratePath()
    {
        throw new NotImplementedException();
    }
}
