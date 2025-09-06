
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public float attackRange = 3f;   // 공격할 거리
    private Transform player;
    private Animator anim;

    void Start()
    {
        // Player 태그 붙은 오브젝트 찾기
        player = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // X축 거리만 체크 (좌우 차이)
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);

        if (distanceX <= attackRange)
        {
            anim.SetBool("isAttack", true);   // 공격 애니메이션
        }
        else
        {
            anim.SetBool("isAttack", false);  // 대기 애니메이션
        }
    }
}
