using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Chip : MonoBehaviour, IPurchasable
{
    private int _price;
    private int _salePrice;

    private Camera mainCamera;
    private Vector3 mousePos;
    private RaycastHit2D hit2D;

    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }
    public bool Placed { get; set; }

    void Awake()
    {
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 100;
                _salePrice = 0;
                break;
            case Difficulty.MEDIUM:
                _price = 120;
                _salePrice = 0;
                break;
            case Difficulty.HARD:
                _price = 140;
                _salePrice = 0;
                break;
            default:
                break;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;   
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            hit2D = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
            if(hit2D.collider != null)
            {
                GameObject hitObject = hit2D.collider.gameObject;

                if (hitObject.tag == "Animal" && hitObject.GetComponent<Animal>().HasChip != true)
                {
                    enabled = false;
                    GameManager.Instance.Buy(gameObject);
                    hitObject.GetComponent<Animal>().HasChip = true;
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
            enabled = false;
        }
    }
}
