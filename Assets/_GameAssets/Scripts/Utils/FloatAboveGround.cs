using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class FloatAboveGround : NetworkBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float distAboveGround;

    private float maxRaycastDist = 100f;

    private void LateUpdate()
    {
        Float();
    }

    private void Float()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, maxRaycastDist, groundLayerMask))
        {
            transform.position = hit.point + (Vector3.up * distAboveGround);
        }
    }
}
