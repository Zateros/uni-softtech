using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEngine.Rendering.DebugUI;

public class Vehicle : MonoBehaviour, IEntity, IPurchasable
{
    public readonly float visionRange = 3f;
    private Vector2 _facing;
    private Vector2 _position;
    private readonly float _speed = 2f;
    private int _price;
    private int _salePrice;
    private bool _goingBack = false;
    private bool _atEnd = false;
    private List<Turist> _passengers;
    private List<Herbivore> herbivores;
    private List<Carnivore> carnivores;
    private bool[] typesSeen = new bool[6];
    private Vector2[] _path;
    private int targetIndex = 0;

    public Vector2 Position { get => _position; }
    public bool IsVisible { get => true;}
    public bool IsFull { get => _goingBack || _passengers.Count == 4; }

    public int Price { get => _price; }
    public int SalePrice {  get => _salePrice; }
    private bool placed = false;
    public bool Placed
    {
        get => placed; set
        {
            _path = new Vector2[0];
            _position = transform.position;
            placed = true;
            StartCoroutine(UpdatePath());
        }
    }

    public Vector2 Facing { get => _facing; }

    public void Awake()
    {
        _passengers = new List<Turist>();
        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                _price = 20000;
                _salePrice = 20000;
                break;
            case Difficulty.MEDIUM:
                _price = 25000;
                _salePrice = 22500;
                break;
            case Difficulty.HARD:
                _price = 30000;
                _salePrice = 25000;
                break;
            default:
                break;
        }
        herbivores = new List<Herbivore>();
        carnivores = new List<Carnivore>();
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        targetIndex = 0;
        if (pathSuccessful)
        {
            _path = waypoints;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
        else
        {
            StopCoroutine(FollowPath());
            StopCoroutine(UpdatePath());
            StartCoroutine(UpdatePath());
        }
    }

    IEnumerator UpdatePath()
    {
        _position = transform.position;
        yield return new WaitForSeconds(.1f);
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            if (_goingBack || IsFull)
            {
                if (targetIndex >= _path.Length)
                {
                    if (!_goingBack)
                    {
                        PathManager.RequestPath(new PathRequest(_position, GameManager.Instance.Exit, OnPathFound), true);
                    }
                    else
                    {
                        PathManager.RequestPath(new PathRequest(_position, GameManager.Instance.Entrance, OnPathFound), true);
                    }
                }
            }
        }
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        while (true)
        {
            if (_goingBack || IsFull)
            {
                if (_position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= _path.Length)
                    {
                        _goingBack = !_goingBack;
                        if (_goingBack)
                        {
                            int cnt = 0;
                            for (int i = 0; i < 6; i++)
                            {
                                if (typesSeen[i]) ++cnt;
                            }
                            int satisfaction = _passengers[0].satisfaction + (cnt - 3) * 10 + (herbivores.Count - 3) * 5 + (carnivores.Count - 3) * 5;
                            if(satisfaction < 0) satisfaction = 0;
                            if (satisfaction > 100) satisfaction = 100;
                            if (GameManager.Instance.satisfaction > satisfaction)
                            {
                                
                                GameManager.Instance.satisfaction = GameManager.Instance.satisfaction - (GameManager.Instance.satisfaction - satisfaction) / 2;
                                foreach (Turist turist in _passengers)
                                {
                                    
                                    GameManager.Instance.Turists.Remove(turist);
                                    Destroy(turist.gameObject);
                                }
                                
                                _passengers.Clear();
                                yield break;
                            }
                            else if (GameManager.Instance.satisfaction < satisfaction)
                            {
                                GameManager.Instance.satisfaction = GameManager.Instance.satisfaction + (satisfaction - GameManager.Instance.satisfaction) / 2;
                                foreach (Turist turist in _passengers)
                                {
                                    GameManager.Instance.Turists.Remove(turist);
                                    Destroy(turist.gameObject);
                                }
                                _passengers.Clear();
                                yield break;
                            }
                            else
                            {
                                foreach (Turist turist in _passengers)
                                {
                                    GameManager.Instance.Turists.Remove(turist);
                                    Destroy(turist.gameObject);
                                }
                                _passengers.Clear();
                                yield break;
                            }
                        }
                       
                    }
                    currentWaypoint = _path[targetIndex];
                    if (!_goingBack)
                    {
                        foreach (Rhino rhino in GameManager.Instance.Rhinos)
                        {
                            if (Vector2.Distance(rhino.Position, _position) <= visionRange)
                            {
                                typesSeen[0] = true;
                                herbivores.Add(rhino);
                            }
                        }
                        foreach (Zebra zebra in GameManager.Instance.Zebras)
                        {
                            if (Vector2.Distance(zebra.Position, _position) <= visionRange)
                            {
                                typesSeen[1] = true;
                                herbivores.Add(zebra);
                            }
                        }
                        foreach (Giraffe giraffe in GameManager.Instance.Giraffes)
                        {
                            if (Vector2.Distance(giraffe.Position, _position) <= visionRange)
                            {
                                typesSeen[2] = true;
                                herbivores.Add(giraffe);
                            }
                        }
                        foreach (Lion lion in GameManager.Instance.Lions)
                        {
                            if (Vector2.Distance(lion.Position, _position) <= visionRange)
                            {
                                typesSeen[3] = true;
                                carnivores.Add(lion);
                            }
                        }
                        foreach (Hyena hyena in GameManager.Instance.Hyenas)
                        {
                            if (Vector2.Distance(hyena.Position, _position) <= visionRange)
                            {
                                typesSeen[4] = true;
                                carnivores.Add(hyena);
                            }
                        }
                        foreach (Cheetah cheetah in GameManager.Instance.Cheetahs)
                        {
                            if (Vector2.Distance(cheetah.Position, _position) <= visionRange)
                            {
                                typesSeen[5] = true;
                                carnivores.Add(cheetah);
                            }
                        }
                    }
                    
                }
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);
                _position = transform.position;
                
            }
            yield return null;
        }
    }

    public bool Enter(Turist turist)
    {
        if (IsFull) return false;
        _passengers.Add(turist);
        return true;
    }
}
