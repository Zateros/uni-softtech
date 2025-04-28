using System.Collections.Generic;
using UnityEngine;

public class Zebra : Herbivore
{
    public new void Awake()
    {
        _FOV = 200f;
        _speed = 1.2f;
        _visionRange = 2f;
        _size = .7f;
        base.Awake();

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 500;
                _salePrice = 500;
                break;
            case Difficulty.MEDIUM:
                _price = 650;
                _salePrice = 600;
                break;
            case Difficulty.HARD:
                _price = 800;
                _salePrice = 700;
                break;
            default:
                break;
        }
    }

    public override void Die()
    {
        GameManager.Instance.Zebras.Remove(this);
        Destroy(gameObject);
    }

    public override List<Animal> GetNeighbours(float range)
    {
        List<Animal> neighbours = new List<Animal>();
        foreach (Zebra zebra in GameManager.Instance.Zebras)
        {
            if (this == zebra) continue;
            if (Vector2.Distance(zebra.transform.position, _position) <= range)
            {
                neighbours.Add(zebra);
            }
        }
        return neighbours;
    }
}
