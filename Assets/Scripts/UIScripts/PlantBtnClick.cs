using UnityEngine;
using UnityEngine.InputSystem;

public class PlantBtnClick : MonoBehaviour
{
    public GameObject PanelAnimal;
    public GameObject PanelPlant;
    public GameObject PanelVehicle;

    public void OpenPanel()
    {
        if (PanelPlant == null) return;

        if (PanelPlant.activeInHierarchy)
        {
            PanelPlant.SetActive(false);
        }
        else
        {
            PanelPlant.SetActive(true);
            if (PanelAnimal != null && PanelAnimal.activeInHierarchy) { PanelAnimal.SetActive(false); }
            if (PanelVehicle != null && PanelVehicle.activeInHierarchy) { PanelVehicle.SetActive(false); }
        }
    }
}
