
using UnityEngine;

public class MonsterMove : MonoBehaviour
{   // 변수값 정해주기
    public float attackRange;   // 공격 사정거리(플레이어와의 거리)
    private Transform player;    // Player의 위치를 담을 변수
    private Animator anim;       // 애니메이션 제어용 변수
    public GameObject projectile; // 투사체 프리팹 (몬스터가 발사할 오브젝트)


    void Start()
    {
        // Player 태그 붙은 오브젝트 찾기
        player = GameObject.FindWithTag("Player").transform;

        anim = GetComponent<Animator>();
    }

    public void Attack()
    {
        GameObject clonedProjectile = Instantiate(projectile, transform.position, Quaternion.identity);


        Debug.Log("뿅");
    }

    void Update()
    {
        
        // X축 거리만 체크 (좌우 차이)
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);

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
    }
}
