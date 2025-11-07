using UnityEngine;

public class playerMonoBehavior : MonoBehaviour
{   //이동관련

    public float MoveSpeed = 6f;
    private Rigidbody2D rb;
    private float MoveInput;

    //점프관련

    public float jumpImpulse = 12f;//점프 뛰는 힘
    public Transform groundCheck; // player 오브젝트의 발 위치
    public float groundCheckRadius = 0.18f; // 바닥 감지하는 원(히트박스 개념) Radius는 반지름
    public LayerMask groundLayer; // 무엇을 '바닥'으로 볼지(레이어 선택)





    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        MoveInput = Input.GetAxisRaw("Horizontal"); //GetAxisRaw는 -1,0,1으로 축의 값 정해줌

        // 점프 키를 "눌렀을 때" 한 번만 true (기본 Jump는 Space)
        // + 바닥에 있을 때만 점프 실행 
        Debug.Log(IsGrounded());
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }


    }
    void FixedUpdate()
    {
        
        rb.linearVelocity = new Vector2(MoveInput * MoveSpeed, rb.linearVelocity.y);//방향있는 속력

    }

    void Jump()
    {         // 현재 위아래 속도를 0으로 초기화한 뒤(연속 점프 시 더 깔끔)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
               // 위쪽으로 순간적인 힘을 더함 → 점프!
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        // 방향 크기 정보, 그힘을 어떻게 주는지 모드 // ForceMode2D.Impulse:딱 한순간에 힘을 주는거
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
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); //DraWireSphere:선으로된 구체를 씬뷰에 그린것
    }

}

