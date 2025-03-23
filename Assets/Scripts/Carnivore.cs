using UnityEngine;
using System;

public class Carnivore : Animal
{
    public Carnivore() { }

    public override void Eat(IEntity e)
    {
        throw new NotImplementedException();
    }

    public override Vector2 GeneratePath()
    {
        throw new NotImplementedException();
    }
}
