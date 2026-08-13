using Crafting;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Fusion;
using System;

public class Player : SimulationBehaviour, ITargetable, IHaveHealth, ICursorEventListener
{
    public enum State
    {
        Default,
        Targeting,
        Inspecting,
        Dead
    }

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Cursor cursor;
    [SerializeField] private PlayerHand hand;
    [SerializeField] private PlayerBoard board;
    [SerializeField] private Transform targetablePoint;
    [SerializeField] private PlayerHUD hud;
    [Header("Player states")] //TODO: implement a proper state machine
    [SerializeField] private PlayerTargeting targeting;
    [SerializeField] private ItemInspectSequence inspect;

    public PlayerRef Ref { get; private set; }

    public State CurrState { get; private set; }

    public int CurrHealth { get; private set; }

    public int MaxHealth => maxHealth;

    public bool CanDrag => CurrState == State.Default; //TODO: prototype!

    public event Action<int> OnHealthChange;

    private void OnEnable()
    {
        SetState(State.Default);
        targeting.OnEnable(this, cursor);
        inspect.OnEnable(cursor);
        ((ICursorEventListener)this).RegisterListener(cursor);
    }

    private void OnDisable()
    {
        targeting.OnDisable();
        inspect.OnDisable();
        ((ICursorEventListener)this).DeregisterListener(cursor);
    }

    private void LateUpdate()
    {
        hud.UpdateHUD();
    }

    public async Task<List<ITargetable>> DoPlayerTargeting(
        Transform targetingCard,
        ITargetable.TargetableType targetableTypeMask, 
        TargetedCardAction.TargetingBehaviour targetingBehaviour, 
        TargetedCardAction.TargetTeam targetTeam,
        int numTargets)
    {
        SetState(State.Targeting);
        var targets = await targeting.DoPlayerTargeting(targetingCard, targetableTypeMask, targetingBehaviour, targetTeam, numTargets);
        SetState(State.Default);
        return targets;
    }

    public void CancelPlayerTargeting()
    {
        Debug.LogError($"TODO: implement targeting cancel behaviour");
    }

    private async void InspectItem(CraftingItem item)
    {
        SetState(State.Inspecting);
        await inspect.InspectItem(item);
        SetState(State.Default);
    }

    private void CancelInspectItem()
    {
        inspect.Cancel();
        SetState(State.Default);
    }

    private void SetState(State state)
    {
        CurrState = state;
    }

#region IHaveHealth
    public void SetHealth(int health)
    {
        CurrHealth = health;
        OnHealthChange?.Invoke(CurrHealth);
        if (CurrHealth <= 0)
        {
            OnKilled();
        }
    }

    public void AddHealth(int healthToAdd)
    {
        SetHealth(Mathf.Min(CurrHealth + healthToAdd, MaxHealth));
    }

    public void DamageHealth(int damage)
    {
        SetHealth(CurrHealth - damage);
    }
#endregion

    private void OnKilled()
    {
        if (board)
        {
            //cancel player's active card actions
            foreach (var item in board.ActiveItems)
            {
                item.CancelActions();
            }
        }
    }

    #region ITargetable
    public Vector3 GetTargetPosition()
    {
        if(targetablePoint)
        {
            return targetablePoint.position;
        }
        else
        {
            Debug.LogWarning($"Player's target point not set! Returning transform.position");
            return transform.position;
        }
    }

    public Transform GetTargetTransform()
    {
        if(targetablePoint)
        {
            return targetablePoint.transform;
        }
        else
        {
            Debug.LogWarning($"Player's target point not set! Returning transform");
            return transform;
        }
    }

    public ITargetable.TargetableType GetTargetableType()
    {
        return ITargetable.TargetableType.Player;
    }

    public void SetTargetingPreviewVisible(bool visible)
    {
        Debug.LogError($"TODO: Targeting preview VFX for players");
    }

    public Player GetOwningPlayer()
    {
        return this;
    }
#endregion

    public void OnCursorEvent(Cursor.EventID e)
    {
        if(CurrState == State.Default && e == Cursor.EventID.RightClickDown)
        {
            //try to inspect currently picked-up item, if any
            var item = cursor.CurrentDragTarget as CraftingItem;
            if(!item) //try to find item under cursor
            {
                cursor.TryGetHoveredListenerOfType<CraftingItem>(out item);
            }

            if(item)
            {
                InspectItem(item);
            }
        }
        else if(CurrState == State.Inspecting 
            && (e == Cursor.EventID.MouseWheelDown || e == Cursor.EventID.RightClickUp))
        {
            CancelInspectItem();
        }
    }
}
