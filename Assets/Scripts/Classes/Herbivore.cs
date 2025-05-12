using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class Herbivore : Animal
{
    public new void Awake()
    {
        base.Awake();
    }

    public override IEnumerable Eat(IEntity e)
    {
        if(e is Plant)
        {
            Plant plant = (Plant)e;
            plant.Eat();
            hunger = _hungerMax;
            yield return new WaitForSeconds(_sleepDuration);
        }
        yield break;
    }
}
