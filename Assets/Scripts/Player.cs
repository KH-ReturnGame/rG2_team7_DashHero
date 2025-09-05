using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SannabiStylePlayer : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 5f;

    [Header("점프")]
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("대쉬")]
    public float dashImpulse = 80f;      // 순간 힘 세기
    public float dashDuration = 0.2f;    // 무적 지속 시간
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private float dashEndTime;
    private float lastDashTime;

    private bool isInvincible = false; // 대쉬 중 무적 상태
    private Rigidbody2D rb;

    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;

        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = Vector3.down * 0.5f;
            groundCheck = gc.transform;
        }
    }

    void Update()
    {
        // --- 바닥 체크 ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- 점프는 좌우 이동과 독립적으로 처리 ---
        HandleJump();

        if (!isDashing)
        {
            HandleMovement();
            HandleDash();
        }
        else
        {
            if (Time.time >= dashEndTime)
                EndDash();
        }
    }

    void HandleMovement()
    {
        // 좌우 이동만 담당
        float moveX = 0f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void HandleDash()
    {
        // J 키 눌렀을 때 x+ 방향으로 Impulse
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= lastDashTime + dashCooldown)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        isInvincible = true;
        dashEndTime = Time.time + dashDuration;
        lastDashTime = Time.time;
        anim.SetTrigger("dash");
        // 순간 힘 추가 (Impulse)
        rb.AddForce(Vector2.right * dashImpulse, ForceMode2D.Impulse);
    }

    void EndDash()
    {
        isDashing = false;
        isInvincible = false;
    }


    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}
