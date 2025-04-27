using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{
    protected float _alligmentPriority;
    protected float _FOV;
    protected float _visionRange;
    protected float _speed;
    protected float turnSpeed;
    protected float pathUpdateMoveThreshold = .1f;
    protected float minPathUpdateTime = .01f;
    protected Vector2 _velocity;
    protected Vector2 _position;
    private Vector2[] _path;
    protected Vector2 target;
    protected int _size;
    protected int _price;
    protected int _salePrice;
    protected int _age;
    protected int _hunger = 100;
    protected int _thirst = 100;
    protected bool _hasChip;
    protected int _sleepTime;
    private int targetIndex = 0;
    private bool placed;
    public bool Placed { get => placed; set {
            _position = gameObject.transform.position;
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(_position);
            if (GameManager.Instance.WMap[(int)pos.x, (int)pos.y].passible)
            {
                target = GenerateRandomTarget() + Cohesion();
                StartCoroutine(UpdatePath());
                placed = true;
            }     
        } }

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsAsleep { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsAdult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsThirsty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsHungry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsCaptured { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool HasChip { get => _hasChip; set => _hasChip = value; }
    public int Price { get => _price; }
    public int SalePrice { get => _salePrice; }

    public void Awake()
    {
        _hasChip = false;
        _path = new Vector2[0];
    }

    Vector2 Cohesion()
    {
        Vector2 cohesion = new Vector2();
        int cnt = 0;
        var neighbours = GetNeighbours();
        if(neighbours.Count == 0) return cohesion;
        foreach (var neighbour in neighbours)
        {
            if(inFOV(neighbour._position))
            {
                cohesion += neighbour._position;
                cnt++;
            }
        }
        if (cnt == 0) return cohesion;
        cohesion /= cnt;
        cohesion = cohesion - _position;
        cohesion = Vector3.Normalize(cohesion);
        return cohesion;
    }

    private bool inFOV(Vector2 pos)
    {
        return Vector2.Angle(_velocity, pos - this._position) <= _FOV;
    }

    private Vector2 GenerateRandomTarget()
    {
        Vector2 target = _position + UnityEngine.Random.insideUnitCircle * _visionRange;
        Vector3 pos = GameManager.Instance.GameTable.WorldToCell(target);
        while (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
        {
            target = _position + UnityEngine.Random.insideUnitCircle * _visionRange;
            pos = GameManager.Instance.GameTable.WorldToCell(target);
        }
        return target;
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        
        if (pathSuccessful)
        {
            _path = waypoints;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            StopCoroutine(UpdatePath());
            target = GenerateRandomTarget() + Cohesion();
            StartCoroutine(UpdatePath());
        }
    }

    IEnumerator UpdatePath()
    {
        yield return new WaitForSeconds(.1f);
        PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if (_path.Length == 0) yield break;
            if (targetIndex >= _path.Length)
            {
                targetIndex = 0;
                target = GenerateRandomTarget() + Cohesion();
                PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
            }
        }
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        _velocity = currentWaypoint;
        while (true)
        {
            if ((Vector2)transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= _path.Length)
                {
                    yield break;
                }
                currentWaypoint = _path[targetIndex];
            }
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(_position);
            float speed = _speed / GameManager.Instance.WMap[(int)pos.x,(int)pos.y].weigth;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            _position = transform.position;
            yield return null;

        }
    }

    public abstract void Eat(IEntity e);
    public abstract List<Animal> GetNeighbours();

public void Drink()
    {
        throw new NotImplementedException();
    }

    public void Mate()
    {
        throw new NotImplementedException();
    }

    public void Move(Vector2 goal)
    {
        throw new NotImplementedException();
    }
}
