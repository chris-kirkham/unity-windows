using System;
using UnityEngine;

public interface IHaveHealth
{
    public void SetHealth(int newHealth);

    public void DamageHealth(int damage);

    public event Action<int> OnHealthChange; //parameter should be new health
}
