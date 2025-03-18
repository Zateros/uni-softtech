using UnityEngine;
using System;

public class Herbivore : Animal
{
    public Herbivore() { }

    public override void Eat(IEntity e)
    {
        throw new NotImplementedException();
    }

    public override void GeneratePath()
    {
        throw new NotImplementedException();
    }
}
