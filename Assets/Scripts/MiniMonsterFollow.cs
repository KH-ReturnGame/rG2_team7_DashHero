using UnityEngine;

public class MiniMonsterAI : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 2f;
    public float detectionRange = 40f;

    [Header("공격 설정")]
    public float attackRange = 2f;
    public float attackRadius = 3f;
    public int dashAttackDamage = 1;
    public float attackCooldown = 1.5f; // 공격 간격
    public LayerMask enemyLayer;

    [Header("참조")]
    public Transform attackPoint;

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private float lastAttackTime; // 마지막 공격 시점을 저장

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        // 1. 플레이어 생존 확인 (Null 체크)
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 2. 감지 범위 체크
        if (distanceToPlayer <= detectionRange)
        {
            // 3. 공격 사거리 안인지 확인
            if (distanceToPlayer <= attackRange)
            {
                // [핵심] Update 안에서 호출 자체에 쿨타임을 적용
                // 현재 시간이 마지막 공격 시간 + 쿨타임보다 클 때만 실행
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    DoDashAttack();
                    lastAttackTime = Time.time; // 공격 직후 시간 업데이트
                }
            }
            else
            {
                // 공격 사거리 밖이면 추격
                transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            }

            FlipSprite();
        }
    }

    // 이제 이 함수는 호출될 때마다 무조건 공격을 수행합니다. (쿨타임 체크는 위에서 함)
    public void DoDashAttack()
    {
        Debug.Log("DoDashAttack 함수가 호출됨!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(dashAttackDamage);
            }
        }
    }

    private void FlipSprite()
    {
        if (playerTransform == null) return;
        spriteRenderer.flipX = (playerTransform.position.x < transform.position.x);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}