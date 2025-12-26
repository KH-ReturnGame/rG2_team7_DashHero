using UnityEngine;

public class EnemyVoidDeath : MonoBehaviour

{
    public float deathY = -10f;

    void Update()
    {
        if (transform.position.y <= deathY)
        {
            Destroy(gameObject);
        }
    }
}

