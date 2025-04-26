using UnityEngine;

public class Tree : Plant
{
    public void Awake()
    {

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 50;
                _salePrice = 50;
                break;
            case Difficulty.MEDIUM:
                _price = 75;
                _salePrice = 70;
                break;
            case Difficulty.HARD:
                _price = 90;
                _salePrice = 80;
                break;
            default:
                break;
        }
    }
}
