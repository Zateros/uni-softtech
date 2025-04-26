using UnityEngine;

public class Cheetah : Carnivore
{
    public new void Awake()
    {
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
}
