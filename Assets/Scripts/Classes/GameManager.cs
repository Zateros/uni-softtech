using System;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;
using System.Xml;
using NUnit.Framework.Internal;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor;
    [SerializeField] public GameObject bush;
    [SerializeField] public GameObject tree;
    [SerializeField] public GameObject turist;
    public static GameManager Instance;

    [SerializeField]
    private Map gameTable;

    [SerializeField]
    private Minimap minimap;

    private DateTime _time;
    private DateTime _prevDay;
    private DateTime _prevWeek;
    private float _prevSpeed;
    private int _daysPassed;
    private int _monthsToWin;
    private DateTime _winningDate;
    private bool _isNight;
    private Difficulty _difficulty;
    private bool _hasWon;
    private bool _notifiedMonthsReset;

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

    private Vector2 _enterance;
    private Vector2 _exit;

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
    public Plant[,] Plants { get; set; }
    private bool _purchaseMode = false;

    public delegate void OnPurchaseModeDisable();
    public event OnPurchaseModeDisable onPurchaseModeDisable;

    public delegate void OnGameOver();
    public event OnGameOver onGameOver;

    public delegate void OnGameWon();
    public event OnGameOver onGameWon;

    public int MinTuristCount { get => _minTuristCount; }
    public int MinTuristSatisfaction { get => _minTuristSatisfaction; }
    public int MinHerbivoreCount { get => _minHerbivoreCount; }
    public int MinCarnivoreCount { get => _minCarnivoreCount; }
    public List<Herbivore> Herbivores
    {
        get
        {
            List<Herbivore> herbivores = new List<Herbivore>();
            herbivores.AddRange(_rhinos);
            herbivores.AddRange(_zebras);
            herbivores.AddRange(_giraffes);
            return herbivores;
        }
    }
    public List<Carnivore> Carnivores
    {
        get
        {
            List<Carnivore> carnivores = new List<Carnivore>();
            carnivores.AddRange(_lions);
            carnivores.AddRange(_hyenas);
            carnivores.AddRange(_cheetahs);
            return carnivores;
        }
    }
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
    public Vector2 Entrance { get => _enterance; }
    public Vector2 Exit { get => _exit; }
    public int DaysPassed { get => _daysPassed; private set { if (value != _daysPassed) _daysPassed = value; } }
    public DateTime Date { get => _time; private set { if (value != _time) _time = value; } }
    public float PrevSpeed { get => _prevSpeed; }
    public bool IsNight
    {
        get => _isNight; set
        {
            if (_isNight && !value) DaysPassed++;
            if (value != _isNight) _isNight = value;
        }
    }
    public Node[,] Roadmap { get; set; } 
    public bool IsGameRunnning { get; set; }
    public int satisfaction = 50;
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
                if (value == false) onPurchaseModeDisable?.Invoke();
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
                _monthsToWin = 3;
                _money = 1000000;
                _minMoney = 10000;
                _entranceFee = 5;
                _minHerbivoreCount = 10;
                _minCarnivoreCount = 10;
                _minTuristCount = 10;
                _minTuristSatisfaction = 20;
                GameTable.Size = new Vector2Int(50, 50);
                break;
            case Difficulty.MEDIUM:
                _monthsToWin = 6;
                _money = 750000;
                _minMoney = 15000;
                _entranceFee = 10;
                _minHerbivoreCount = 15;
                _minCarnivoreCount = 15;
                _minTuristCount = 15;
                _minTuristSatisfaction = 25;
                GameTable.Size = new Vector2Int(100, 100);
                break;
            case Difficulty.HARD:
                _monthsToWin = 12;
                _money = 500000;
                _minMoney = 20000;
                _entranceFee = 15;
                _minHerbivoreCount = 20;
                _minCarnivoreCount = 20;
                _minTuristCount = 20;
                _minTuristSatisfaction = 30;
                GameTable.Size = new Vector2Int(150, 150);
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

        Date = DateTime.Today;
        _prevDay = Date;
        _prevWeek = Date;
        _winningDate = Date.AddMonths(_monthsToWin);
        _notifiedMonthsReset = false;
        Time.timeScale = 1f;


        WMap = new Node[gameTable.Size.x, gameTable.Size.y];
        Roadmap = new Node[gameTable.Size.x, gameTable.Size.y];
        Plants = new Plant[gameTable.Size.x, gameTable.Size.y];

        Map.onMapGenerated += OnMapGenerated;
        IsGameRunnning = true;

        _hasWon = false;
        DontDestroyOnLoad(this);
    }
        
    void Update()
    {

        if(_money < 0)
            _money = 0;

        if (Input.GetKeyDown(KeyCode.Escape) && PurchaseMode)
        {
            PurchaseMode = false;
        }
        if (_winningDate == Date.AddDays(_daysPassed))
        {
            onGameWon?.Invoke();
            _hasWon = true;
        }

        if ((Date.AddDays(_daysPassed) - _prevDay).TotalDays == 1)
        {
            _prevDay = Date.AddDays(_daysPassed);
            AnimalsHunger();
        }

        if((Date.AddDays(_daysPassed) - _prevWeek).TotalDays == 7)
        {
            _prevWeek = Date.AddDays(_daysPassed);
            AnimalsAge();
        }


        if (IsGameRunnning)
            _prevSpeed = Time.timeScale;
    }

    private void AnimalsHunger()
    {
        foreach (Rhino rhino in _rhinos)
        {
            rhino.hunger -= 10;
            rhino.thirst -= 10;
        }

        foreach (Zebra zebra in _zebras)
        {
            zebra.hunger -= 10;
            zebra.thirst -= 10;
        }

        foreach (Giraffe giraffe in _giraffes)
        {
            giraffe.hunger -= 10;
            giraffe.thirst -= 10;
        }

        foreach (Lion lion in _lions)
        {
            lion.hunger -= 10;
            lion.thirst -= 10;
        }

        foreach (Hyena hyena in _hyenas)
        {
            hyena.hunger -= 10;
            hyena.thirst -= 10;
        }

        foreach (Cheetah cheetah in _cheetahs)
        {
            cheetah.hunger -= 10;
            cheetah.thirst -= 10;
        }
    }


    private void AnimalsAge()
    {
        foreach (Rhino rhino in _rhinos)
        {
            rhino.age++;
        }

        foreach (Zebra zebra in _zebras)
        {
            zebra.age++;
        }

        foreach (Giraffe giraffe in _giraffes)
        {
            giraffe.age++;
        }

        foreach (Lion lion in _lions)
        {
            lion.age++;
        }

        foreach (Hyena hyena in _hyenas)
        {
            hyena.age++;
        }

        foreach (Cheetah cheetah in _cheetahs)
        {
            cheetah.age++;
        }
    }

