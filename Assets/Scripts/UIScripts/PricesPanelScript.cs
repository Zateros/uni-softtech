using UnityEngine;
using TMPro;


/// <summary>
/// Handles Prices button click, opens prices panel.
/// </summary>
public class PricesPanelScript : MonoBehaviour
{
    public GameObject PricesPanel;
    public TextMeshProUGUI ItemsText;
    public TextMeshProUGUI PricesText;
    public TextMeshProUGUI SalePricesText;

    public GameObject Rhino;
    public GameObject Zebra;
    public GameObject Giraffe;
    public GameObject Lion;
    public GameObject Hyena;
    public GameObject Cheetah;
    public GameObject Grass;
    public GameObject Bush;
    public GameObject Tree;
    public GameObject Jeep;
    public GameObject Chip;
    public Road Road;
    public Water Water;

    /// <summary>
    /// Opens up Prices panel, writes purchasable items' prices on it based on difficulty.
    /// </summary>
    public void OnPricesBtnClick()
    {
        if (PricesPanel.activeSelf)
        {
            PricesPanel.SetActive(false);
        }
        else
        {
            PricesPanel.SetActive(true);

            Road = new();
            Water = new();
            var myRhino = Instantiate(Rhino);
            var myZebra = Instantiate(Zebra);
            var myGiraffe = Instantiate(Giraffe);
            var myLion = Instantiate(Lion);
            var myHyena = Instantiate(Hyena);
            var myCheetah = Instantiate(Cheetah);
            var myGrass = Instantiate(Grass);
            var myBush = Instantiate(Bush);
            var myTree = Instantiate(Tree);
            var myJeep = Instantiate(Jeep);
            var myChip = Instantiate(Chip);

            ItemsText.text =
$@"Items

Rhino
Zebra
Giraffe
Lion
Hyena
Cheetah
Grass
Bush
Tree
Jeep
Road
Water
Chip";
            
            PricesText.text =
$@"Prices

{myRhino.GetComponent<Rhino>().Price} $
{myZebra.GetComponent<Zebra>().Price} $
{myGiraffe.GetComponent<Giraffe>().Price} $
{myLion.GetComponent<Lion>().Price} $
{myHyena.GetComponent<Hyena>().Price} $
{myCheetah.GetComponent<Cheetah>().Price} $
{myGrass.GetComponent<Grass>().Price} $
{myBush.GetComponent<Bush>().Price} $
{myTree.GetComponent<Tree>().Price} $
{myJeep.GetComponent<Vehicle>().Price} $
{Road.Price} $
{Water.Price} $
{myChip.GetComponent<Chip>().Price} $";

            SalePricesText.text =
$@"Sale Prices

{myRhino.GetComponent<Rhino>().SalePrice} $
{myZebra.GetComponent<Zebra>().SalePrice} $
{myGiraffe.GetComponent<Giraffe>().SalePrice} $
{myLion.GetComponent<Lion>().SalePrice} $
{myHyena.GetComponent<Hyena>().SalePrice} $
{myCheetah.GetComponent<Cheetah>().SalePrice} $
{myGrass.GetComponent<Grass>().SalePrice} $
{myBush.GetComponent<Bush>().SalePrice} $
{myTree.GetComponent<Tree>().SalePrice} $
{myJeep.GetComponent<Vehicle>().SalePrice} $
{Road.SalePrice} $
{Water.SalePrice} $
-";

            Destroy(myRhino);
            Destroy(myZebra);
            Destroy(myGiraffe);
            Destroy(myLion);
            Destroy(myHyena);
            Destroy(myCheetah);
            Destroy(myGrass);
            Destroy(myBush);
            Destroy(myTree);
            Destroy(myJeep);
            Destroy(myChip);
        }
    }
}
