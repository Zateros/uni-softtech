using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;
using System.Xml;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursor;

    public static GameManager Instance;

    private Map _gameTable;
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

    private List<GameObject> _animals;
    private List<GameObject> _vehicles;
    private List<GameObject> _tourists;
    private List<GameObject> _poachers;

    private Load _gameLoader;
    private Save _gameSaver;

    public Node[,] WMap { get; private set; }
    public List<GameObject> Vehicles { get => _vehicles; }
    public List<GameObject> Animals { get => _animals; }
    public Map GameTable { get => _gameTable; }
    public List<List<Vector2>> Routes { get; private set; } = new List<List<Vector2>>();
    public bool IsGameRunnning { get; private set; }
    public int Money { get => _money;}
    public Difficulty Difficulty { get => _difficulty;}

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

        _animals = new List<GameObject>();
        _vehicles = new List<GameObject>();
        _tourists = new List<GameObject>();
        _poachers = new List<GameObject>();

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);

        for (int i = 0; i < _gameTable.Size.x; i++)
        {
            for (int j = 0; j < _gameTable.Size.y; j++)
            {
                switch (_gameTable.gameMap[i, j])
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
        if (gameObject == null) { throw new NullReferenceException(); }

        switch (gameObject.name)
        {
            case "Rhino":
                _animals.Add(gameObject);
                break;
            case "Zebra":
                _animals.Add(gameObject);
                break;
            case "Giraffe":
                _animals.Add(gameObject);
                break;
            case "Lion":
                _animals.Add(gameObject);
                break;
            case "Hyena":
                _animals.Add(gameObject);
                break;
            case "Cheetah":
                _animals.Add(gameObject);
                break;
            case "Grass":
                
                break;
            case "Bush":
                
                break;
            case "Tree":
                
                break;
            case "Jeep":
                _vehicles.Add(gameObject);
                break;
            case "Road":
                
                break;
            default:
                break;

        }

        int price = gameObject.GetComponent<IPurchasable>().Price;
        _money -= price;
    }

    public void Sell(GameObject gameObject)
    {
        if(_animals.Contains(gameObject))
            _animals.Remove(gameObject);

        if(_vehicles.Contains(gameObject))
            _vehicles.Remove(gameObject);

        int salePrice = gameObject.GetComponent<IPurchasable>().SalePrice;
        _money += salePrice;

        Destroy(gameObject);
    }

}