using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int health;

    private int currHealth;

    public void DealDamage(int damage)
    {
        SetHealth(currHealth - damage);
    }

    private void SetHealth(int health)
    {
        throw new System.NotImplementedException();
    }
}
