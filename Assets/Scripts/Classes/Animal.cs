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
    protected float minPathUpdateTime = .01f;
    protected int _childCount = 0;
    protected int _maxChildCount;
    protected Vector2 dir;
    protected Vector2 _position;
    protected Vector2[] _path;
    protected Vector2 target;
    protected bool _foundWater = false;
    protected bool _foundFood = false;
    protected Vector2 _lastWaterSource;
    protected Vector2 _lastFoodSource;
    protected float _size;
    protected int _price;
    protected int _salePrice;
    protected int _age = 1;
    protected int _maxage;
    public int hunger;
    protected readonly int _hungerMax = 100;
    public int thirst;
    protected readonly int _thirstMax = 100;
    protected bool _hasChip = false;
    protected bool _asleep = false;
    protected float _sleepDuration = 5f;
    private int targetIndex = 0;
    private bool placed;
    public bool Placed
    {
        get => placed; set
        {
            _position = gameObject.transform.position;
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(_position);
            StartCoroutine(UpdatePath());
            placed = true;
        }
    }
    public Sprite _blipIcon;
    public delegate void OnAnimalDestroy();
    public event OnAnimalDestroy onAnimalDestroy;

    public bool IsVisible { get => _hasChip; }
    public Sprite BlipIcon { get => _blipIcon; set { _blipIcon = value; } }
    public bool IsAsleep { get => _asleep; }
    public bool IsAdult { get => _age >= 5; }
    public bool IsThirsty { get => thirst <= _thirstMax / 2; }
    public bool IsHungry { get => hunger <= _hungerMax / 2; }
    public bool HasChip { get => _hasChip; set => _hasChip = value; }
    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }

    public void Awake()
    {
        _path = new Vector2[0];
        hunger = _hungerMax;
        thirst = _thirstMax;
    }

    Vector2 Seperation()
    {
        Vector2 sepVec = new Vector2();
        var neighbours = GetNeighbours(_size / 2);
        if (neighbours.Count == 0) return sepVec;
        foreach (var neighbour in neighbours)
        {
            if (inFOV(neighbour._position))
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

    Vector2 Alligment()
    {
        Vector2 allign = new Vector2();
        var neighbours = GetNeighbours(_visionRange / 2);
        if (neighbours.Count == 0) return allign;
        foreach (var neighbour in neighbours)
        {
            if (inFOV(neighbour._position))
            {
                allign += neighbour.dir;
            }
        }
        allign = allign.normalized;
        return allign;
    }

    Vector2 Cohesion()
    {
        Vector2 cohesion = new Vector2();
        int cnt = 0;
        var neighbours = GetNeighbours(_visionRange / 2);
        if (neighbours.Count == 0) return cohesion;
        foreach (var neighbour in neighbours)
        {
            if (inFOV(neighbour._position))
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

    private bool inFOV(Vector2 pos)
    {
        return Vector2.Angle(dir, pos - _position) <= _FOV;
    }

    private Vector2 GenerateRandomTarget()
    {
        return UnityEngine.Random.insideUnitCircle;
    }

    void OnDestroy()
    {
        onAnimalDestroy?.Invoke();
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
        Vector3Int pos;
        if (_age == _maxage || thirst == 0 || hunger == 0)
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
                    target = _lastWaterSource;
                }
                else
                {
                    StartCoroutine(Drink());
                }
            }
            else
            {
                target = _position + GenerateRandomTarget() * _position.magnitude;
            }
        }
        else if (IsHungry)
        {
            if (_foundFood)
            {
                if (Vector2.Distance(_lastFoodSource, _position) > 1)
                {
                    target = _lastFoodSource;
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
                        target = food._position;
                    }
                }
                else
                {
                    target = _position + GenerateRandomTarget() * _position.magnitude;
                }
            }
            else
            {
                target = _position + GenerateRandomTarget() * _position.magnitude;
            }
        }
        else
        {
            dir = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
            dir = dir.normalized;
            target = (_position + dir * _position.magnitude);
        }
        yield return new WaitForSeconds(.1f);
        PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
        while (true)
        {
            if (_age == _maxage || thirst == 0 || hunger == 0)
            {
                Die();
                yield break;
            }
            yield return new WaitForSeconds(minPathUpdateTime);
            if (_path.Length == 0)
            {
                yield break;
            }
            pos = GameManager.Instance.GameTable.WorldToCell(_position);
            if (IsThirsty && (GameManager.Instance.GameTable.gameMap[pos.x, pos.y] == Terrain.POND || GameManager.Instance.GameTable.gameMap[pos.x, pos.y] == Terrain.RIVER))
            {
                StartCoroutine(Drink());
            }
            if (targetIndex >= _path.Length)
            {
                targetIndex = 0;
                if (IsThirsty)
                {

                    if (_foundWater)
                    {
                        if (Vector2.Distance(_lastWaterSource, _position) > 1)
                        {
                            target = _lastWaterSource;
                        }
                        else
                        {
                            StartCoroutine(Drink());
                        }
                    }
                    else
                    {
                        target = (_position + GenerateRandomTarget() * _position.magnitude);
                    }
                }
                else if (IsHungry)
                {
                    if (_foundFood)
                    {
                        if (Vector2.Distance(_lastFoodSource, _position) > 1)
                        {
                            target = _lastFoodSource;
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
                                target = food._position;
                            }
                        }
                        else
                        {
                            target = _position + GenerateRandomTarget() * _position.magnitude;
                        }
                    }
                    else
                    {
                        target = _position + GenerateRandomTarget() * _position.magnitude;
                    }
                }
                else
                {
                    dir = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
                    dir = dir.normalized;
                    target = (_position + dir * _position.magnitude);
                }
                PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
            }
        }
    }

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

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        dir = currentWaypoint.normalized;
        while (true)
        {
            if (!_asleep)
            {
                if (_position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= _path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = _path[targetIndex];
                    dir = currentWaypoint.normalized;
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
                        targetIndex = _path.Length;
                        yield break;
                    }
                }
                else if (IsHungry && _foundFood)
                {
                    if (Vector2.Distance(_lastFoodSource, _position) <= 1)
                    {
                        pos = GameManager.Instance.GameTable.WorldToCell(_lastFoodSource);
                        StartCoroutine(Eat(GameManager.Instance.Plants[pos.x, pos.y]));
                        targetIndex = _path.Length;
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
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                _position = transform.position;
            }
            yield return null;
        }
    }

    public abstract IEnumerator Eat(IEntity e);
    public abstract List<Animal> GetNeighbours(float range);
    public abstract void Die();

    IEnumerator Drink()
    {
        _asleep = true;
        yield return new WaitForSeconds(_sleepDuration / 5);
        _asleep = false;
        thirst = _thirstMax;
        yield break;
    }

    public void Mate()
    {
        if (IsAdult && _childCount < _maxChildCount)
        {
            var neighbours = GetNeighbours(_visionRange);
            foreach (var neighbour in neighbours)
            {
                if (neighbour.IsAdult && neighbour._childCount < neighbour._maxChildCount && inFOV(neighbour._position))
                {
                    _childCount++;
                    neighbour._childCount++;
                }
            }
        }
    }
}
