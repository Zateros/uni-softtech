using UnityEngine;

public class Hill : IPurchasable
{

    public int Price { get => 0; }
    public int SalePrice { get => 0; }
    public bool Placed { get; set; }
    public static int DestroyExtraCost { get => 200; }
}
