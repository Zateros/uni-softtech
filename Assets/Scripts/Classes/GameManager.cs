using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor;

    public static GameManager Instance;

    [SerializeField]
    private Map gameTable;

    [SerializeField]
    private Minimap minimap;

    private DateTime _time;
    private Speed _gameSpeed;
    private Difficulty _difficulty;
    private bool _hasWon;

    private int _money;
    private int _entranceFee;
    private int _minHerbivoreCount;
    private int _minCarnivoreCount;
    private int _minMoney;
    private int _minTouristCount;
    public readonly float eps = 0.1f;

    private List<GameObject> _rhinos;
    private List<GameObject> _zebras;
    private List<GameObject> _giraffes;
    private List<GameObject> _lions;
    private List<GameObject> _hyenas;
    private List<GameObject> _cheetahs;

    private List<GameObject> _vehicles;
    private List<GameObject> _tourists;
    private List<GameObject> _poachers;

    private Load _gameLoader;
    private Save _gameSaver;
    private bool _purchaseMode = false;

    public delegate void OnPurchaseModeDisable();
    public event OnPurchaseModeDisable onPurchaseModeDisable;

    public List<GameObject> Vehicles { get => _vehicles; }
    public List<GameObject> Animals { get => _animals; }
    public Map GameTable { get => gameTable; }
    public Minimap Minimap { get => minimap; }
    public List<List<Vector2>> Routes { get; private set; } = new List<List<Vector2>>();
    public bool IsGameRunnning { get; private set; }
    public int Money { get => _money; }
    public Difficulty Difficulty { get => _difficulty; }
    public bool PurchaseMode
    {
        get => _purchaseMode;
        set
        {
            if (value != _purchaseMode)
            {
                _purchaseMode = value;
                if(value == false) onPurchaseModeDisable?.Invoke();
            }
        }
    }

    void Awake()
    {
        Instance = this;
        _difficulty = Difficulty.EASY;
        switch (_difficulty)
        {
            case Difficulty.EASY:
                break;
            case Difficulty.MEDIUM:
                break;
            case Difficulty.HARD:
                break;
            default:
                break;
        }

        _money = 1500;

        _rhinos = new List<GameObject>();
        _zebras = new List<GameObject>();
        _giraffes = new List<GameObject>();
        _lions = new List<GameObject>();
        _hyenas = new List<GameObject>();
        _cheetahs = new List<GameObject>();

        _vehicles = new List<GameObject>();
        _tourists = new List<GameObject>();
        _poachers = new List<GameObject>();

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);

        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && PurchaseMode) {
            PurchaseMode = false;
        }
    }

    public void StartGame()
    {
        throw new NotImplementedException();
    }

    public void GameLoop()
    {
        throw new NotImplementedException();
    }

    public void PauseGame()
    {
        throw new NotImplementedException();
    }

    public void GameLoad()
    {
        throw new NotImplementedException();
    }

    public void GameSave()
    {
        throw new NotImplementedException();
    }

    public void CalculateSatisfaction()
    {
        throw new NotImplementedException();
    }

    public void Buy(GameObject gameObject)
    {
        int price;
        if (gameObject == null)
        {
            Road road = new();
            price = road.Price;
        }
        else
        {
            switch (gameObject.name)
            {
                case "Rhino":
                    _rhinos.Add(gameObject);
                    break;
                case "Zebra":
                    _zebras.Add(gameObject);
                    break;
                case "Giraffe":
                    _giraffes.Add(gameObject);
                    break;
                case "Lion":
                    _lions.Add(gameObject);
                    break;
                case "Hyena":
                    _hyenas.Add(gameObject);
                    break;
                case "Cheetah":
                    _cheetahs.Add(gameObject);
                    break;
                case "Jeep":
                    _vehicles.Add(gameObject);
                    break;
                default:
                    break;
            }
        
            price = gameObject.GetComponent<IPurchasable>().Price;
        }
        _money -= price;
    }

    public void Sell(GameObject gameObject)
    {
        switch (gameObject.name)
        {
            case "Rhino":
                _rhinos.Remove(gameObject);
                break;
            case "Zebra":
                _zebras.Remove(gameObject);
                break;
            case "Giraffe":
                _giraffes.Remove(gameObject);
                break;
            case "Lion":
                _lions.Remove(gameObject);
                break;
            case "Hyena":
                _hyenas.Remove(gameObject);
                break;
            case "Cheetah":
                _cheetahs.Remove(gameObject);
                break;
            case "Jeep":
                _vehicles.Remove(gameObject);
                break;
            default:
                break;
        }
        
        int salePrice = gameObject.GetComponent<IPurchasable>().SalePrice;
        _money += salePrice;

        Destroy(gameObject);
    }

}
