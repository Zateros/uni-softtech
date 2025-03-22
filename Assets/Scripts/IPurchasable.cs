using UnityEngine;

public interface IPurchasable
{
    public int Cost { get; }
    public void Buy(IEntity e);
    public void Place(IEntity e);
    public void Sell(IEntity e);
    
}
