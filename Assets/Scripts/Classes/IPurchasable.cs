
public interface IPurchasable
{   
    public int Price { get; }
    public int SalePrice {  get; }
    public static int DestroyExtraCost {  get; }
    public bool Placed { get; set; }
}
