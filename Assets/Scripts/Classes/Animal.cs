using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;
using System.Collections;
using UnityEditor.PackageManager.Requests;
using static UnityEditor.PlayerSettings;

public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{
    protected float _visionRange;
    protected float _speed;
    protected float turnSpeed;
    protected float pathUpdateMoveThreshold = .1f;
    protected float minPathUpdateTime = .01f;
    protected Vector2 _position;
    private Vector2[] _path;
    protected Vector2 target;
    protected int _size;
    protected int _price;
    protected int _salePrice;
    protected int _age;
    protected int _hunger;
    protected int _thirst;
    protected bool _hasChip;
    protected int _sleepTime;
    private int targetIndex = 0;
    private bool placed;
    public bool Placed { get => placed; set {
            _position = gameObject.transform.position;
            Vector3 pos = GameManager.Instance.GameTable.WorldToCell(_position);
            if (GameManager.Instance.WMap[(int)pos.x, (int)pos.y].passible)
            {
                target = GenerateRandomTarget();
                StartCoroutine(UpdatePath());
                placed = true;
            }     
        } }
    public Sprite _blipIcon;
    public delegate void OnAnimalDestroy();
    public event OnAnimalDestroy onAnimalDestroy;

    public bool IsVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Sprite BlipIcon { get => _blipIcon; set { _blipIcon = value; } }
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

    void OnDestroy()
    {
        onAnimalDestroy?.Invoke();
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
            target = GenerateRandomTarget();
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
                target = GenerateRandomTarget();
                PathManager.RequestPath(new PathRequest(_position, target, OnPathFound));
            }
        }
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = _path[0];

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

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _speed * Time.deltaTime);
            _position = transform.position;
            yield return null;

        }
    }

    public abstract Vector2 GeneratePath();

    public abstract void Eat(IEntity e);


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
