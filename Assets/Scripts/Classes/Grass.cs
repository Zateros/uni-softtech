using UnityEngine;

public class Grass : Plant
{
    public void Awake()
    {

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 5;
                _salePrice = 5;
                break;
            case Difficulty.MEDIUM:
                _price = 7;
                _salePrice = 5;
                break;
            case Difficulty.HARD:
                _price = 10;
                _salePrice = 7;
                break;
            default:
                break;
        }
    }
}
