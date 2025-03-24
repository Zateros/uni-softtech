using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
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

    public List<GameObject> Vehicles { get => _vehicles; }
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
        if (gameObject == null) { Debug.Log("OhNo"); }
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
        switch (gameObject.name)
        {
            case "Rhino":
                _animals.Remove(gameObject);
                break;
            case "Zebra":
                _animals.Remove(gameObject);
                break;
            case "Giraffe":
                _animals.Remove(gameObject);
                break;
            case "Lion":
                _animals.Remove(gameObject);
                break;
            case "Hyena":
                _animals.Remove(gameObject);
                break;
            case "Cheetah":
                _animals.Remove(gameObject);
                break;
            case "Grass":

                break;
            case "Bush":

                break;
            case "Tree":

                break;
            case "Jeep":
                _vehicles.Remove(gameObject);
                break;
            case "Road":

                break;
            default:
                break;

        }
        
        int salePrice = gameObject.GetComponent<IPurchasable>().SalePrice;
        _money += salePrice;

        Destroy(gameObject);
    }

}
