using System.Threading.Tasks;
using UnityEngine;
using StateMachine;

[System.Serializable]   
public class PlayerTargeting : ICursorEventListener
{
    [SerializeField] private Cursor cursor;
    [SerializeField] private GameObject targetingVFXPrefab;
    [SerializeField] private LayerMask targetableLayerMask;

    private ITargetable currentTarget;
    private bool targetingActive = false;

    private const float MaxTargetRaycastDist = 100f;
    private const bool SearchInChildren = true;
    private const bool SearchInParents = true;

    public void OnEnable()
    {
        ((ICursorEventListener)this).RegisterListener(cursor);
    }

    public void OnDisable()
    {
        ((ICursorEventListener)this).DeregisterListener(cursor);
    }

    public async Task<ITargetable> DoTargeting()
    {
        if (!cursor)
        {
            Debug.LogError($"No {nameof(Cursor)} set for this PlayerTargeter!");
            return null;
        }

        currentTarget = null;
        targetingActive = true;
        while(targetingActive)
        {
            await Task.Yield();
        }

        return currentTarget;
    }

    private bool TryFetchTarget(out ITargetable target)
    {
        target = null;

        if (CamUtils.RaycastFromCamera(cursor.Cam, cursor.ClampedPosition_SS, out var hit, MaxTargetRaycastDist, targetableLayerMask))
        {
            var hitObj = hit.collider.gameObject;

            target = hitObj.GetComponent<ITargetable>();

            if (target == null && SearchInChildren)
            {
                target = hitObj.GetComponentInChildren<ITargetable>();
            }

            if (target == null && SearchInParents)
            {
                target = hitObj.GetComponentInParent<ITargetable>();
            }

            if (target != null)
            {
                Debug.Log($"Target: {((MonoBehaviour)target).name}");
                return true;
            }
        }

        return false;
    }

    public void OnCursorEvent(Cursor.EventID e)
    {
        if(!targetingActive)
        {
            return;
        }

        if(e == Cursor.EventID.LeftClickDown)
        {
            if(currentTarget != null) //found target, let targeting function return
            {
                targetingActive = false; 
            }
        }
        else if(e == Cursor.EventID.MouseMove)
        {
            TryFetchTarget(out currentTarget);
        }
        else if(e == Cursor.EventID.RightClickDown) //cancel targeting
        {
            currentTarget = null;
            targetingActive = false;
        }
    }
}
