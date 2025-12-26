using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{   //�̵�����

    public float MoveSpeed = 6f;
    private Rigidbody2D rb;
    private float MoveInput;

    //��������

    public float jumpImpulse = 12f;//���� �ٴ� ��
    public Transform groundCheck; // player ������Ʈ�� �� ��ġ
    public float groundCheckRadius = 0.18f; // �ٴ� �����ϴ� ��(��Ʈ�ڽ� ����) Radius�� ������
    public LayerMask groundLayer; // ������ '�ٴ�'���� ����(���̾� ����)


    //�뽬 ���� (���߿����� ����)
    public float dashSpeed = 18f; // �뽬 �ӵ�
    public float dashTime = 0.15f; // �뽬 �����Ǵ� �ӵ� ª������ ������
    public float dashCooldown = 0.5f; //�뽬 ��Ÿ�� ���� �뽬 ����

    public int maxAirDashCount = 1; // ���߿��� �뽬 ������ ��

    private bool isDashing = false; // ���� �뽬���ΰ�(�뽬���̸� �ٸ� �ڵ� ���� �ȵ�)
    private bool canDash = true; // �뽬 ��Ÿ�� �������� Ȯ��
    private int airDashCount = 0; // ���߿��� �̹� ��� �뽬 ���°�

    private int facingDir = 1; //�÷��̾� �ٶ󺸴� ����

    public Camera cam;

    // [�߰�] ������ �뽬 �ڵ� ����(����)�� ������
    // =======================

    public Transform attackPoint;       // ���� ������(�÷��̾� ���� Empty ������Ʈ)
    public float attackRadius = 0.45f;  // ���� ����(�� ������)
    public LayerMask enemyLayer;        // �� ���̾�(Enemy ���̾�)
    public int dashAttackDamage = 1;    // ������ �뽬 ���� ������

    void LateUpdate()
    {
        // 카메라 경계 계산
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
    
        Vector3 camPos = cam.transform.position;
    
        // 플레이어 X 위치만 제한
        float clampedX = Mathf.Clamp(transform.position.x, 
            camPos.x - camWidth, 
            camPos.x + camWidth);
    
        // Y는 제한 없이 자유롭게
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        MoveInput = Input.GetAxisRaw("Horizontal"); //GetAxisRaw�� -1,0,1���� ���� �� ������

        // [�߰�] ������ ���� ����  MoveInput : ���� �����ϴ� ���� �̸�
        // MoveInput�� 0�� �ƴ� ���� ������ ������
        // (������ ���� ���� ���������� �ٶ� �������� �뽬�Ϸ���)

        if (MoveInput != 0)  
        {
            facingDir = (MoveInput > 0) ? 1 : -1;
        }

        // [�߰�] ���� ������ ���� �뽬 Ƚ�� ����
        // �����ߴٰ� �����ϸ� �ٽ� ���� �뽬 1ȸ�� "����"��
     
        if (IsGrounded())
        {
            airDashCount = 0;
        }
        // [�߰�] �뽬 �Է� (LeftShift)
        // ����:
        // 1) ��Ÿ���� ����(canDash)
        // 2) ���� �뽬 ���� �ƴ�(!isDashing)
        // 3) ���� �뽬 Ƚ�� ���� ����(airDashCount < maxAirDashCount)
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (canDash && !isDashing && airDashCount < maxAirDashCount)
            {
                StartCoroutine(Dash()); // �ڷ�ƾ���� "�뽬 �ð�/��Ÿ��" ���� [Dash �ڷ�ƾ] : "�ð��� �帣�� �ൿ"�� ó���ϴ� �Լ�
                                        // =====================================
                                        // �ڷ�ƾ�� ���� �Լ�ó�� �ٷ� ������ �ʰ�,
                                        // �߰��߰� "��� ����ٰ�(��ٷȴٰ�) �ٽ� �̾ ����"�� �� �־�.
            }
        }


        // ���� Ű�� "������ ��" �� ���� true (�⺻ Jump�� Space)
        // + �ٴڿ� ���� ���� ���� ���� 
        Debug.Log(IsGrounded());
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }


    }
    void FixedUpdate()

    {  // [�߰�] �뽬 �߿��� �̵� �ڵ带 �����ϸ� �� ��!
        // �����ϸ� rb.linearVelocity�� ��� ����Ἥ �뽬�� ����ų� ������
        if (isDashing) return;

        rb.linearVelocity = new Vector2(MoveInput * MoveSpeed, rb.linearVelocity.y);//�����ִ� �ӷ�

    }

    void Jump()
    {         // ���� ���Ʒ� �ӵ��� 0���� �ʱ�ȭ�� ��(���� ���� �� �� ���)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
               // �������� �������� ���� ���� �� ����!
        rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
        // ���� ũ�� ����, ������ ��� �ִ��� ��� // ForceMode2D.Impulse:�� �Ѽ����� ���� �ִ°�
    }
 
    IEnumerator Dash()
    { 
    // [�߰�] �뽬 �ڷ�ƾ �Լ�
    // �ڷ�ƾ(Coroutine) = "�ð��� �ɸ��� ����"�� ������� ó���ϴ� ����Ƽ ���
    // ��) �뽬:
    // 1) �뽬 ����
    // 2) 0.15�� ���� ����
    // 3) �뽬 ����
    // 4) 0.5�� ��Ÿ��
    //
    // �̷� �帧�� �Ϲ� �Լ�(void Dash())�� ����� "��ٸ���"�� �����ϱ� ��ƴ�.
    // �׷��� �ڷ�ƾ�� ����, �ڷ�ƾ�� ��ȯ���� IEnumerator�� �ؾ� �Ѵ�.
    //
    // �� IEnumerator�� '�Լ�'�� �ƴ϶� '��ȯ��(�ڷ���)'�̴�.
    // "�� �Լ��� �ڷ�ƾ ������� ����� �� �ִ�"��� ����Ƽ�� �˷��ִ� ����.
    //
    // �ڷ�ƾ�� StartCoroutine(Dash()); �� �����ؾ� �Ѵ�
    
        // ���� �뽬 ��� Ƚ�� 1ȸ ����
        // (���߿����� �����Ϸ��� �� ���� ���)
        airDashCount++;

        PlayerHealth ph = GetComponent<PlayerHealth>();
        if (ph != null) ph.isInvincible = true; // ���� ����

        // ��Ÿ�� ����: �뽬 ���
        canDash = false;

       
        // �뽬 ���� ���·� ����
        isDashing = true;

        // �뽬 ���� ����:
        // - �Է��� ������ �� �������� �뽬
        // - �Է��� ������ ���������� �ٶ� �������� �뽬
        int dashDir = facingDir;
        if (MoveInput != 0)
        {
            dashDir = (MoveInput > 0) ? 1 : -1;
        }

        // dashDir == 1  : ������ ���� �뽬 �� ���� O
        // dashDir == -1 : ���� ȸ�� �뽬 �� ���� X
        if (dashDir == 1)
        
           

    
        {
            GetComponent<PlayerAnim>()?.PlayDashAttackAnim();

            // �ִϸ��̼��� ȭ�鿡 ���� �ݿ��ǵ��� ��� ��ٷȴٰ� ������ ����
            yield return null; // 1������ ��� (���� ����)
                               // yield return new WaitForSeconds(0.02f); // �� �ڿ������� �Ϸ��� �̰ɷ�

            DoDashAttack();
        }

        // �뽬�� "Ȯ���ϰ�" ���̰� ������� �߷��� ��� ���� �� ����
        // (���� �뽬�� �Ʒ��� �������鼭 ������� ���� ����)
        float originalGravity = rb.gravityScale; // ���� �߷� ����
        rb.gravityScale = 0f;                    // �뽬 ���� �߷� 0

        // �뽬 ������ �ӵ��� ������ �ֱ�
        // y�� 0���� �ؼ� �������� �� ������(�繫���� '��' ����)
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);

        // dashTime ��ŭ �뽬 ����
        yield return new WaitForSeconds(dashTime);

        // �߷� ������� ����
        rb.gravityScale = originalGravity;

        // �뽬 ��
        isDashing = false;

        if (ph != null) ph.isInvincible = false; // ���� ����(�뽬 ��)

        // ��Ÿ�� ��ٸ���
        yield return new WaitForSeconds(dashCooldown);

        // ��Ÿ�� �� �� �ٽ� �뽬 ����
        canDash = true;
    }



    void DoDashAttack()
    {
        // attackPoint ��ġ �������� ���� �׷��� ���� ã��
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        // ã�� ���鿡�� ������ �ֱ�
        foreach (Collider2D hit in hits)
        {
            // ���� EnemyHealth ��ũ��Ʈ�� �پ��ִٴ� ����!
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(dashAttackDamage);
            }
        }
    }
    // �߹ٴ� ��ó�� ���� ���� �׷��� "Ground" ���̾�� ��ġ�� '�ٴ�'�̶�� �Ǵ�
    bool IsGrounded()
    {
    return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // 
    }
    // �����Ϳ��� �ٴ� üũ ���� ���̰�(������)
    void OnDrawGizmosSelected()
    
    {
        // �뽬 ���� ����(��)�� ���� ǥ��
if (attackPoint != null)
    Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); //DraWireSphere:�����ε� ��ü�� ���信 �׸���
    }

}

