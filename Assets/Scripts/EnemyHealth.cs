using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 1;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} 맞음! 남은 HP: {hp}");

        if (hp <= 0)
            Destroy(gameObject);
    }
}
