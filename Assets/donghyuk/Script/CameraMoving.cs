using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    
    public Transform player;

    // 카메라가 따라오는 부드러운 속도
    public float followSpeed = 5f;

    // 이전 프레임 플레이어 X값 저장
    private float lastPlayerX;

    void Start()
    {
        // 첫 시작 시 플레이어 X값 기록
        lastPlayerX = player.position.x;
    }

    void Update()
    {
        
        // 현재 플레이어 X
        float currentX = player.position.x;

        // 오른쪽으로 이동 중인지 체크
        bool movingRight = currentX > lastPlayerX;

        // 오른쪽으로 이동할 때만 카메라가 따라옴
        if (movingRight)
        {
            // 카메라가 따라가야 할 목표 위치
            // X도 따라가고, Y도 따라가고, Z는 현재 유지
            Vector3 targetPos = new Vector3(player.position.x+10, player.position.y+4, transform.position.z);

            // 부드럽게 따라오도록 Lerp 사용
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }

        // 이후 비교를 위한 X값 갱신
        lastPlayerX = currentX;
    }
}
