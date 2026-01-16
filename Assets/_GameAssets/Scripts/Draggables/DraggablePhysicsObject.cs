using UnityEngine;

public class DraggablePhysicsObject : DraggableElement
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float targetDistanceAboveGround = 10f;
    [SerializeField] private LayerMask groundRaycastMask;
    [SerializeField] private float moveSpeed = 1f;

    private const float MinDistFromCamera = 1f;
    private const float MaxRaycastDist = 100f;
    private const int MaxRaycastHits = 20;
    private RaycastHit[] raycastHits = new RaycastHit[MaxRaycastHits];

    protected override void Start()
    {
        base.Start();

        if (!rb)
        {
            Debug.LogError($"No Rigidbody set for this {nameof(DraggablePhysicsObject)}!");
        }

        if(!coll)
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
        }

        if(coll)
        {
            coll.enabled = false;
        }
    }

    protected override void OnEndDrag()
    {
        base.OnEndDrag();

        if(rb)
        {
            rb.isKinematic = false;
        }

        if(coll)
        {
            coll.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if(rb && isDragging)
        {
            var cam = Cursor.Inst.AssociatedCamera;
            var cursorPos = Cursor.Inst.ClampedPosition_SS;

            //get distance above ground/other objects
            var distFromCamera = targetDistanceAboveGround;
            //var numHits = Physics.RaycastNonAlloc(cam.ScreenPointToRay(cursorPos), raycastHits, MaxRaycastDist, groundRaycastMask);
            if(Physics.Raycast(
                cam.ScreenPointToRay(cursorPos),
                out var hit,
                MaxRaycastDist,
                groundRaycastMask,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore))
            {
                distFromCamera = Mathf.Clamp(hit.distance - targetDistanceAboveGround, MinDistFromCamera, hit.distance);  
            }
            
            var targetPos_WS = cam.ScreenToWorldPoint(new Vector3(cursorPos.x, cursorPos.y, distFromCamera));
            //var targetPos_WS = (Vector3)Cursor.Inst.ClampedPosition_WS + (cam.transform.forward * distFromCursor);
            //rb.AddForce((targetPos_WS - rb.position) * moveSpeed, ForceMode.Acceleration); //TODO: use a PID controller or smth
            rb.MovePosition(targetPos_WS);
        }
    }
}
