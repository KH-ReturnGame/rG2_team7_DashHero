using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{   //이동관련

    public float MoveSpeed = 6f;
    private Rigidbody2D rb;
    private float MoveInput;

    //점프관련

    public float jumpImpulse = 12f;//점프 뛰는 힘
    public Transform groundCheck; // player 오브젝트의 발 위치
    public float groundCheckRadius = 0.18f; // 바닥 감지하는 원(히트박스 개념) Radius는 반지름
    public LayerMask groundLayer; // 무엇을 '바닥'으로 볼지(레이어 선택)


    //대쉬 관련 (공중에서도 가능)
    public float dashSpeed = 18f; // 대쉬 속도
    public float dashTime = 0.15f; // 대쉬 유지되는 속도 짧을수록 슉느낌
    public float dashCooldown = 0.5f; //대쉬 쿨타임 연속 대쉬 방지

    public int maxAirDashCount = 1; // 공중에서 대쉬 가능한 수

    private bool isDashing = false; // 지금 대쉬중인가(대쉬중이면 다른 코드 실행 안됨)
    private bool canDash = true; // 대쉬 쿨타임 끝났는지 확인
    private int airDashCount = 0; // 공중에서 이미 몇번 대쉬 썻는가

    private int facingDir = 1; //플레이어 바라보는 방향

    // [추가] 오른쪽 대쉬 자동 공격(베기)용 변수들
    // =======================

    public Transform attackPoint;       // 공격 기준점(플레이어 앞쪽 Empty 오브젝트)
    public float attackRadius = 0.45f;  // 공격 범위(원 반지름)
    public LayerMask enemyLayer;        // 적 레이어(Enemy 레이어)
    public int dashAttackDamage = 1;    // 오른쪽 대쉬 공격 데미지



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        MoveInput = Input.GetAxisRaw("Horizontal"); //GetAxisRaw는 -1,0,1으로 축의 값 정해줌

        // [추가] 마지막 방향 저장  MoveInput : 값을 저장하는 변수 이름
        // MoveInput이 0이 아닐 때만 방향을 갱신함
        // (가만히 있을 때도 마지막으로 바라본 방향으로 대쉬하려고)

        if (MoveInput != 0)  
        {
            facingDir = (MoveInput > 0) ? 1 : -1;
        }

        // [추가] 땅에 닿으면 공중 대쉬 횟수 리셋
        // 점프했다가 착지하면 다시 공중 대쉬 1회가 "충전"됨
     
        if (IsGrounded())
        {
            airDashCount = 0;
        }
        // [추가] 대쉬 입력 (LeftShift)
        // 조건:
        // 1) 쿨타임이 끝남(canDash)
        // 2) 지금 대쉬 중이 아님(!isDashing)
        // 3) 공중 대쉬 횟수 남아 있음(airDashCount < maxAirDashCount)
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (canDash && !isDashing && airDashCount < maxAirDashCount)
            {
                StartCoroutine(Dash()); // 코루틴으로 "대쉬 시간/쿨타임" 관리 [Dash 코루틴] : "시간이 흐르는 행동"을 처리하는 함수
                                        // =====================================
                                        // 코루틴은 보통 함수처럼 바로 끝나지 않고,
                                        // 중간중간 "잠깐 멈췄다가(기다렸다가) 다시 이어서 실행"할 수 있어.
            }
        }


        // 점프 키를 "눌렀을 때" 한 번만 true (기본 Jump는 Space)
        // + 바닥에 있을 때만 점프 실행 
        Debug.Log(IsGrounded());
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }


    }
    void FixedUpdate()

    {  // [추가] 대쉬 중에는 이동 코드를 실행하면 안 됨!
        // 실행하면 rb.linearVelocity를 계속 덮어써서 대쉬가 끊기거나 약해짐
        if (isDashing) return;

        rb.linearVelocity = new Vector2(MoveInput * MoveSpeed, rb.linearVelocity.y);//방향있는 속력

    }

    void Jump()
    {         // 현재 위아래 속도를 0으로 초기화한 뒤(연속 점프 시 더 깔끔)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
               // 위쪽으로 순간적인 힘을 더함 → 점프!
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        // 방향 크기 정보, 그힘을 어떻게 주는지 모드 // ForceMode2D.Impulse:딱 한순간에 힘을 주는거
    }
 
    IEnumerator Dash()
    { 
    // [추가] 대쉬 코루틴 함수
    // 코루틴(Coroutine) = "시간이 걸리는 동작"을 순서대로 처리하는 유니티 기능
    // 예) 대쉬:
    // 1) 대쉬 시작
    // 2) 0.15초 동안 유지
    // 3) 대쉬 종료
    // 4) 0.5초 쿨타임
    //
    // 이런 흐름은 일반 함수(void Dash())로 만들면 "기다리기"를 구현하기 어렵다.
    // 그래서 코루틴을 쓰고, 코루틴은 반환형을 IEnumerator로 해야 한다.
    //
    // ★ IEnumerator는 '함수'가 아니라 '반환형(자료형)'이다.
    // "이 함수는 코루틴 방식으로 실행될 수 있다"라고 유니티에 알려주는 역할.
    //
    // 코루틴은 StartCoroutine(Dash()); 로 실행해야 한다
    
        // 공중 대쉬 사용 횟수 1회 증가
        // (공중에서만 제한하려고 이 값을 사용)
        airDashCount++;

        // 쿨타임 시작: 대쉬 잠금
        canDash = false;

        // 대쉬 시작 상태로 변경
        isDashing = true;

        // 대쉬 방향 결정:
        // - 입력이 있으면 그 방향으로 대쉬
        // - 입력이 없으면 마지막으로 바라본 방향으로 대쉬
        int dashDir = facingDir;
        if (MoveInput != 0)
        {
            dashDir = (MoveInput > 0) ? 1 : -1;
        }

        // dashDir == 1  : 오른쪽 돌진 대쉬 → 공격 O
        // dashDir == -1 : 왼쪽 회피 대쉬 → 공격 X
        if (dashDir == 1)
        {
            DoDashAttack();
        }

        if (dashDir == 1)
        {
            GetComponent<PlayerAnim>()?.PlayDashAttackAnim();

            // 애니메이션이 화면에 먼저 반영되도록 잠깐 기다렸다가 데미지 적용
            yield return null; // 1프레임 대기 (가장 간단)
                               // yield return new WaitForSeconds(0.02f); // 더 자연스럽게 하려면 이걸로

            DoDashAttack();
        }

        // 대쉬를 "확실하게" 보이게 만들려면 중력을 잠깐 끄는 게 좋음
        // (공중 대쉬가 아래로 떨어지면서 흐려지는 느낌 방지)
        float originalGravity = rb.gravityScale; // 원래 중력 저장
        rb.gravityScale = 0f;                    // 대쉬 동안 중력 0

        // 대쉬 순간에 속도를 강제로 주기
        // y는 0으로 해서 수평으로 쭉 나가게(사무라이 '슉' 느낌)
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);

        // dashTime 만큼 대쉬 유지
        yield return new WaitForSeconds(dashTime);

        // 중력 원래대로 복구
        rb.gravityScale = originalGravity;

        // 대쉬 끝
        isDashing = false;

        // 쿨타임 기다리기
        yield return new WaitForSeconds(dashCooldown);

        // 쿨타임 끝 → 다시 대쉬 가능
        canDash = true;
    }



    void DoDashAttack()
    {
        // attackPoint 위치 기준으로 원을 그려서 적을 찾음
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        // 찾은 적들에게 데미지 주기
        foreach (Collider2D hit in hits)
        {
            // 적에 EnemyHealth 스크립트가 붙어있다는 가정!
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(dashAttackDamage);
            }
        }
    }
    // 발바닥 근처에 작은 원을 그려서 "Ground" 레이어와 겹치면 '바닥'이라고 판단
    bool IsGrounded()
    {
    return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // 
    }
    // 에디터에서 바닥 체크 원이 보이게(디버깅용)
    void OnDrawGizmosSelected()
    
    {
        // 대쉬 공격 범위(원)도 씬에 표시
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); //DraWireSphere:선으로된 구체를 씬뷰에 그린것
    }

}

