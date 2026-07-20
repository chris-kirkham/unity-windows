using UnityEngine;

public class PlayerTargetingVisualiserLine : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    private Transform target;

    public void SetTargetTransform(Transform target)
    {
        this.target = target;
    }

    private void LateUpdate()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        if(!line || !target)
        {
            return;
        }

        line.SetPosition(line.positionCount - 1, target.position);
    }
}
