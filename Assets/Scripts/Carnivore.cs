using UnityEngine;
using System;

public class Carnivore : Animal
{
    public Carnivore() { }

    public override void Eat(IEntity e)
    {
        throw new NotImplementedException();
    }

    public override void GeneratePath()
    {
        throw new NotImplementedException();
    }
}
