using UnityEngine;

public interface ITargetable
{
    [System.Flags]
    public enum TargetableType
    {
        Card,
        Player
    }

    public TargetableType GetTargetType(); 

    public Vector3 GetPositionAsTarget();

    public void SetTargetingPreviewVisible(bool visible);
}
