using UnityEngine;

public interface ITargetable
{
    [System.Flags]
    public enum TargetableType
    {
        Card = 1 << 0,
        Player = 1 << 1,
        Environment = 1 << 2,
    }


    public TargetableType GetTargetableType();

    public Player GetOwningPlayer();

    public Vector3 GetTargetPosition();

    public Transform GetTargetTransform();

    public void SetTargetingPreviewVisible(bool visible);
}
