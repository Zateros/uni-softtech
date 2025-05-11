using System;
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

    private Map gameTable;

    void Awake()
    {
        gameTable = GameManager.Instance.GameTable;
    }

    void Start()
    {
        DepthSorting sorting;

        System.Random rnd = new System.Random();
        int chosenAnimal;
        int x;
        int y;

        for (int i = 0; i < GameManager.Instance.MinHerbivoreCount; i++)
        {
            x = rnd.Next(gameTable.Size.x);
            y = rnd.Next(gameTable.Size.y);
            while (!gameTable.IsInBounds(x, y) && gameTable.gameMap[x, y] == Terrain.RIVER && gameTable.gameMap[x, y] == Terrain.POND && gameTable.gameMap[x, y] == Terrain.ENTRANCE && gameTable.gameMap[x, y] == Terrain.EXIT)
            {
                x = rnd.Next(gameTable.Size.x);
                y = rnd.Next(gameTable.Size.y);
            }
            Vector3Int pos = new(x,y);

            chosenAnimal = rnd.Next(1, 4);
            switch (chosenAnimal)
            {
                case 1:
                    var myRhino = Instantiate(Rhino, gameTable.CellToWorld(pos), Quaternion.identity);
                    myRhino.GetComponent<FollowMouse>().enabled = false;
                    myRhino.name = "Rhino";
                    myRhino.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Rhinos.Add(myRhino.GetComponent<Rhino>());
                    GameManager.Instance.Minimap.AddBlip(myRhino);
                    sorting = myRhino.GetComponent<DepthSorting>() ?? myRhino.GetComponentInChildren<DepthSorting>();
                    break;
                case 2:
                    var myZebra = Instantiate(Zebra, gameTable.CellToWorld(pos), Quaternion.identity);
                    myZebra.GetComponent<FollowMouse>().enabled = false;
                    myZebra.name = "Zebra";
                    myZebra.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Zebras.Add(myZebra.GetComponent<Zebra>());
                    GameManager.Instance.Minimap.AddBlip(myZebra);
                    sorting = myZebra.GetComponent<DepthSorting>() ?? myZebra.GetComponentInChildren<DepthSorting>();
                    break;
                case 3:
                    var myGiraffe = Instantiate(Giraffe, gameTable.CellToWorld(pos), Quaternion.identity);
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

        for (int i = 0; i < GameManager.Instance.MinCarnivoreCount; i++)
        {
            x = rnd.Next(gameTable.Size.x);
            y = rnd.Next(gameTable.Size.y);
            while (!gameTable.IsInBounds(x, y) && gameTable.gameMap[x, y] == Terrain.RIVER && gameTable.gameMap[x, y] == Terrain.POND && gameTable.gameMap[x, y] == Terrain.ENTRANCE && gameTable.gameMap[x, y] == Terrain.EXIT)
            {
                x = rnd.Next(gameTable.Size.x);
                y = rnd.Next(gameTable.Size.y);
            }
            Vector3Int pos = new(x, y);

            chosenAnimal = rnd.Next(1, 4);
            switch (chosenAnimal)
            {
                case 1:
                    var myLion = Instantiate(Lion, gameTable.CellToWorld(pos), Quaternion.identity);
                    myLion.GetComponent<FollowMouse>().enabled = false;
                    myLion.name = "Lion";
                    myLion.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Lions.Add(myLion.GetComponent<Lion>());
                    GameManager.Instance.Minimap.AddBlip(myLion);
                    sorting = myLion.GetComponent<DepthSorting>() ?? myLion.GetComponentInChildren<DepthSorting>();
                    break;
                case 2:
                    var myHyena = Instantiate(Hyena, gameTable.CellToWorld(pos), Quaternion.identity);
                    myHyena.GetComponent<FollowMouse>().enabled = false;
                    myHyena.name = "Hyena";
                    myHyena.GetComponent<IPurchasable>().Placed = true;
                    GameManager.Instance.Hyenas.Add(myHyena.GetComponent<Hyena>());
                    GameManager.Instance.Minimap.AddBlip(myHyena);
                    sorting = myHyena.GetComponent<DepthSorting>() ?? myHyena.GetComponentInChildren<DepthSorting>();
                    break;
                case 3:
                    var myCheetah = Instantiate(Cheetah, gameTable.CellToWorld(pos), Quaternion.identity);
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
}
