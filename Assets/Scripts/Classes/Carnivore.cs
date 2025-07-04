using UnityEngine;
using System.Collections;

public abstract class Carnivore : Animal
{
    public new void Awake()
    {
        base.Awake();
    }

    public override IEnumerator Eat(IEntity e)
    {
        if (e is Herbivore)
        {
            Herbivore herbivore = (Herbivore)e;
            herbivore.Die();
            _asleep = true;
            hunger = _hungerMax;
            yield return new WaitForSeconds(_sleepDuration);
            _asleep = false;
        }
        yield break;
    }
}
