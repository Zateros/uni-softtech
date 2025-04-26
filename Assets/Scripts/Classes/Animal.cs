using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.IO;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public abstract class Animal : MonoBehaviour, IEntity, IPurchasable
{
    protected float _visionRange;
    protected float _speed;
    protected float turnSpeed;
    protected float pathUpdateMoveThreshold = .5f;
    protected float minPathUpdateTime = .2f;
    protected float turnDst = 5;
    protected float stoppingDst = 10;
    protected Vector2 _position;
    private Path _path;
    protected Vector2 target;
    protected int _size;
    protected int _price;
    protected int _salePrice;
    protected int _age;
    protected int _hunger;
    protected int _thirst;
    protected bool _hasChip;
    protected int _sleepTime;
    private int framecnt = 1200;
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
        _position = gameObject.transform.position;
        target = GenerateRandomTarget();
        StartCoroutine(UpdatePath());
    }

    private Vector2 GenerateRandomTarget()
    {
        return UnityEngine.Random.onUnitSphere * _visionRange;
    }

    void OnDestroy()
    {
        onAnimalDestroy?.Invoke();
    }

    void OnDestroy()
    {
        onAnimalDestroy?.Invoke();
    }

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _path = new Path(waypoints, transform.position, turnDst, stoppingDst);

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {

        if (Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }
        PathManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector2 targetPosOld = target;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));
                targetPosOld = target;
            }
        }
    }

    IEnumerator FollowPath()
    {

        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(_path.lookPoints[0]);

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (_path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == _path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {

                if (pathIndex >= _path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(_path.turnBoundaries[_path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(_path.lookPoints[pathIndex] - (Vector2)transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * _speed * speedPercent, Space.Self);
            }

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
