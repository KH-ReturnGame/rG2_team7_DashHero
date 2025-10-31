
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public float attackRange;   // 변수값 정해주기
    private Transform player;
    private Animator anim;

    public GameObject projectile;


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
