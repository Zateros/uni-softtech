using System.Collections.Generic;
using UnityEngine;

public class Giraffe : Herbivore
{
    public new void Awake()
    {
        _FOV = 200f;
        _speed = 1f;
        _visionRange = 3f;
        _size = 1f;
        base.Awake();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 2500;
                _salePrice = 2500;
                break;
            case Difficulty.MEDIUM:
                _price = 4000;
                _salePrice = 3750;
                break;
            case Difficulty.HARD:
                _price = 5000;
                _salePrice = 4000;
                break;
            default:
                break;
        }
    }

    public override void Die()
    {
        GameManager.Instance.Giraffes.Remove(this);
        Destroy(gameObject);
    }

    public override List<Animal> GetNeighbours(float range)
    {
        List<Animal> neighbours = new List<Animal>();
        foreach (Giraffe giraffe in GameManager.Instance.Giraffes)
        {
            if (this == giraffe) continue;
            if (Vector2.Distance(giraffe.transform.position, _position) <= range)
            {
                neighbours.Add(giraffe);
            }
        }
        return neighbours;
    }
}
