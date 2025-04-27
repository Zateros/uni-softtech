using System.Collections.Generic;
using UnityEngine;

public class Hyena : Carnivore
{
    public new void Awake()
    {
        _FOV = 200f;
        _speed = 1.5f;
        _visionRange = 2f;
        _size = .35f;
        base.Awake();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 10;
                _salePrice = 10;
                break;
            case Difficulty.MEDIUM:
                _price = 20;
                _salePrice = 20;
                break;
            case Difficulty.HARD:
                _price = 30;
                _salePrice = 30;
                break;
            default:
                break;
        }
    }

    public override void Die()
    {
        GameManager.Instance.Hyenas.Remove(this);
        Destroy(gameObject);
    }

    public override List<Animal> GetNeighbours(float range)
    {
        List<Animal> neighbours = new List<Animal>();
        foreach (Hyena hyena in GameManager.Instance.Hyenas)
        {
            if (this == hyena) continue;
            if (Vector2.Distance(hyena.transform.position, _position) <= range)
            {
                neighbours.Add(hyena);
            }
        }
        return neighbours;
    }
}
