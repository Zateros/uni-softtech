using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PurchasableBtnClick : MonoBehaviour
{
    public GameObject PanelAnimal;
    public GameObject PanelPlant;
    public GameObject PanelVehicle;
    [SerializeField] public GameObject Canvas;
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
    [SerializeField] public GameObject Road;

    private Camera mainCamera;
    private Vector3 mousePosition;
    private Vector3 offset;

    private void Start()
    {
        mainCamera = Camera.main;
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
                var myRoad = Instantiate(Road, mousePosition, Quaternion.identity);
                myRoad.name = "Road";
                break;
            default:
                break;
        }

        if (PanelAnimal != null && PanelAnimal.activeInHierarchy) { PanelAnimal.SetActive(false); }
        if (PanelPlant != null && PanelPlant.activeInHierarchy) { PanelPlant.SetActive(false); }
        if (PanelVehicle != null && PanelVehicle.activeInHierarchy) { PanelVehicle.SetActive(false); }
    }

}
