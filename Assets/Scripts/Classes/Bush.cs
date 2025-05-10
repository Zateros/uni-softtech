using UnityEngine;

public class Bush : Plant
{
    public void Awake()
    {
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 40;
                _salePrice = 40;
                break;
            case Difficulty.MEDIUM:
                _price = 50;
                _salePrice = 45;
                break;
            case Difficulty.HARD:
                _price = 65;
                _salePrice = 55;
                break;
            default:
                break;
        }
    }
}
