using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 100f;
    
    private float lastPlayerX;

    void Start()
    {
        if (player != null)
        {
            lastPlayerX = player.position.x;
            // 시작 시 카메라를 플레이어 위치로 초기화
            transform.position = new Vector3(player.position.x + 10, player.position.y + 4, transform.position.z);
        }
    }

    void LateUpdate() // Update 대신 LateUpdate 사용
    {
        if (player == null) return;

        float currentX = player.position.x;
        bool movingRight = currentX > lastPlayerX;

        if (movingRight)
        {
            Vector3 targetPos = new Vector3(player.position.x + 10, player.position.y + 4, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }

        lastPlayerX = currentX;
    }
}