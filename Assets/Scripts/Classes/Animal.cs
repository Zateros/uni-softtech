using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{
    protected float _wonderPriority = 10;
    protected float _cohesionPriority = 90;
    protected float _alligmentPriority = 90;
    protected float _seperationPriority = 20;
    protected float _FOV;
    protected float _visionRange;
    protected float _speed;
    protected float _minPathUpdateTime = .01f;
    protected int _childCount = 0;
    protected int _maxChildCount;
    protected Vector2 _dir;
    protected Vector2 _facing = Vector2.zero;
    protected Vector2 _position;
    protected Vector2[] _path;
    protected Vector2 _target;
    protected bool _foundWater = false;
    protected bool _foundFood = false;
    protected Vector2 _lastWaterSource;
    protected Vector2 _lastFoodSource;
    protected float _size;
    protected int _price;
    protected int _salePrice;
    public int age = 0;
    protected int _maxage = 10;
    public int hunger;
    protected readonly int _hungerMax = 100;
    public int thirst;
    protected readonly int _thirstMax = 50;
    protected bool _hasChip = false;
    protected bool _asleep = false;
    protected float _sleepDuration = 5f;
    private int _targetIndex = 0;
    private bool _placed;
    public Vector2 Position { get => _position; }
    public Sprite blipIcon;
    public delegate void OnAnimalDestroy();
    public event OnAnimalDestroy onAnimalDestroy;

    public Vector2 Facing { get => _facing; }
    /// <summary>
    /// Sets the position and start everything after the animal is placed
    /// </summary>
    public bool Placed
    {
        get => _placed; set
        {
            _position = gameObject.transform.position;
            StartCoroutine(UpdatePath());
            _placed = true;
        }
    }

    public bool IsVisible { get => _hasChip; }
    public Sprite BlipIcon { get => blipIcon; set { blipIcon = value; } }
    public bool IsAsleep { get => _asleep; }
    public bool IsAdult { get => age >= 5; }
    public bool IsThirsty { get => thirst <= _thirstMax / 2; }
    public bool IsHungry { get => hunger <= _hungerMax / 2; }
    public bool HasChip { get => _hasChip; set => _hasChip = value; }
    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }
    public static int DestroyExtraCost { get => 0; }

    public void Awake()
    {
        _path = new Vector2[0];
        hunger = _hungerMax;
        thirst = _thirstMax;
    }
    /// <summary>
    /// Calculates seperation
    /// </summary>
    /// <returns>A vector towrds the seperation direction</returns>
    Vector2 Seperation()
    {
        Vector2 sepVec = new Vector2();
        var neighbours = GetNeighbours(_size / 2);
        if (neighbours.Count == 0) return sepVec;
        foreach (var neighbour in neighbours)
        {
            if (InFOV(neighbour._position))
            {
                Vector2 movTowards = _position - neighbour._position;
                if (movTowards.magnitude > 0)
                {
                    sepVec += movTowards.normalized / movTowards.magnitude;
                }
            }
        }
        return sepVec.normalized;
    }

    /// <summary>
    /// Calculates alligment
    /// </summary>
    /// <returns>A vector towrds the alligment direction</returns>
    Vector2 Alligment()
    {
        Vector2 allign = new Vector2();
        var neighbours = GetNeighbours(_visionRange / 2);
        if (neighbours.Count == 0) return allign;
        foreach (var neighbour in neighbours)
        {
            if (InFOV(neighbour._position))
            {
                allign += neighbour._dir;
            }
        }
        allign = allign.normalized;
        return allign;
    }

    /// <summary>
    /// Calculates cohesion
    /// </summary>
    /// <returns>A vector towrds the cohesion direction</returns>
    Vector2 Cohesion()
    {
        Vector2 cohesion = new Vector2();
        int cnt = 0;
        var neighbours = GetNeighbours(_visionRange / 2);
        if (neighbours.Count == 0) return cohesion;
        foreach (var neighbour in neighbours)
        {
            if (InFOV(neighbour._position))
            {
                cohesion += neighbour._position;
                cnt++;
            }
        }
        if (cnt == 0) return cohesion;
        cohesion /= cnt;
        cohesion = cohesion - _position;
        cohesion = cohesion.normalized;
        return cohesion;
    }

    /// <summary>
    /// Checks if the animal can see teh given point
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool InFOV(Vector2 pos)
    {
        return Vector2.Angle(_dir, pos - _position) <= _FOV;
    }

    /// <summary>
    /// Generates random direction
    /// </summary>
    /// <returns>Random direction</returns>
    private Vector2 GenerateRandomTarget()
    {
        return UnityEngine.Random.insideUnitCircle;
    }

    void OnDestroy()
    {
        onAnimalDestroy?.Invoke();
    }

    /// <summary>
    /// Handles A* returns
    /// </summary>
    /// <param name="waypoints"></param>
    /// <param name="pathSuccessful"></param>
    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        _targetIndex = 0;
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

    /// <summary>
    /// Path generation and update loop, handles animal logic
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdatePath()
    {
        _position = transform.position;
        Vector3Int pos;
        if (age == _maxage || thirst == 0 || hunger == 0)
        {
            Die();
            yield break;
        }
        pos = GameManager.Instance.GameTable.WorldToCell(_position);
        if (IsThirsty && (GameManager.Instance.GameTable.gameMap[pos.x, pos.y] == Terrain.POND || GameManager.Instance.GameTable.gameMap[pos.x, pos.y] == Terrain.RIVER))
        {
            StartCoroutine(Drink());
        }
        if (IsThirsty)
        {
            if (_foundWater)
            {
                if (Vector2.Distance(_lastWaterSource, _position) > 1)
                {
                    _target = _lastWaterSource;
                }
                else
                {
                    StartCoroutine(Drink());
                }
            }
            else
            {
                _target = _position + GenerateRandomTarget() * _position.magnitude;
            }
        }
        else if (IsHungry)
        {
            if (_foundFood)
            {
                if (Vector2.Distance(_lastFoodSource, _position) > 1)
                {
                    _target = _lastFoodSource;
                }
                else if (this is Herbivore)
                {
                    pos = GameManager.Instance.GameTable.WorldToCell(_lastFoodSource);
                    StartCoroutine(Eat(GameManager.Instance.Plants[pos.x, pos.y]));
                }
            }
            else if (this is Carnivore)
            {
                Herbivore food = FoodNearBy();
                if (food != null)
                {
                    if (Vector2.Distance(_position, food._position) <= 1)
                    {
                        StartCoroutine(Eat(food));
                    }
                    else
                    {
                        Debug.Log(food._position);
                        _target = food._position;
                    }
                }
                else
                {
                    _target = _position + GenerateRandomTarget() * _position.magnitude;
                }
            }
            else
            {
                _target = _position + GenerateRandomTarget() * _position.magnitude;
            }
        }
        else
        {
            Vector2 tmp = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
            tmp = tmp.normalized;
            _target = (_position + tmp * _position.magnitude);
        }
        yield return new WaitForSeconds(.1f);
        PathManager.RequestPath(new PathRequest(_position, _target, OnPathFound),false);
        while (true)
        {
            if (age == _maxage || thirst == 0 || hunger == 0)
            {
                Die();
                yield break;
            }
            yield return new WaitForSeconds(_minPathUpdateTime);
            if (_path.Length == 0)
            {
                yield break;
            }
            pos = GameManager.Instance.GameTable.WorldToCell(_position);
            if (IsThirsty && (GameManager.Instance.GameTable.gameMap[pos.x, pos.y] == Terrain.POND || GameManager.Instance.GameTable.gameMap[pos.x, pos.y] == Terrain.RIVER))
            {
                StartCoroutine(Drink());
            }
            if (_targetIndex >= _path.Length)
            {
                _targetIndex = 0;
                if (IsThirsty)
                {

                    if (_foundWater)
                    {
                        if (Vector2.Distance(_lastWaterSource, _position) > 1)
                        {
                            _target = _lastWaterSource;
                        }
                        else
                        {
                            StartCoroutine(Drink());
                        }
                    }
                    else
                    {
                        _target = (_position + GenerateRandomTarget() * _position.magnitude);
                    }
                }
                else if (IsHungry)
                {
                    if (_foundFood)
                    {
                        if (Vector2.Distance(_lastFoodSource, _position) > 1)
                        {
                            _target = _lastFoodSource;
                        }
                        else if (this is Herbivore)
                        {
                            pos = GameManager.Instance.GameTable.WorldToCell(_lastFoodSource);
                            StartCoroutine(Eat(GameManager.Instance.Plants[pos.x, pos.y]));
                        }
                    }
                    else if (this is Carnivore)
                    {
                        Herbivore food = FoodNearBy();
                        if (food != null)
                        {
                            if (Vector2.Distance(_position, food._position) <= 1)
                            {
                                StartCoroutine(Eat(food));
                            }
                            else
                            {
                                _target = food._position;
                            }
                        }
                        else
                        {
                            _target = _position + GenerateRandomTarget() * _position.magnitude;
                        }
                    }
                    else
                    {
                        _target = _position + GenerateRandomTarget() * _position.magnitude;
                    }
                }
                else
                {
                    Vector2 tmp = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
                    tmp = tmp.normalized;
                    _target = (_position + tmp * _position.magnitude);
                }
                PathManager.RequestPath(new PathRequest(_position, _target, OnPathFound),false);
            }
        }
    }

    /// <summary>
    /// Searches for nearby Herbivores
    /// </summary>
    /// <returns>A herbivore if there is one in vison</returns>
    private Herbivore FoodNearBy()
    {
        Herbivore closest = null;
        float distance = -1;
        foreach (Herbivore herbivore in GameManager.Instance.Herbivores)
        {
            if (distance == -1 && Vector2.Distance(_position, herbivore._position) <= _visionRange)
            {
                closest = herbivore;
                distance = Vector2.Distance(_position, herbivore._position);
            }
            else if (Vector2.Distance(_position, herbivore._position) <= distance)
            {
                closest = herbivore;
                distance = Vector2.Distance(_position, herbivore._position);
            }
        }
        return closest;
    }

    /// <summary>
    /// Follows the give path
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        _dir = currentWaypoint.normalized;
        while (true)
        {
            if (!_asleep)
            {
                if (_position == currentWaypoint)
                {
                    _targetIndex++;
                    if (_targetIndex >= _path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = _path[_targetIndex];
                    _dir = currentWaypoint.normalized;
                }
                Vector3Int pos = GameManager.Instance.GameTable.WorldToCell(_position);
                var WMap = GameManager.Instance.WMap;
                var Map = GameManager.Instance.GameTable.gameMap;
                var PMap = GameManager.Instance.Plants;
                float speed = _speed / WMap[pos.x, pos.y].weigth;
                if (IsThirsty && _foundWater)
                {
                    if (Vector2.Distance(_lastWaterSource, _position) <= 1)
                    {
                        StartCoroutine(Drink());
                        _targetIndex = _path.Length;
                        yield break;
                    }
                }
                else if (IsHungry && _foundFood)
                {
                    if (Vector2.Distance(_lastFoodSource, _position) <= 1)
                    {
                        pos = GameManager.Instance.GameTable.WorldToCell(_lastFoodSource);
                        StartCoroutine(Eat(GameManager.Instance.Plants[pos.x, pos.y]));
                        _targetIndex = _path.Length;
                        yield break;
                    }
                }
                for (int i = 1; i <= (int)_visionRange; i++)
                {
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x - i, pos.y - i))
                    {
                        if (Map[pos.x - i, pos.y - i] == Terrain.POND || Map[pos.x - i, pos.y - i] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x - i, pos.y - i));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x - i, pos.y - i] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x - i, pos.y - i));
                            _foundFood = true;
                        }
                    }
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x - i, pos.y))
                    {
                        if (Map[pos.x - i, pos.y] == Terrain.POND || Map[pos.x - i, pos.y] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x - i, pos.y));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x - i, pos.y] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x - i, pos.y));
                            _foundFood = true;
                        }
                    }
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x - i, pos.y + i))
                    {
                        if (Map[pos.x - i, pos.y + i] == Terrain.POND || Map[pos.x - i, pos.y + i] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x - i, pos.y + i));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x - i, pos.y + i] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x - i, pos.y + i));
                            _foundFood = true;
                        }
                    }
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x, pos.y - i))
                    {
                        if (Map[pos.x, pos.y - i] == Terrain.POND || Map[pos.x, pos.y - i] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y - i));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x, pos.y - i] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y - i));
                            _foundFood = true;
                        }
                    }
                    if (this is Herbivore && PMap[pos.x, pos.y] != null)
                    {
                        _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
                        _foundFood = true;
                    }
                    if (Map[pos.x, pos.y] == Terrain.POND || Map[pos.x, pos.y] == Terrain.RIVER)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
                        _foundWater = true;
                    }
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x, pos.y + i))
                    {
                        if (Map[pos.x, pos.y + i] == Terrain.POND || Map[pos.x, pos.y + i] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y + i));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x, pos.y + i] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y + i));
                            _foundFood = true;
                        }
                    }
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x + i, pos.y - i))
                    {
                        if (Map[pos.x + i, pos.y - i] == Terrain.POND || Map[pos.x + i, pos.y - i] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x + i, pos.y - i));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x + i, pos.y - i] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x + i, pos.y - i));
                            _foundFood = true;
                        }
                    }
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x + i, pos.y))
                    {
                        if (Map[pos.x + i, pos.y] == Terrain.POND || Map[pos.x + i, pos.y] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x + i, pos.y));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x + i, pos.y] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x + i, pos.y));
                            _foundFood = true;
                        }
                    }
                    if (GameManager.Instance.GameTable.IsInBounds(pos.x + i, pos.y + i))
                    {
                        if (Map[pos.x + i, pos.y + i] == Terrain.POND || Map[pos.x + i, pos.y + i] == Terrain.RIVER)
                        {
                            _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x + i, pos.y + i));
                            _foundWater = true;
                            if (this is Carnivore)
                            {
                                break;
                            }
                        }
                        else if (this is Herbivore && PMap[pos.x + i, pos.y + i] != null)
                        {
                            _lastFoodSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x + i, pos.y + i));
                            _foundFood = true;
                        }
                    }
                }
                if (speed <= 0)
                {
                    speed *= -1;
                }
                _dir = currentWaypoint.normalized;
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                _position = transform.position;
                _facing = (currentWaypoint - _position).normalized;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Eats the given entity
    /// </summary>
    /// <param name="e">entity to be eaten</param>
    /// <returns></returns>
    public abstract IEnumerator Eat(IEntity e);
    /// <summary>
    /// Returns the nearby animals within the range
    /// </summary>
    /// <param name="range"></param>
    /// <returns>List of the nearby animals</returns>
    public abstract List<Animal> GetNeighbours(float range);
    /// <summary>
    /// The animal dies
    /// </summary>
    public abstract void Die();

    /// <summary>
    /// Drinks water
    /// </summary>
    /// <returns></returns>
    IEnumerator Drink()
    {
        _asleep = true;
        yield return new WaitForSeconds(_sleepDuration / 5);
        _asleep = false;
        thirst = _thirstMax;
        yield break;
    }

    /// <summary>
    /// If 2 adults of the same kind are near eachother and can still have kids, they will mate
    /// </summary>
    public void Mate()
    {
        if (IsAdult && _childCount < _maxChildCount)
        {
            var neighbours = GetNeighbours(_visionRange);
            foreach (var neighbour in neighbours)
            {
                if (neighbour.IsAdult && neighbour._childCount < neighbour._maxChildCount && InFOV(neighbour._position))
                {
                    _childCount++;
                    neighbour._childCount++;
                }
            }
        }
    }
}
