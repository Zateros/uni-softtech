using UnityEngine;
using System;
using System.Collections.Generic;

public class Rhino : Herbivore
{
    public new void Awake()
    {
        _FOV = 180f;
        _speed = .8f;
        _visionRange = 2f;
        _size = 1.5f;
        base.Awake();

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 1500;
                _salePrice = 1500;
                break;
            case Difficulty.MEDIUM:
                _price = 2000;
                _salePrice = 1750;
                break;
            case Difficulty.HARD:
                _price = 2500;
                _salePrice = 2000;
                break;
            default:
                break;
        }
    }

    public override void Die()
    {
        GameManager.Instance.Rhinos.Remove(this);
        Destroy(gameObject);
    }

    public override List<Animal> GetNeighbours(float range)
    {
        List<Animal> neighbours = new List<Animal>();
        foreach (Rhino rhino in GameManager.Instance.Rhinos)
        {
            if (this == rhino) continue;
            if (Vector2.Distance(rhino.transform.position, _position) <= range)
            {
                neighbours.Add(rhino);
            }
        }
        return neighbours;
    }
}
