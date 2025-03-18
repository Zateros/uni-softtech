using UnityEngine;

public interface IPurchasable
{
    public void Buy(IEntity e);
    public void Place(IEntity e);
    public void Sell(IEntity e);
    
}
