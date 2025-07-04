using UnityEngine;

public class Road : IPurchasable
{
    private int _price;
    private int _salePrice;

    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }
    public bool Placed { get; set; }

    public static int DestroyExtraCost { get => 0;}

    public Road()
    {

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 100;
                _salePrice = 0;
                break;
            case Difficulty.MEDIUM:
                _price = 150;
                _salePrice = 0;
                break;
            case Difficulty.HARD:
                _price = 250;
                _salePrice = 0;
                break;
            default:
                break;
        }
    }
}
