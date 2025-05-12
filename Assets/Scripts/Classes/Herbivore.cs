using UnityEngine;
using System.Collections;

public abstract class Herbivore : Animal
{
    public new void Awake()
    {
        base.Awake();
    }

    public override IEnumerator Eat(IEntity e)
    {
        Debug.Log("Eat");
        if (e is Plant)
        {
            _asleep = true;
            hunger = _hungerMax;
            yield return new WaitForSeconds(_sleepDuration);
            _asleep = false;
        }
        yield break;
    }
}
