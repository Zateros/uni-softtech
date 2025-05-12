using System.Collections;
using UnityEngine;

public class Turist : MonoBehaviour, IEntity
{
    private readonly float _visionRange = 2f;
    public int satisfaction;
    private bool inJeep = false;
    private Vehicle vehicle = null;
    protected Vector2 dir;
    protected Vector2 _position;
    Vector2 _facing = Vector2.zero;

    public bool IsVisible { get => true; }
    public int Satisfaction { get => satisfaction; }
    public Vector2 Facing { get => _facing; }

    private void Awake()
    {
        _position = transform.position;

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.EASY:
                satisfaction = 70;
                break;
            case Difficulty.MEDIUM:
                satisfaction = 60;
                break;
            case Difficulty.HARD:
                satisfaction = 50;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (!inJeep)
        {
            vehicle = PickNearestVehicle();
            if (vehicle != null)
            {
                bool succes = vehicle.Enter(this);
                inJeep = succes;
            }
        }
    }

    private Vehicle PickNearestVehicle()
    {
        Vehicle closest = null;
        float distance = -1;
        foreach (Vehicle vehicle in GameManager.Instance.Vehicles)
        {
            if (distance == -1 && Vector2.Distance(_position, vehicle.Position) <= _visionRange && !vehicle.IsFull)
            {
                closest = vehicle;
                distance = Vector2.Distance(_position, vehicle.Position);
            }
            else if (Vector2.Distance(_position, vehicle.Position) < distance && !vehicle.IsFull)
            {
                closest = vehicle;
                distance = Vector2.Distance(_position, vehicle.Position);
            }
        }
        return closest;
    }
}
