using UnityEngine;
using TMPro;

public class PricesPanelScript : MonoBehaviour
{
    [SerializeField] public GameObject PricesPanel;
    [SerializeField] public TextMeshProUGUI ItemsText;
    [SerializeField] public TextMeshProUGUI PricesText;
    [SerializeField] public TextMeshProUGUI SalePricesText;

    [SerializeField] public GameObject Rhino;
    [SerializeField] public GameObject Zebra;
    [SerializeField] public GameObject Giraffe;
    [SerializeField] public GameObject Lion;
    [SerializeField] public GameObject Hyena;
    [SerializeField] public GameObject Cheetah;
    [SerializeField] public GameObject Grass;
    [SerializeField] public GameObject Bush;
    [SerializeField] public GameObject Tree;
    [SerializeField] public GameObject Jeep;
    [SerializeField] public GameObject Chip;
    public Road Road;

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
{Road.SalePrice}$
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
