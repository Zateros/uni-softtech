using UnityEngine;
using UnityEngine.InputSystem;

public class AnimalBtnClick : MonoBehaviour
{
    public GameObject PanelAnimal;
    public GameObject PanelPlant;
    public GameObject PanelVehicle;

    public void OpenPanel()
    {
        if (PanelAnimal == null) return;

        if (PanelAnimal.activeInHierarchy)
        {
            PanelAnimal.SetActive(false);
        }
        else
        {
            PanelAnimal.SetActive(true);
            if (PanelPlant != null && PanelPlant.activeInHierarchy) { PanelPlant.SetActive(false); }
            if (PanelVehicle != null && PanelVehicle.activeInHierarchy) { PanelVehicle.SetActive(false); }
        }
    }
}
