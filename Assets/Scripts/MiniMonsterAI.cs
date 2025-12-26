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
    public float attackCooldown = 1.5f;
    public LayerMask enemyLayer;

    [Header("참조")]
    public Transform attackPoint;

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private float lastAttackTime;

    void Start()
    {
        // 자식에 붙어있는 경우까지 안전하게 찾기
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 기본은 Idle
        if (anim) anim.SetBool("IsRunning", false);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 감지 범위 밖이면 아무것도 안 함(Idle 유지)
        if (distanceToPlayer > detectionRange) return;

        // 공격 사거리 안이면 공격(쿨타임)
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // 공격 애니메이션 트리거
                if (anim) anim.SetTrigger("Attack");

                // 데미지 판정
                DoDashAttack();

                // 쿨타임 갱신
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // 사거리 밖이면 추격 + Run
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );

            if (anim) anim.SetBool("IsRunning", true);
        }

        FlipSprite();
    }

    public void DoDashAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            // PlayerHealth가 콜라이더 자식에 붙어있을 수도 있어서 InParent 사용
            PlayerHealth player = hit.GetComponentInParent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(dashAttackDamage);
            }
        }
    }

    private void FlipSprite()
    {
        if (playerTransform == null || spriteRenderer == null) return;
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