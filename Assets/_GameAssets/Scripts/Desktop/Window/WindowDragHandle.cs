using UnityEngine;

public class WindowDragHandle : MonoBehaviour
{
    [SerializeField] private RectTransform handleRect;

    private void OnDrawGizmos()
    {
        if(!handleRect)
        {
            return;
        }

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
        var handleRectWorldCorners = new Vector3[4];
        handleRect.GetWorldCorners(handleRectWorldCorners);
        Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(handleRectWorldCorners), looped: true);
        var size = handleRectWorldCorners[2] - handleRectWorldCorners[0];

        Gizmos.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.1f);
        Gizmos.DrawCube(handleRectWorldCorners[0] + size / 2, size);
    }
}
