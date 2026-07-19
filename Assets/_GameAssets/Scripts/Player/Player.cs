using Crafting;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Fusion;
using System;

public class Player : SimulationBehaviour, ITargetable, IHaveHealth
{
    public enum State
    {
        Default,
        Targeting,
        Dead
    }

    [SerializeField] private int health;
    [SerializeField] private PlayerHand hand;
    [SerializeField] private PlayerBoard board;
    [SerializeField] private PlayerTargeting targeter;
    [SerializeField] private Transform targetablePoint;
    [SerializeField] private PlayerHUD hud;

    public PlayerRef Ref { get; private set; }

    public State CurrState { get; private set; }

    public int CurrHealth { get; private set; }

    public int MaxHealth => health;

    public bool CanDrag => CurrState == State.Default; //TODO: prototype!

    public event Action<int> OnHealthChange;

    private void OnEnable()
    {
        SetState(State.Default);
        targeter.OnEnable();
    }

    private void OnDisable()
    {
        targeter.OnDisable();
    }

    private void LateUpdate()
    {
        hud.UpdateHUD();
    }

    public void DamageHealth(int damage)
    {
        SetHealth(CurrHealth - damage);
    }

    public void SetHealth(int health)
    {
        CurrHealth = health;
        OnHealthChange?.Invoke(CurrHealth);
        if (CurrHealth <= 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        if(board)
        {
            //cancel player's active card actions
            foreach(var item in board.ActiveItems)
            {
                item.CancelActions();
            }
        }
    }

    public async Task<List<ITargetable>> DoPlayerTargeting(CardAction.TargetingBehaviour targetingBehaviour, int numTargets)
    {
        SetState(State.Targeting);

        var targets = new List<ITargetable>(numTargets);
        if (targetingBehaviour == CardAction.TargetingBehaviour.PlayerChoosesTargets)
        {
            for (int i = 0; i < numTargets; i++)
            {
                targets.Add(await targeter.DoTargeting());
            }
        }
        else if(targetingBehaviour == CardAction.TargetingBehaviour.RandomTargets)
        {
            //TODO: Pick random targets from other players' sides
        }

        SetState(State.Default);
        return targets;
    }

    private void SetState(State state)
    {
        CurrState = state;
    }

    //ITargetable
    public Vector3 GetPositionAsTarget()
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

    //ITargetable
    public ITargetable.TargetableType GetTargetType()
    {
        return ITargetable.TargetableType.Player;
    }

    //ITargetable
    public void SetTargetingPreviewVisible(bool visible)
    {
        Debug.LogError($"TODO: Targeting preview VFX for players");
    }
}
