using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 1;

    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        // Animator가 자식에 붙어있는 경우가 많아서 InChildren 추천
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        Debug.Log($"{gameObject.name} 맞음! 남은 HP: {hp}");

        if (hp <= 0)
        {
            isDead = true;

            // 죽는 애니메이션 트리거 (Animator에 Trigger "Die" 필요)
            if (anim != null) anim.SetTrigger("Die");
            else Destroy(gameObject); // Animator 없으면 바로 삭제(안전장치)

            // Destroy는 여기서 하지 말기!
            // 애니 끝에서 Animation Event로 DestroySelf() 호출
        }
    }

    // Death 애니 마지막 프레임에 Animation Event로 이 함수 호출해줘
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

