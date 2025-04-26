using UnityEngine;

public class Lion : Carnivore
{
    public new void Awake()
    {
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
}