#nullable enable
    public void Buy(GameObject? gameObject, int? objectPrice)
    {
        int price;
        if (gameObject == null)
        {
            price = objectPrice ?? 0;
        }
        else
        {
            gameObject.GetComponent<IPurchasable>().Placed = true;
            Vector3Int pos;
            switch (gameObject.name)
            {
                case "Rhino":
                    _rhinos.Add(gameObject.GetComponent<Rhino>());
                    break;
                case "Zebra":
                    _zebras.Add(gameObject.GetComponent<Zebra>());
                    break;
                case "Giraffe":
                    _giraffes.Add(gameObject.GetComponent<Giraffe>());
                    break;
                case "Lion":
                    _lions.Add(gameObject.GetComponent<Lion>());
                    break;
                case "Hyena":
                    _hyenas.Add(gameObject.GetComponent<Hyena>());
                    break;
                case "Cheetah":
                    _cheetahs.Add(gameObject.GetComponent<Cheetah>());
                    break;
                case "Jeep":
                    _vehicles.Add(gameObject.GetComponent<Vehicle>());
                    break;
                case "Bush":
                    pos = GameTable.WorldToCell(gameObject.transform.position);
                    Plants[pos.x, pos.y] = gameObject.GetComponent<Bush>();
                    break;
                case "Tree":
                    pos = GameTable.WorldToCell(gameObject.transform.position);
                    Plants[pos.x, pos.y] = gameObject.GetComponent<Tree>();
                    break;
                case "Grass":
                    pos = GameTable.WorldToCell(gameObject.transform.position);
                    Plants[pos.x, pos.y] = gameObject.GetComponent<Grass>();
                    break;
                default:
                    break;
            }

            price = gameObject.GetComponent<IPurchasable>().Price;
        }

        if (!_hasWon && _money <= _minMoney + 10000)
            Notifier.Instance.Notify($"Money is low ({_money})!\nMin money: {_minMoney}");

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

        if (!_hasWon && _herbivoreCount <= _minHerbivoreCount + 2)
            Notifier.Instance.Notify($"Herbivore count is low ({_herbivoreCount})!\nMin herbivore count: {_minHerbivoreCount}");


        if (!_hasWon && _carnivoreCount <= _minCarnivoreCount + 2)
            Notifier.Instance.Notify($"Carnivore count is low ({_carnivoreCount})!\nMin carnivore count: {_minCarnivoreCount}");

        int salePrice = gameObject.GetComponent<IPurchasable>().SalePrice;
        if (gameObject.tag == "Animal" && gameObject.GetComponent<Animal>().HasChip)
            salePrice += 100;

        _money += salePrice;

        Destroy(gameObject);
    }

    public void SpeedUp()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale + 0.25f, 0f, 2f);
    }
    
    public void SlowDown()
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale - .25f, 0f, 2f);
    }

    private void OnMapGenerated()
    {

        for (int i = 0; i < gameTable.Size.x; i++)
        {
            for (int j = 0; j < gameTable.Size.y; j++)
            {
                Roadmap[i, j] = new Node(i, j, -1);
                switch (gameTable.gameMap[i, j])
                {
                    case Terrain.HILL:
                        WMap[i, j] = new Node(i, j, 2);
                        break;
                    case Terrain.RIVER:
                        WMap[i, j] = new Node(i, j, 2);
                        break;
                    case Terrain.POND:
                        WMap[i, j] = new Node(i, j, -1);
                        break;
                    case Terrain.ENTRANCE:
                        WMap[i,j] = new Node(i, j, 1);
                        Roadmap[i,j] = new Node(i,j, 1);
                        _enterance = gameTable.CellToWorld(new Vector3Int(i, j));
                        break;
                    case Terrain.EXIT:
                        WMap[i, j] = new Node(i, j, 1);
                        Roadmap[i, j] = new Node(i, j, 1);
                        _exit = gameTable.CellToWorld(new Vector3Int(i, j));
                        break;
                    default:
                        WMap[i, j] = new Node(i, j, 1);
                        break;
                }
            }
        }

        for (int i = 0; i < gameTable.Size.x; i++)
        {
            for (int j = 0; j < gameTable.Size.y; j++)
            {
                if ((i == gameTable.Size.x - 1 || j == gameTable.Size.y - 1 || i == 0 || j == 0) && gameTable.gameMap[i, j] == Terrain.ENTRANCE)
                {
                    _enterance = gameTable.CellToWorld(new Vector3Int(i, j, 0));
                }
                if ((i == gameTable.Size.x - 1 || j == gameTable.Size.y - 1 || i == 0 || j == 0) && gameTable.gameMap[i, j] == Terrain.EXIT)
                {
                    _exit = gameTable.CellToWorld(new Vector3Int(i, j, 0));
                }
            }
        }
    }

    private void OnDisable()
    {
        Map.onMapGenerated -= OnMapGenerated;
    }
}