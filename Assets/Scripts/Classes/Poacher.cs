using UnityEngine;
using System;

public class Poacher : MonoBehaviour, IEntity
{
    private int _visionRange;
    private Vector2 _position;
    private int _size;

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Animal CapturedAnimal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Poacher() { }

    public Vector2 GeneratePath()
    {
        throw new NotImplementedException();
    }


    public void CaptureAnimal(Animal animal)
    {
        throw new NotImplementedException();
    }

    public void ShootAnimal(Animal animal)
    {
        throw new NotImplementedException();
    }

    public void Move(Vector2 goal)
    {
        throw new NotImplementedException();
    }
}
