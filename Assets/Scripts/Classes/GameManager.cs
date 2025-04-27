using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;
using System.Xml;

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
    private int _minMoney;
    private int _entranceFee;
    private int _herbivoreCount;
    private int _minHerbivoreCount;
    private int _carnivoreCount;
    private int _minCarnivoreCount;
    private int _minTuristCount;
    private int _minTuristSatisfaction;
    public readonly float eps = 0.1f;

    private List<Rhino> _rhinos;
    private List<Zebra> _zebras;
    private List<Giraffe> _giraffes;
    private List<Lion> _lions;
    private List<Hyena> _hyenas;
    private List<Cheetah> _cheetahs;

    private List<Vehicle> _vehicles;
    private List<Turist> _turists;
    private List<Poacher> _poachers;

    private Load _gameLoader;
    private Save _gameSaver;

    public Node[,] WMap { get; private set; }
    private bool _purchaseMode = false;

    public delegate void OnPurchaseModeDisable();
    public event OnPurchaseModeDisable onPurchaseModeDisable;

    public int MinTuristCount { get => _minTuristCount; }
    public int MinTuristSatisfaction { get => _minTuristSatisfaction; }
    public List<Rhino> Rhinos { get => _rhinos; }
    public List<Zebra> Zebras { get => _zebras; }
    public List<Giraffe> Giraffes { get => _giraffes; }
    public List<Lion> Lions { get => _lions; }
    public List<Hyena> Hyenas { get => _hyenas; }
    public List<Cheetah> Cheetahs { get => _cheetahs; }
    public List<Turist> Turists { get => _turists; }
    public List<Vehicle> Vehicles { get => _vehicles; }
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

        _difficulty = (Difficulty)DifficultyBtns.difficulty;

        switch (_difficulty)
        {
            case Difficulty.EASY:
                _money = 1000000;
                _minMoney = 10000;
                _entranceFee = 5;
                _minHerbivoreCount = 10;
                _minCarnivoreCount = 10;
                _minTuristCount = 10;
                _minTuristSatisfaction = 20;
                break;
            case Difficulty.MEDIUM:
                _money = 750000;
                _minMoney = 15000;
                _entranceFee = 10;
                _minHerbivoreCount = 15;
                _minCarnivoreCount = 15;
                _minTuristCount = 15;
                _minTuristSatisfaction = 25;
                break;
            case Difficulty.HARD:
                _money = 500000;
                _minMoney = 20000;
                _entranceFee = 15;
                _minHerbivoreCount = 20;
                _minCarnivoreCount = 20;
                _minTuristCount = 20;
                _minTuristSatisfaction = 30;
                break;
            default:
                break;
        }

        _rhinos = new List<Rhino>();
        _zebras = new List<Zebra>();
        _giraffes = new List<Giraffe>();
        _lions = new List<Lion>();
        _hyenas = new List<Hyena>();
        _cheetahs = new List<Cheetah>();

        _vehicles = new List<Vehicle>();
        _turists = new List<Turist>();
        _poachers = new List<Poacher>();

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);

        WMap = new Node[gameTable.Size.x, gameTable.Size.y];

        for (int i = 0; i < gameTable.Size.x; i++)
        {
            for (int j = 0; j < gameTable.Size.y; j++)
            {
                switch (gameTable.gameMap[i, j])
                {
                    case Terrain.HILL:
                        WMap[i, j] = new Node(i,j,2);
                        break;
                    case Terrain.RIVER:
                        WMap[i, j] = new Node(i, j, 2);
                        break;
                    case Terrain.POND:
                        WMap[i, j] = new Node(i, j, -1);
                        break;
                    default:
                        WMap[i, j] = new Node(i, j, 1);
                        break;
                }
            }
        }

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

    public int CalculateSatisfaction()
    {
        return 50;
    }
#nullable enable
    public void Buy(GameObject? gameObject)
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
                    _rhinos.Add(gameObject.GetComponent<Rhino>());
                    gameObject.GetComponent<Animal>().Placed = true;
                    break;
                case "Zebra":
                    _zebras.Add(gameObject.GetComponent<Zebra>());
                    gameObject.GetComponent<Animal>().Placed = true;
                    break;
                case "Giraffe":
                    _giraffes.Add(gameObject.GetComponent<Giraffe>());
                    gameObject.GetComponent<Animal>().Placed = true;
                    break;
                case "Lion":
                    _lions.Add(gameObject.GetComponent<Lion>());
                    gameObject.GetComponent<Animal>().Placed = true;
                    break;
                case "Hyena":
                    _hyenas.Add(gameObject.GetComponent<Hyena>());
                    gameObject.GetComponent<Animal>().Placed = true;
                    break;
                case "Cheetah":
                    _cheetahs.Add(gameObject.GetComponent<Cheetah>());
                    gameObject.GetComponent<Animal>().Placed = true;
                    break;
                case "Jeep":
                    _vehicles.Add(gameObject.GetComponent<Vehicle>());
                    break;
                default:
                    break;
            }            
            price = gameObject.GetComponent<IPurchasable>().Price;
        }


        _herbivoreCount = _rhinos.Count + _zebras.Count + _giraffes.Count;
        _carnivoreCount = _lions.Count + _hyenas.Count + _cheetahs.Count;

        /*if (_money < _minMoney + 500)
            Notifier.Instance.Notify($"Money is low ({_money})!\nMin money: {_minMoney}");*/

        _money -= price;
    }

    public void Sell(GameObject gameObject)
    {
        switch (gameObject.name)
        {
            case "Rhino":
                _rhinos.Remove(gameObject.GetComponent<Rhino>());
                break;
            case "Zebra":
                _zebras.Remove(gameObject.GetComponent<Zebra>());
                break;
            case "Giraffe":
                _giraffes.Remove(gameObject.GetComponent<Giraffe>());
                break;
            case "Lion":
                _lions.Remove(gameObject.GetComponent<Lion>());
                break;
            case "Hyena":
                _hyenas.Remove(gameObject.GetComponent<Hyena>());
                break;
            case "Cheetah":
                _cheetahs.Remove(gameObject.GetComponent<Cheetah>());
                break;
            case "Jeep":
                _vehicles.Remove(gameObject.GetComponent<Vehicle>());
                break;
            default:
                break;
        }


        _herbivoreCount = _rhinos.Count + _zebras.Count + _giraffes.Count;
        _carnivoreCount = _lions.Count + _hyenas.Count + _cheetahs.Count;

        /*if (_herbivoreCount < _minHerbivoreCount + 1)
            Notifier.Instance.Notify($"Herbivore count is low ({_herbivoreCount})!\nMin herbivore count: {_minHerbivoreCount}");


        if (_carnivoreCount < _minCarnivoreCount + 1)
            Notifier.Instance.Notify($"Carnivore count is low ({_carnivoreCount})!\nMin carnivore count: {_minCarnivoreCount}");
        */

        int salePrice = gameObject.GetComponent<IPurchasable>().SalePrice;
        if (gameObject.tag == "Animal" && gameObject.GetComponent<Animal>().HasChip)
            salePrice += 100;
        
        _money += salePrice;

        Destroy(gameObject);
    }

}