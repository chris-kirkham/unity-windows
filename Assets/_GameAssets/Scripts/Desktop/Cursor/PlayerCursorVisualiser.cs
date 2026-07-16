using Fusion;
using UnityEngine;

//world-space visualiser for other players' cursor movements
[RequireComponent(typeof(NetworkTransform))]
public class PlayerCursorVisualiser : NetworkBehaviour
{
    [SerializeField] private bool visibleForLocalPlayer; //TODO!
    [SerializeField] private Cursor cursor;
    [SerializeField] private LayerMask groundRaycastMask;
    [SerializeField] private float targetDistanceAboveGround;

    private const float MinDistFromCamera = 1f;

    private void OnEnable()
    {
        if(HasStateAuthority && !visibleForLocalPlayer)
        {
            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if(!cursor)
        {
            return;
        }

        if(CamUtils.GetWorldSpaceFollowAboveGroundPos(
            cursor.Cam,
            cursor.ClampedPosition_SS,
            targetDistanceAboveGround, 
            groundRaycastMask, 
            MinDistFromCamera,
            out var pos_WS))
        {
            transform.position = pos_WS;
        }
    }
}
