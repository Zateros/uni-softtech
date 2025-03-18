using UnityEngine;

public interface IEntity
{
    public bool IsVisible { get; set; }

    public void Move();
    public void GeneratePath();
}
