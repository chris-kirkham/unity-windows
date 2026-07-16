using Crafting;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ICardActionTarget, IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private PlayerHand hand;
    [SerializeField] private PlayerBoard board;
    [SerializeField] private Transform cardActionTargetPoint;

    private int currHealth;

    public void Damage(int damage)
    {
        SetHealth(currHealth - damage);
    }

    private void SetHealth(int health)
    {
        currHealth = health;
        if(currHealth <= 0)
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

    //ICardActionTarget
    public Vector3 GetTargetPosition()
    {
        if(cardActionTargetPoint)
        {
            return cardActionTargetPoint.position;
        }
        else
        {
            Debug.LogWarning($"Player's target point not set! Returning transform.position");
            return transform.position;
        }
    }
}
