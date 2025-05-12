using UnityEngine;
using System.Collections;

public abstract class Carnivore : Animal
{
    public new void Awake()
    {
        base.Awake();
    }

    public override IEnumerable Eat(IEntity e)
    {
        if (e is Herbivore)
        {
            Herbivore herbivore = (Herbivore)e;
            herbivore.Die();
            hunger = _hungerMax;
            yield return new WaitForSeconds(_sleepDuration);
        }
        yield break;
    }
}
