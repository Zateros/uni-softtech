using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
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

    private List<Animal> _animals;
    public List<Vehicle> Vehicles { get; private set; }
    private List<Tourist> _tourists;
    private List<Poacher> _poachers;

    private Load _gameLoader;
    private Save _gameSaver;

    public Map GameTable { get; private set; }
    public List<List<Vector2>> Routes { get; private set; }
    public bool IsGameRunnning { get; private set; }

    public GameManager() { }

    public void StartGame()
    {
        throw new NotImplementedException();
    }

    public void BuyPlant()
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

    public void Buy(IPurchasable e)
    {
        throw new NotImplementedException();
    }

    public void Sell(IPurchasable e)
    {
        throw new NotImplementedException();
    }

    public void Place(IPurchasable e)
    {
        throw new NotImplementedException();
    }

}
