using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;


public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{
    protected float _wonderPriority = 70;
    protected float _cohesionPriority = 60;
    protected float _alligmentPriority = 50;
    protected float _seperationPriority = 50;
    protected float _FOV;
    protected float _visionRange;
    protected float _speed;
    protected float turnSpeed;
    protected float minPathUpdateTime = .1f;
    protected Vector2 dir;
    protected Vector2 _position;
    private Vector2[] _path;
    protected Vector2 target;
    protected float _size;
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
                dir = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
                dir = dir.normalized;
                target = _position + dir * _visionRange;
                pos = GameManager.Instance.GameTable.WorldToCell(target);
                if (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
                {
                    target = _position - dir * _visionRange * 2;
                }
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

    Vector2 Seperation()
    {
        Vector2 sepVec = new Vector2();
        var neighbours = GetNeighbours(_size);
        if (neighbours.Count == 0) return sepVec;
        foreach (var neighbour in neighbours)
        {
            if (inFOV(neighbour._position))
            {
                Vector2 movTowards = _position - neighbour._position;
                if(movTowards.magnitude > 0) 
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
        var neighbours = GetNeighbours(_visionRange);
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
        var neighbours = GetNeighbours(_visionRange);
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
            dir = GenerateRandomTarget();
            dir = dir.normalized;
            target = _position + dir * _visionRange;
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(target);
            if (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
            {
                target = _position - dir * _visionRange * 2;
            }
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
                dir = GenerateRandomTarget() * _wonderPriority + Cohesion() * _cohesionPriority + Alligment() * _alligmentPriority + Seperation() * _seperationPriority;
                dir = dir.normalized;
                Vector3 pos = GameManager.Instance.GameTable.WorldToCell(target);
                if (pos.x < 0 || pos.y < 0 || pos.x >= GameManager.Instance.GameTable.Size.x || pos.y >= GameManager.Instance.GameTable.Size.y)
                {
                    target = _position - dir * _visionRange * 2;
                }
                PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
            }
        }
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];
        dir = currentWaypoint;
        dir = dir.normalized;
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
                dir = currentWaypoint;
                dir = dir.normalized;
            }
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(_position);
            float speed = _speed / GameManager.Instance.WMap[(int)pos.x,(int)pos.y].weigth;
            var prevpos = _position;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            _position = transform.position;
            if(prevpos == _position)
            {
                yield break;
            }
            yield return null;

        }
    }

    public abstract void Eat(IEntity e);
    public abstract List<Animal> GetNeighbours(float range);

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
