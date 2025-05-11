using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
    protected Vector2 dir;
    protected Vector2 _facing;
    protected Vector2 _position;
    private Vector2[] _path;
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
    protected int _hunger = 100;
    protected readonly int _hungerMax = 100;
    protected int _thirst = 100;
    protected readonly int _thirstMax = 100;
    protected bool _hasChip = false;
    protected bool _asleep = false;
    protected float _sleepDuration = 5f;
    private int targetIndex = 0;
    private bool placed;
    public Sprite _blipIcon;
    public delegate void OnAnimalDestroy();
    public event OnAnimalDestroy onAnimalDestroy;

    public Vector2 Facing { get { return _facing; } }
    public bool Placed
    {
        get => placed; set
        {
            _position = gameObject.transform.position;
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(_position);
            if (GameManager.Instance.WMap[(int)pos.x, (int)pos.y].passible)
            {
                StartCoroutine(UpdatePath());
                placed = true;
            }
        }
    }
    public bool IsVisible { get => _hasChip; }
    public Sprite BlipIcon { get => _blipIcon; set { _blipIcon = value; } }
    public bool IsAsleep { get => _asleep; }
    public bool IsAdult { get => _age >= 5; }
    public bool IsThirsty { get => _thirst <= _thirstMax / 2; }
    public bool IsHungry { get => _hunger <= _hungerMax / 2; }
    public bool IsCaptured { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool HasChip { get => _hasChip; set => _hasChip = value; }
    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }

    public void Awake()
    {
        _path = new Vector2[0];
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
        Vector2 target = (UnityEngine.Random.insideUnitCircle * _visionRange).normalized;
        Vector3 pos = GameManager.Instance.GameTable.WorldToCell(target);
        while (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
        {
            target = (UnityEngine.Random.insideUnitCircle * _visionRange).normalized;
            pos = GameManager.Instance.GameTable.WorldToCell(target);
        }
        return target;
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
            dir = GenerateRandomTarget();
            dir = dir.normalized;
            target = (_position + dir * _position.magnitude) / 2;
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(target);
            if (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
            {
                var tx = target.x;
                target.x = -target.y * 2;
                target.y = -tx * 2;
            }
            StartCoroutine(UpdatePath());
        }
    }

    IEnumerator UpdatePath()
    {
        Vector3Int pos;
        if (_age == _maxage)
        {
            Die();
            yield break;
        }
        if (IsThirsty)
        {
            if (_foundWater)
            {
                if (Vector2.Distance(_position, _lastWaterSource) > GameManager.Instance.eps)
                {
                    target = _lastWaterSource;
                }
                else
                {
                    Drink();
                }
            }
            else
            {
                target = (_position + GenerateRandomTarget() * _position.magnitude) / 2;
            }
        }
        else if (IsHungry)
        {
            if (this is Herbivore)
            {
                if (_foundFood)
                {
                    if (_position != _lastFoodSource && target != _lastFoodSource)
                    {
                        target = _lastFoodSource;
                    }
                    else if (_position == _lastFoodSource)
                    {
                        pos = GameManager.Instance.GameTable.WorldToCell(target);
                        Eat(GameManager.Instance.Plants[pos.x, pos.y]);
                    }
                }
                else
                {
                    target = (_position + GenerateRandomTarget() * _position.magnitude) / 2;
                }
            }
            else
            {
                target = (_position + GenerateRandomTarget() * _position.magnitude) / 2;
            }
        }
        else
        {
            dir = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
            dir = dir.normalized;
            target = (_position + dir * _position.magnitude) / 2;
            pos = GameManager.Instance.GameTable.WorldToCell(target);
            if (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
            {
                var tx = target.x;
                target.x = -target.y * 2;
                target.y = -tx * 2;
            }
        }
        yield return new WaitForSeconds(.1f);
        PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
        while (true)
        {
            if (_age == _maxage || _thirst == 0 || _hunger == 0)
            {
                Die();
                yield break;
            }
            yield return new WaitForSeconds(minPathUpdateTime);
            if (_path.Length == 0) yield break;
            if (IsThirsty)
            {
                if (_foundWater)
                {
                    if (Vector2.Distance(_position, _lastWaterSource) > GameManager.Instance.eps && target != _lastWaterSource)
                    {
                        target = _lastWaterSource;
                        targetIndex = _path.Length;
                    }
                    else if (Vector2.Distance(_position, _lastWaterSource) <= GameManager.Instance.eps)
                    {
                        Drink();
                    }
                }
                else
                {
                    target = (_position + GenerateRandomTarget() * _position.magnitude) / 2;
                }
            }
            else if (IsHungry)
            {
                if (this is Herbivore)
                {
                    if (_foundFood)
                    {
                        if (_position != _lastFoodSource && target != _lastFoodSource)
                        {
                            target = _lastFoodSource;
                            targetIndex = _path.Length;
                        }
                        else if (_position == _lastFoodSource)
                        {
                            pos = GameManager.Instance.GameTable.WorldToCell(target);
                            Eat(GameManager.Instance.Plants[pos.x, pos.y]);
                        }
                    }
                }
                else
                {
                    var food = FoodNearBy();
                    if (food != null)
                    {
                        if (Vector2.Distance(_position, food._position) <= GameManager.Instance.eps)
                        {
                            Eat(food);
                        }
                        else
                        {
                            target = _lastWaterSource;
                            targetIndex = _path.Length;
                        }
                    }
                }
            }
            else if (targetIndex >= _path.Length)
            {
                targetIndex = 0;
                dir = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
                dir = dir.normalized;
                target = (_position + dir * _position.magnitude) / 2;
                pos = GameManager.Instance.GameTable.WorldToCell(target);
                if (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
                {
                    var tx = target.x;
                    target.x = -target.y * 2;
                    target.y = -tx * 2;
                }
                PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
            }
            if (targetIndex >= _path.Length)
            {
                PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
            }
        }
    }

    private Herbivore FoodNearBy()
    {
        foreach (var herbivore in GameManager.Instance.Herbivores)
        {
            if (Vector2.Distance(herbivore._position, _position) <= _visionRange)
            {
                return herbivore;
            }
        }
        return null;
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        dir = currentWaypoint.normalized;
        while (true)
        {
            if (Vector2.Distance((Vector2)transform.position, currentWaypoint) <= GameManager.Instance.eps)
            {
                targetIndex++;
                if (targetIndex >= _path.Length)
                {
                    yield break;
                }
                currentWaypoint = _path[targetIndex];
                dir = currentWaypoint.normalized;
            }
            if (IsThirsty && _foundWater && target != _lastWaterSource)
            {
                target = _lastWaterSource;
                targetIndex = _path.Length;
                yield break;
            }
            Vector3Int pos = GameManager.Instance.GameTable.WorldToCell(_position);
            var WMap = GameManager.Instance.WMap;
            var Map = GameManager.Instance.GameTable;
            var PMap = GameManager.Instance.Plants;
            float speed = _speed / WMap[pos.x, pos.y].weigth;
            for (int i = 1; i <= (int)_visionRange; i++)
            {
                if (pos.x - i >= 0 && pos.y - i >= 0)
                {
                    if (!WMap[pos.x - i, pos.y - i].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
                if (pos.x - i >= 0)
                {
                    if (!WMap[pos.x - i, pos.y].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
                if (pos.x - i >= 0 && pos.y + i <= GameManager.Instance.GameTable.Size.y)
                {
                    if (!WMap[pos.x - i, pos.y + i].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
                if (pos.y - i >= 0)
                {
                    if (!WMap[pos.x, pos.y - i].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
                if (pos.y + i <= GameManager.Instance.GameTable.Size.y)
                {
                    if (!WMap[pos.x, pos.y + i].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
                if (pos.x + i <= GameManager.Instance.GameTable.Size.x && pos.y - i >= 0)
                {
                    if (!WMap[pos.x + i, pos.y - i].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
                if (pos.x + i <= GameManager.Instance.GameTable.Size.x)
                {
                    if (!WMap[pos.x + i, pos.y].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
                if (pos.x + i <= GameManager.Instance.GameTable.Size.x && pos.y + i <= GameManager.Instance.GameTable.Size.y)
                {
                    if (!WMap[pos.x + i, pos.y + i].passible)
                    {
                        _lastWaterSource = GameManager.Instance.GameTable.CellToWorld(new Vector3Int(pos.x, pos.y));
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
            var prevpos = _position;
            if (speed <= 0)
            {
                speed *= -1;
                Debug.Log(speed);
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            _position = transform.position;
            _facing = (currentWaypoint - _position).normalized;
            if (prevpos == _position)
            {
                OnPathFound(new Vector2[0], false);
                yield break;
            }
            yield return null;
        }
    }

    public abstract IEnumerable Eat(IEntity e);
    public abstract List<Animal> GetNeighbours(float range);
    public abstract void Die();

    IEnumerator Drink()
    {
        Debug.Log("Drink");
        _thirst = _thirstMax;
        Debug.Log(IsThirsty);
        yield return new WaitForSeconds(1);
        yield break;
    }

    public void Mate()
    {
        throw new NotImplementedException();
    }
}
