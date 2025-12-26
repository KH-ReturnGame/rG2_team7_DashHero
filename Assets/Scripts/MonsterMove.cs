using UnityEngine;

public class MonsterMove : MonoBehaviour
{   // 변수값 정해주기
    public float attackRange;   // 공격 사정거리(플레이어와의 거리)
    private Transform player;    // Player의 위치를 담을 변수
    private Animator anim;       // 애니메이션 제어용 변수
    public GameObject projectile; // 투사체 프리팹 (몬스터가 발사할 오브젝트)
    private SpriteRenderer spr;


    void Start()
    {
        // Player 태그 붙은 오브젝트 찾기
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        anim = GetComponent<Animator>();

        spr = GetComponent<SpriteRenderer>();
    }

    public void Attack()
    {
        // 도중에 플레이어가 사라졌다면 공격 로직 실행 안 함
        if (player == null) return;

        Vector2 playerPosition = player.transform.position; //플레이어의 위치계산


        GameObject clonedProjectile = Instantiate(projectile, transform.position, Quaternion.identity);//투사체 생성

        clonedProjectile.GetComponent<Fireball>().Initialize(playerPosition); //투사체 방향설정
        Debug.Log("뿅");
    }

    void Update()
    {
        // 도중에 플레이어가 Destroy 되면 player 변수는 null이 됨. 이를 체크해서 에러 방지.
        if (player == null)
        {
            anim.SetBool("isAttack", false);
            return;
        }

        // X축 거리만 체크 (좌우 차이)
        float distanceX = Mathf.Abs(transform.position.x - player.transform.position.x);

        if (distanceX <= attackRange)
        {
            // Debug.Log("공격가능 및 공격중");
            anim.SetBool("isAttack", true);   // 공격 애니메이션
        }
        else
        {
            //d Debug.Log("공격 불가능");
            anim.SetBool("isAttack", false);  // 대기 애니메이션
        }


        if (player.transform.position.x > transform.position.x)
        {
            spr.flipX = true;
        }
        else
            spr.flipX = false;
    }


}
