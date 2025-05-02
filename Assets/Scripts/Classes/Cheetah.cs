using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Cheetah : Carnivore
{
    public new void Awake()
    {
        _FOV = 210f;
        _speed = 2f;
        _visionRange = 2f;
        _size = .5f;
        base.Awake();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 500;
                _salePrice = 500;
                break;
            case Difficulty.MEDIUM:
                _price = 750;
                _salePrice = 600;
                break;
            case Difficulty.HARD:
                _price = 1000;
                _salePrice = 800;
                break;
            default:
                break;
        }
    }

    public override void Die()
    {
        GameManager.Instance.Cheetahs.Remove(this);
        Destroy(gameObject);
    }

    public override List<Animal> GetNeighbours(float range)
    {
        List<Animal> neighbours = new List<Animal>();
        foreach (Cheetah cheetah in GameManager.Instance.Cheetahs)
        {
            if (this == cheetah) continue;
            if(Vector2.Distance(cheetah.transform.position, _position) <= range)
            {
                neighbours.Add(cheetah);
            }
        }
        return neighbours;
    }
}
