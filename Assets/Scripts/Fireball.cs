using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f; // 파이어볼의 속도
    private Rigidbody2D rb;  // Rigidbody2D 컴포넌트
    public int damage = 1;

    // 파이어볼이 플레이어 반대 방향으로 날아가게 하는 함수
    public void Initialize(Vector2 playerPosition)
    {
        // Rigidbody2D 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();

        // 현재 파이어볼 위치와 플레이어 위치를 이용해 방향 계산 (플레이어 방향)
        Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;

        // 방향에 맞게 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


        // 반대 방향으로 힘을 가함
        rb.AddForce(direction * speed, ForceMode2D.Impulse); // Impulse로 힘을 즉시 가함
    }

    // ★ 핵심: 플레이어와 닿으면 데미지
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
        // 벽에 닿아도 사라지게 하고 싶으면 아래처럼 레이어/태그로 처리 가능
        // if (other.CompareTag("Ground")) Destroy(gameObject);
    }


}
