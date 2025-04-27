using UnityEngine;
using System;

public abstract class Herbivore : Animal
{
    public new void Awake()
    {
        base.Awake();
    }

    public override void Eat(IEntity e)
    {
        throw new NotImplementedException();
    }
}
