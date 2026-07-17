using Crafting;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using StateMachine;

public class Player : MonoBehaviour, ITargetable, IDamageable
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
    [SerializeField, FormerlySerializedAs("cardActionTargetPoint")] private Transform targetablePoint;

    public State CurrState { get; private set; }

    public int CurrHealth { get; private set; }

    public int MaxHealth => health;

    public bool CanDrag => CurrState == State.Default; //TODO: prototype!

    private void OnEnable()
    {
        SetState(State.Default);
        targeter.OnEnable();
    }

    private void OnDisable()
    {
        targeter.OnDisable();
    }

    public void Damage(int damage)
    {
        SetHealth(CurrHealth - damage);
    }

    private void SetHealth(int health)
    {
        CurrHealth = health;
        if(CurrHealth <= 0)
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

    public async Task<ITargetable> DoPlayerTargeting()
    {
        SetState(State.Targeting);
        var target = await targeter.DoTargeting();
        SetState(State.Default);
        return target;
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
