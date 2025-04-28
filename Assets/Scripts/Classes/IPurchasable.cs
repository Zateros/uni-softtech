using UnityEngine;

public interface IPurchasable
{   
    public int Price { get; }
    public int SalePrice {  get; }
    public bool Placed { get; set; }
}
