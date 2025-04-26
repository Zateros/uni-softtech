using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public interface IEntity
{
    public bool IsVisible { get; set; }

    /// <summary>
    /// A* with weights: 
    /// straigth: 10
    /// diagnal: 14
    /// hill * 2
    /// pond: impassable
    /// </summary>
    /// <param name="goal"> where the entity wants to go</param>
    public abstract void Move(Vector2 goal);

}