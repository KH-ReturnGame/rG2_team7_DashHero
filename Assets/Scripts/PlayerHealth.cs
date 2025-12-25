using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isInvincible = false;
    public int hp = 1; // 플레이어의 체력

    // 몬스터의 공격 스크립트에서 호출되는 함수
    public void TakeDamage(int damage)
    {

        
            if (isInvincible) return; // 대쉬 중이면 데미지 무시

            // 기존 데미지 처리...
        
    
    // 공격을 받으면 즉시 체력을 0으로 설정 (한 번에 죽음)
    hp = 0;

        Debug.Log($"{gameObject.name} (플레이어)가 공격을 받아 사망했습니다.");

        Die();
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");

        // [수정된 부분] 오브젝트를 완전히 삭제하지 않고 비활성화(숨김) 처리합니다.
        Destroy(gameObject);

        // 주의: 만약 카메라가 플레이어의 자식(Child)으로 붙어있다면, 
        // 플레이어가 비활성화될 때 카메라 화면도 같이 꺼질 수 있습니다.
        // 그럴 경우 아래 주석을 해제하여 카메라를 부모로부터 독립시켜야 합니다.
        // if (Camera.main != null && Camera.main.transform.parent == transform)
        // {
        //     Camera.main.transform.SetParent(null);
        // }
    }
}