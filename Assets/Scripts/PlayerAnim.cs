using UnityEngine;

public class PlayerAnim : MonoBehaviour


{
    public Animator anim;

    private Rigidbody2D rb;


    private PlayerMove move;

    // 오른쪽 대쉬 공격 애니메이션 재생(Trigger)
    public void PlayDashAttackAnim()
    {
        anim.SetTrigger("DashAttack");
    }
  

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        move = GetComponent<PlayerMove>();

        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        bool running = Mathf.Abs(x) > 0.01f;

        anim.SetBool("IsRunning", running);


   
        bool grounded = true; 
        if (move != null && move.groundCheck != null)
        {
            grounded = Physics2D.OverlapCircle(
                move.groundCheck.position,
                move.groundCheckRadius,
                move.groundLayer
            );
        }
        anim.SetBool("IsGrounded", grounded);

       
        bool falling = (rb != null) && (rb.linearVelocity.y < -0.1f);
        anim.SetBool("IsFalling", falling);
    }


}