using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnEntities : MonoBehaviour
{
    public GameObject Rhino;
    public GameObject Zebra;
    public GameObject Giraffe;
    public GameObject Lion;
    public GameObject Hyena;
    public GameObject Cheetah;
    public GameObject Jeep;
    public GameObject Turist;

    private Map _gameTable;
    private int _waitTime;
    private System.Random _rnd;
    private int _maxDailyTuristCount;
    private int _dailyTuristCount;

    private DateTime _prevDay;

    void Awake()
    {
        _gameTable = GameManager.Instance.GameTable;
        _rnd = new System.Random();
        _maxDailyTuristCount = 10;
    }

    void Start()
    {
        CycleTime();

        DepthSorting sorting;

        int chosenAnimal;
        int x;
        int y;

        for (int i = 0; i < GameManager.Instance.MinHerbivoreCount + 5; i++)
        {
            x = _rnd.Next(_gameTable.Size.x);
            y = _rnd.Next(_gameTable.Size.y);
            while (!_gameTable.IsInBounds(x, y) && _gameTable.gameMap[x, y] == Terrain.RIVER && _gameTable.gameMap[x, y] == Terrain.POND && _gameTable.gameMap[x, y] == Terrain.ENTRANCE && _gameTable.gameMap[x, y] == Terrain.EXIT)
            {
                x = _rnd.Next(_gameTable.Size.x);
                y = _rnd.Next(_gameTable.Size.y);
            }
            Vector3Int pos = new(x,y);

            chosenAnimal = _rnd.Next(1, 4);
            switch (chosenAnimal)
            {
                case 1:
                    var myRhino = Instantiate(Rhino, _gameTable.CellToWorld(pos), Quaternion.identity);
                    myRhino.GetComponent<FollowMouse>().enabled = false;
                    myRhino.name = "Rhino";
                    myRhino.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Rhinos.Add(myRhino.GetComponent<Rhino>());
                    GameManager.Instance.Minimap.AddBlip(myRhino);
                    sorting = myRhino.GetComponent<DepthSorting>() ?? myRhino.GetComponentInChildren<DepthSorting>();
                    break;
                case 2:
                    var myZebra = Instantiate(Zebra, _gameTable.CellToWorld(pos), Quaternion.identity);
                    myZebra.GetComponent<FollowMouse>().enabled = false;
                    myZebra.name = "Zebra";
                    myZebra.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Zebras.Add(myZebra.GetComponent<Zebra>());
                    GameManager.Instance.Minimap.AddBlip(myZebra);
                    sorting = myZebra.GetComponent<DepthSorting>() ?? myZebra.GetComponentInChildren<DepthSorting>();
                    break;
                case 3:
                    var myGiraffe = Instantiate(Giraffe, _gameTable.CellToWorld(pos), Quaternion.identity);
                    myGiraffe.GetComponent<FollowMouse>().enabled = false;
                    myGiraffe.name = "Giraffe";
                    myGiraffe.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Giraffes.Add(myGiraffe.GetComponent<Giraffe>());
                    GameManager.Instance.Minimap.AddBlip(myGiraffe);
                    sorting = myGiraffe.GetComponent<DepthSorting>() ?? myGiraffe.GetComponentInChildren<DepthSorting>();
                    break;
                default:
                    break;
            }
        }

        for (int i = 0; i < GameManager.Instance.MinCarnivoreCount + 5; i++)
        {
            x = _rnd.Next(_gameTable.Size.x);
            y = _rnd.Next(_gameTable.Size.y);
            while (!_gameTable.IsInBounds(x, y) && _gameTable.gameMap[x, y] == Terrain.RIVER && _gameTable.gameMap[x, y] == Terrain.POND && _gameTable.gameMap[x, y] == Terrain.ENTRANCE && _gameTable.gameMap[x, y] == Terrain.EXIT)
            {
                x = _rnd.Next(_gameTable.Size.x);
                y = _rnd.Next(_gameTable.Size.y);
            }
            Vector3Int pos = new(x, y);

            chosenAnimal = _rnd.Next(1, 4);
            switch (chosenAnimal)
            {
                case 1:
                    var myLion = Instantiate(Lion, _gameTable.CellToWorld(pos), Quaternion.identity);
                    myLion.GetComponent<FollowMouse>().enabled = false;
                    myLion.name = "Lion";
                    myLion.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Lions.Add(myLion.GetComponent<Lion>());
                    GameManager.Instance.Minimap.AddBlip(myLion);
                    sorting = myLion.GetComponent<DepthSorting>() ?? myLion.GetComponentInChildren<DepthSorting>();
                    break;
                case 2:
                    var myHyena = Instantiate(Hyena, _gameTable.CellToWorld(pos), Quaternion.identity);
                    myHyena.GetComponent<FollowMouse>().enabled = false;
                    myHyena.name = "Hyena";
                    myHyena.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Hyenas.Add(myHyena.GetComponent<Hyena>());
                    GameManager.Instance.Minimap.AddBlip(myHyena);
                    sorting = myHyena.GetComponent<DepthSorting>() ?? myHyena.GetComponentInChildren<DepthSorting>();
                    break;
                case 3:
                    var myCheetah = Instantiate(Cheetah, _gameTable.CellToWorld(pos), Quaternion.identity);
                    myCheetah.GetComponent<FollowMouse>().enabled = false;
                    myCheetah.name = "Cheetah";
                    myCheetah.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Cheetahs.Add(myCheetah.GetComponent<Cheetah>());
                    GameManager.Instance.Minimap.AddBlip(myCheetah);
                    sorting = myCheetah.GetComponent<DepthSorting>() ?? myCheetah.GetComponentInChildren<DepthSorting>();
                    break;
                default:
                    break;
            }

        }
    }

    IEnumerator WaitFor(float time)
    {
        yield return new WaitForSeconds(time);
        CycleTime();
    }

    private void CycleTime()
    {
        if((GameManager.Instance.Date - _prevDay).TotalDays == 1)
        {
            _maxDailyTuristCount = GameManager.Instance.satisfaction / 5;
        }

        if (_dailyTuristCount != _maxDailyTuristCount)
        {
            int cnt = _rnd.Next(1, 4);
            _dailyTuristCount = Mathf.Clamp(_dailyTuristCount + cnt, 0, _maxDailyTuristCount);

            for (int i = 0; i < cnt; i++)
            {
                var myturist = Instantiate(Turist, GameManager.Instance.Entrance, Quaternion.identity);
                myturist.name = "Turist";
                GameManager.Instance.Turists.Add(myturist.GetComponent<Turist>());
                GameManager.Instance.Money += GameManager.Instance.entranceFee;
            }

            _waitTime = _rnd.Next(1, 11);
            StartCoroutine(WaitFor(_waitTime));
        }
    }
}
