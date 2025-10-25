/*using UnityEngine;

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
    public float dashCooldown = 2f;
    private bool isDashing = false;
    private float dashEndTime;
    private float lastDashTime;

    private bool isInvincible = false; // 대쉬 중 무적 상태
    private Rigidbody2D rb;





    public string stateName; // The name of the Animator state to check (e.g., "Idle", "Run")
    public int layerIndex = 0;




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















        if (anim != null && !string.IsNullOrEmpty(stateName))
        {
            // Check if the current state on the specified layer matches the stateName
            if (anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName))
            {
                Debug.Log(stateName + " is currently active in the Animator.");

                // You can also check if the animation is still in progress within that state
                // by checking normalizedTime, which goes from 0 at the start to 1 at the end of the state.
                if (anim.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < 1.0f)
                {
                    Debug.Log(stateName + " is still playing.");
                }
                else
                {
                    Debug.Log(stateName + " has finished playing.");
                }
            }
            else
            {
                Debug.Log(stateName + " is not the active state.");
            }
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
            Debug.Log("Is Dash Key Down + Dash CoolTime:");
            Debug.Log(Time.time >= lastDashTime + dashCooldown);
            StartDash();
        }
    }

    void StartDash()
    {
        Debug.Log("Dash 발동");
        isDashing = true;
        isInvincible = true;
        dashEndTime = Time.time + dashDuration;
        lastDashTime = Time.time;
        anim.SetTrigger("dash");
        Debug.Log("Dash 애니 발동");
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
    public float dashImpulse = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;
    private bool isDashing = false;
    private bool isInvincible = false;
    private float dashEndTime;
    private float lastDashTime;
    private float lastMoveDirection = 1f; // 마지막 이동 방향 (1 = 오른쪽, -1 = 왼쪽)

    private Rigidbody2D rb;
    private Animator anim;

    // 추가 멤버: 애니메이션 판정 안정화용
    [Header("Animation Stability")]
    [Tooltip("y속도의 작은 변화를 무시하기 위한 임계값")]
    public float yVelocityThreshold = 0.1f; // 필요에 따라 0.05~0.2 사이로 조절
    private bool wasInAir = false;          // 실제로 공중에 들어간 적이 있는지
    private bool prevIsGrounded = true;     // 이전 프레임의 grounded 상태

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
        // 이전 grounded 상태 저장
        prevIsGrounded = isGrounded;

        // 현재 grounded 갱신
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 만약 이전엔 땅에 있었고 지금은 땅에 없다면 '공중 진입'으로 기록
        if (prevIsGrounded && !isGrounded)
        {
            wasInAir = true;
        }

        // 땅에 닿으면 공중 상태 초기화
        if (isGrounded)
        {
            wasInAir = false;
        }

        // 점프 처리 (독립)
        HandleJump();

        // 이동/대쉬 처리 (대쉬 중엔 입력 무시)
        if (!isDashing)
        {
            HandleMovement();
            HandleDash();
        }
        else if (Time.time >= dashEndTime)
        {
            EndDash();
        }

        // --- 애니메이터 업데이트 (안전하게 null 체크 포함) ---
        if (anim != null)
        {
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("yVelocity", rb.velocity.y);

            // 안정화된 점프/낙하 판정:
            // - 실제로 공중에 '진입한 적이 있어야' 점프/낙하 애니 사용 (wasInAir)
            // - y 속도는 데드존을 적용
            float y = rb.velocity.y;

            bool isActuallyJumping = wasInAir && (y > yVelocityThreshold);
            bool isActuallyFalling = wasInAir && (y < -yVelocityThreshold);

            anim.SetBool("isJumping", isActuallyJumping);
            anim.SetBool("isFalling", isActuallyFalling);

            // 이동 파라미터 (절대값)
            anim.SetFloat("moveX", Mathf.Abs(rb.velocity.x) / Mathf.Max(0.0001f, moveSpeed));
        }
    }

    void HandleMovement()
    {
        float moveX = 0f;
        if (Input.GetKey(KeyCode.A)) { moveX = -1f; lastMoveDirection = -1f; }
        if (Input.GetKey(KeyCode.D)) { moveX = 1f; lastMoveDirection = 1f; }

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
        // 애니 파라미터는 Update에서 처리 (anim null 체크 포함)
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            wasInAir = true; // 점프 시작이므로 공중 진입으로 표시
            if (anim != null) anim.SetTrigger("jump");
        }
    }

    void HandleDash()
    {
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

        if (anim != null) anim.SetTrigger("dash");
        // Y 속도를 보존하면서 X 방향으로 Impulse 성격의 순간 속도 적용
        // 기존에 AddForce(Impulse)를 사용하면 y를 바꾸지 않으려면 velocity 조작이 편리
        rb.velocity = new Vector2(lastMoveDirection * dashImpulse, rb.velocity.y);
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

    public bool IsInvincible() => isInvincible;
}*/

