using Unity.VisualScripting;
using UnityEngine;

public class DraggablePhysicsObject : DraggableObject
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider coll;
    [SerializeField] private float targetDistanceAboveGround = 10f;
    [SerializeField] private LayerMask groundRaycastMask;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float lerpBackFromPosOverrideTime = 0.5f;

    private const float MinDistFromCamera = 1f;
    private const float MaxRaycastDist = 100f;

    private bool lerpBackFromPositionOverride = false;
    private float lerpBackFromPosOverrideStartTime;
    private bool hadPosOverrideLastTick = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!rb)
        {
            Debug.LogError($"No Rigidbody set for this {nameof(DraggablePhysicsObject)}!");
        }

        if (!coll)
        {
            Debug.LogError($"No Collider set for this {nameof(DraggablePhysicsObject)}");
        }
    }

    protected override void OnStartDrag()
    {
        base.OnStartDrag();
        
        if(rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if(coll)
        {
            coll.enabled = false;
        }
    }

    protected override void OnEndDrag()
    {
        base.OnEndDrag();
    
        if(!IsPlaced) //let object fall/move with physics if not placed at a point
        {
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.linearVelocity = Vector3.zero;
            }

            if (coll)
            {
                coll.enabled = true;
            }
        }
    }

    public override void FixedUpdateNetwork()
    //public void LateUpdate()
    {
        if(rb && isDragging)
        {
            var targetPos = GetDragPosition();
            if(dragPositionOverride.HasValue)
            {
                hadPosOverrideLastTick = true;
            }
            else //position override lerp stuff... a lot of faff just to make things look smoother...
            {
                if(hadPosOverrideLastTick)
                {
                    lerpBackFromPositionOverride = true;
                    lerpBackFromPosOverrideStartTime = Time.time;
                    hadPosOverrideLastTick = false;
                }

                if(Time.time - lerpBackFromPosOverrideStartTime < lerpBackFromPosOverrideTime)
                {
                    var t = (Time.time - lerpBackFromPosOverrideStartTime) / lerpBackFromPosOverrideTime;
                    transform.position = Vector3.Lerp(transform.position, targetPos, t);
                    if(t >= 1f)
                    {
                        lerpBackFromPositionOverride = false;
                    }
                }
                else
                {
                    transform.position = targetPos;
                }
            }

            //transform.position = targetPos;
            //rb.MovePosition(targetPos);
        }
    }

    public override Vector3 GetDragPosition()
    {
        if(dragPositionOverride.HasValue)
        {
            return dragPositionOverride.Value;
        }

        var cam = cursor.Cam;
        var cursorPos = cursor.ClampedPosition_SS;

        //get distance above ground/other objects
        var distFromCamera = targetDistanceAboveGround;
        if (Physics.Raycast(
            cam.ScreenPointToRay(cursorPos), out var hit, MaxRaycastDist, groundRaycastMask, QueryTriggerInteraction.Ignore))
        {
            var hitPointWithHeightOffset = hit.point + (Vector3.up * targetDistanceAboveGround);
            distFromCamera = Mathf.Max(MinDistFromCamera, Vector3.Distance(cam.transform.position, hitPointWithHeightOffset));
            var targetPos_WS = hit.point + ((cam.transform.position - hit.point).normalized * targetDistanceAboveGround);

            return targetPos_WS;
        }

        Debug.LogError($"No valid drag position found! Returning (0,0,0)");
        return Vector3.zero;
    }
}
