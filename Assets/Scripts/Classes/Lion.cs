using System.Collections.Generic;
using UnityEngine;

public class Lion : Carnivore
{
    public new void Awake()
    {
        _FOV = 190f;
        _speed = 1.25f;
        _visionRange = 2f;
        _size = .75f;
        base.Awake();

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 650;
                _salePrice = 650;
                break;
            case Difficulty.MEDIUM:
                _price = 800;
                _salePrice = 750;
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
        GameManager.Instance.Lions.Remove(this);
        Destroy(gameObject);
    }

    public override List<Animal> GetNeighbours(float range)
    {
        List<Animal> neighbours = new List<Animal>();
        foreach (Lion lion in GameManager.Instance.Lions)
        {
            if (this == lion) continue;
            if (Vector2.Distance(lion.transform.position, _position) <= range)
            {
                neighbours.Add(lion);
            }
        }
        return neighbours;
    }
}
