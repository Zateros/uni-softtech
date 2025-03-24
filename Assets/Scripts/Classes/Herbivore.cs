using UnityEngine;
using System;

public class Herbivore : Animal
{
    public new void Awake()
    {
        base.Awake();
    }

    public override void Eat(IEntity e)
    {
        throw new NotImplementedException();
    }

    public override Vector2 GeneratePath()
    {
        return UnityEngine.Random.onUnitSphere;
    }
}
