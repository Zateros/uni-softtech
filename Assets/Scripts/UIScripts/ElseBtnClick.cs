using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleBtnClick : MonoBehaviour
{
    public GameObject PanelAnimal;
    public GameObject PanelPlant;
    public GameObject PanelVehicle;

    public void OpenPanel()
    {
        if (PanelVehicle == null) return;

        if (PanelVehicle.activeInHierarchy)
        {
            PanelVehicle.SetActive(false);
        }
        else
        {
            PanelVehicle.SetActive(true);
            if (PanelAnimal != null && PanelAnimal.activeInHierarchy) { PanelAnimal.SetActive(false); }
            if (PanelPlant != null && PanelPlant.activeInHierarchy) { PanelPlant.SetActive(false); }
        }
    }
}
