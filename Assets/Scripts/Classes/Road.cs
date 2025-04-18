using UnityEngine;

public class Road : IPurchasable
{
    private int _price;
    private int _salePrice;

    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }


    public Road()
    {

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 10;
                _salePrice = 0;
                break;
            case Difficulty.MEDIUM:
                _price = 20;
                _salePrice = 0;
                break;
            case Difficulty.HARD:
                _price = 30;
                _salePrice = 0;
                break;
            default:
                break;
        }
    }
}
