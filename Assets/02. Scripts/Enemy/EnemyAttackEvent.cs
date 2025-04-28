using UnityEngine;

public class EnemyAttackEvent : MonoBehaviour
{
    public Enemy enemy;

    public void AttackEvent()
    {
        enemy.MakeDamage();
    }
}
