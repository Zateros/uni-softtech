using System;
using UnityEngine;

public interface IEntity
{
    public bool IsVisible { get; set; }

    public void Move();
    public Vector2 GeneratePath();
}
