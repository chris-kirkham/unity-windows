using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]   
public class PlayerTargeting : ICursorEventListener
{
    [SerializeField] private LayerMask targetableLayerMask;
    [SerializeField] private WorldSpaceCursorVisualiser worldSpaceCursor;
    [SerializeField] private PlayerTargetingVisualiserLine targetingLinePrefab;
    private List<PlayerTargetingVisualiserLine> activeTargetingLines;

    private Player player;
    private Cursor cursor;
    private ITargetable nextTarget;
    private bool waitingForNextTarget = false;

    private const float MaxTargetRaycastDist = 100f;
    private const bool SearchInChildren = true;
    private const bool SearchInParents = true;

    public void OnEnable(Player player, Cursor cursor)
    {
        this.player = player;
        this.cursor = cursor;
        ((ICursorEventListener)this).RegisterListener(cursor);
    }

    public void OnDisable()
    {
        ((ICursorEventListener)this).DeregisterListener(cursor);
    }

    public async Task<List<ITargetable>> DoPlayerTargeting(
        Transform targetingCard,
        ITargetable.TargetableType targetableTypeMask, 
        TargetedCardAction.TargetingBehaviour targetingBehaviour, 
        TargetedCardAction.TargetTeam targetTeam,
        int numTargets)
    {
        DestroyActiveTargetingLines();

        if (!cursor)
        {
            Debug.LogError($"No {nameof(Cursor)} set for this PlayerTargeter!");
            return null;
        }

        //initialise targeting viz lines
        activeTargetingLines = new List<PlayerTargetingVisualiserLine>(numTargets);
        for (int i = 0; i < numTargets; i++)
        {
            var line = GameObject.Instantiate(targetingLinePrefab, targetingCard.position, targetingCard.rotation);
            line.SetTargetTransform(worldSpaceCursor.transform); //lines follow world-space cursor initially
            activeTargetingLines.Add(line);
        }

        var targets = new List<ITargetable>(numTargets);
        if (targetingBehaviour == TargetedCardAction.TargetingBehaviour.PlayerChoosesTargets)
        {
            for (int i = 0; i < numTargets; i++)
            {
                ITargetable target = null;
                do
                {
                    target = await WaitForNextTarget();
                }
                while (!IsTargetValid(target, targetTeam, targetableTypeMask));
                
                targets.Add(target);
                if (target != null)
                {
                    activeTargetingLines[i].SetTargetTransform(target.GetTargetTransform());
                }
            }
        }
        else if (targetingBehaviour == TargetedCardAction.TargetingBehaviour.RandomTargets)
        {
            //TODO: Pick random targets from other players' sides
            throw new NotImplementedException();
        }

        DestroyActiveTargetingLines();
        return targets;

        bool IsTargetValid(ITargetable target, TargetedCardAction.TargetTeam targetTeam, ITargetable.TargetableType targetableTypeMask)
        {
            if(target == null)
            {
                return false;
            }

            //TODO: make this work for teams rather than just self/enemy
            if (targetTeam == TargetedCardAction.TargetTeam.Friendly && target.GetOwningPlayer() != player) 
            {
                return false;
            }

            //TODO: make this work for teams rather than just self/enemy
            if (targetTeam == TargetedCardAction.TargetTeam.Enemy && target.GetOwningPlayer() == player)
            {
                return false;
            }

            if(targetableTypeMask.HasFlag(ITargetable.TargetableType.Card) && target is CraftingItem)
            {
                return true;
            }

            if(targetableTypeMask.HasFlag(ITargetable.TargetableType.Player) && target is Player)
            {
                return true;
            }

            return false;
        }
    }

    public async Task<ITargetable> WaitForNextTarget() //routine which waits for next target selection from player input
    {
        nextTarget = null;
        waitingForNextTarget = true;
        while (waitingForNextTarget)
        {
            await Task.Yield();
        }

        return nextTarget;
    }

    public void CancelTargeting()
    {
        DestroyActiveTargetingLines();
    }

    private void DestroyActiveTargetingLines()
    {
        if (activeTargetingLines != null && activeTargetingLines.Count > 0)
        {
            foreach (var line in activeTargetingLines)
            {
                GameObject.Destroy(line.gameObject);
            }
        }
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
        if(!waitingForNextTarget)
        {
            return;
        }

        if (e == Cursor.EventID.MouseMove)
        {
            TryFetchTarget(out nextTarget);
        }
        else if (e == Cursor.EventID.LeftClickDown)
        {
            if(nextTarget != null) //found target, let targeting function return
            {
                waitingForNextTarget = false; 
            }
        }
        else if(e == Cursor.EventID.RightClickDown) //cancel targeting
        {
            nextTarget = null;
            waitingForNextTarget = false;
            CancelTargeting();
            //TODO: send targeting cancellation to player
            //TODO/FEATURE: allow cancelling only previous target? Or would that get tedious if player wants to cancel all
            //...this is just a minor QoL feature, don't prioritise it
        }
    }
}
