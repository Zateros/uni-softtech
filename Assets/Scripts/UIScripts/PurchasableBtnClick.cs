using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Map;

public class PurchasableBtnClick : MonoBehaviour
{
    public GameObject PanelAnimal;
    public GameObject PanelPlant;
    public GameObject PanelVehicle;

    public Slider SellToggle;

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
    [SerializeField] private Placer placer;

    private Camera mainCamera;
    private Vector3 mousePosition;

    private void Start()
    {
        mainCamera = Camera.main;
        if (placer != null)
        {
            Placer.onPlaced += Buy;
            GameManager.Instance.onPurchaseModeDisable += DisablePurchaseMode;
        }
    }

    public void OnBtnClick()
    {
        if (SellToggle.GetComponent<ToggleSwitch>().IsToggled)
        {
            Notifier.Instance.Notify("Can't buy items while in Sell Mode!");
            return;
        }

        string clickedBtnName = EventSystem.current.currentSelectedGameObject.name;
        GameObject CurrentPanel = null;
        GameObject OtherPanel1 = null;
        GameObject OtherPanel2 = null;

        switch (clickedBtnName)
        {
            case "AnimalBtn":
                CurrentPanel = PanelAnimal;
                OtherPanel1 = PanelPlant;
                OtherPanel2 = PanelVehicle;
                break;

            case "PlantBtn":
                CurrentPanel = PanelPlant;
                OtherPanel1 = PanelAnimal;
                OtherPanel2 = PanelVehicle;
                break;

            case "ElseBtn":
                CurrentPanel = PanelVehicle;
                OtherPanel1 = PanelAnimal;
                OtherPanel2 = PanelPlant;
                
                break;
            default:
                break;
        }

        if (CurrentPanel == null || OtherPanel1 == null || OtherPanel2 == null)
            return;

        if (CurrentPanel.activeInHierarchy)
        {
            CurrentPanel.SetActive(false);
        }
        else
        {
            CurrentPanel.SetActive(true);
            if (OtherPanel1.activeInHierarchy) { OtherPanel1.SetActive(false); }
            if (OtherPanel2.activeInHierarchy) { OtherPanel2.SetActive(false); }
        }
    }

    public void SpawnEntity()
    {
        string clickedBtnName = EventSystem.current.currentSelectedGameObject.name;
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0;

        switch (clickedBtnName)
        {

            case "RhinoBtn":
                var myRhino = Instantiate(Rhino, mousePosition, Quaternion.identity);
                myRhino.name = "Rhino";
                break;
            case "ZebraBtn":
                var myZebra = Instantiate(Zebra, mousePosition, Quaternion.identity);
                myZebra.name = "Zebra";
                break;
            case "GiraffeBtn":
                var myGiraffe = Instantiate(Giraffe, mousePosition, Quaternion.identity);
                myGiraffe.name = "Giraffe";
                break;
            case "LionBtn":
                var myLion = Instantiate(Lion, mousePosition, Quaternion.identity);
                myLion.name = "Lion";
                break;
            case "HyenaBtn":
                var myHyena = Instantiate(Hyena, mousePosition, Quaternion.identity);
                myHyena.name = "Hyena";
                break;
            case "CheetahBtn":
                var myCheetah = Instantiate(Cheetah, mousePosition, Quaternion.identity);
                myCheetah.name = "Cheetah";
                break;
            case "GrassBtn":
                var myGrass = Instantiate(Grass, mousePosition, Quaternion.identity);
                myGrass.name = "Grass";
                break;
            case "BushBtn":
                var myBush = Instantiate(Bush, mousePosition, Quaternion.identity);
                myBush.name = "Bush";
                break;
            case "TreeBtn":
                var myTree = Instantiate(Tree, mousePosition, Quaternion.identity);
                myTree.name = "Tree";
                break;
            case "JeepBtn":
                var myJeep = Instantiate(Jeep, mousePosition, Quaternion.identity);
                myJeep.name = "Jeep";
                break;
            case "RoadBtn":
                GameManager.Instance.PurchaseMode = true;
                placer.SetMode(PlacerMode.LINE, PlacerType.ROAD);
                placer.enabled = true;
                break;
            case "ChipBtn":
                var myChip = Instantiate(Chip, mousePosition, Quaternion.identity);
                myChip.name = "Chip";
                break;
            case "WaterBtn":
                GameManager.Instance.PurchaseMode = true;
                placer.SetMode(PlacerMode.SQUARE, PlacerType.WATER);
                placer.enabled = true;
                break;
            default:
                break;
        }

        if (PanelAnimal != null && PanelAnimal.activeInHierarchy) { PanelAnimal.SetActive(false); }
        if (PanelPlant != null && PanelPlant.activeInHierarchy) { PanelPlant.SetActive(false); }
        if (PanelVehicle != null && PanelVehicle.activeInHierarchy) { PanelVehicle.SetActive(false); }
    }

    private void Buy(int price)
    {
        GameManager.Instance.Buy(null, price);
    }

    private void DisablePurchaseMode()
    {
        placer.enabled = false;
    }
}
