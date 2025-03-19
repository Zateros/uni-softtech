using UnityEngine;

public class Road : MonoBehaviour, IPurchasable
{
    private int _price;
    private int _salePrice;

    public Road() { }


    public void Buy(IEntity e)
    {
        throw new System.NotImplementedException();
    }

    public void Place(IEntity e)
    {
        throw new System.NotImplementedException();
    }

    public void Sell(IEntity e)
    {
        throw new System.NotImplementedException();
    }
}
