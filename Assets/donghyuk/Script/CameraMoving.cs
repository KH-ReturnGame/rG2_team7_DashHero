using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public Transform mainCamera;  
    private bool isAttached = true;

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        // 1. 왼쪽으로 이동하면 카메라 분리
        if (move < 0 && isAttached)
        {
            mainCamera.SetParent(null); 
            isAttached = false;
        }

        // 2. 오른쪽 또는 입력 없음 → 카메라 다시 Player에 붙임
        if (move >= 0 && !isAttached)
        {
            mainCamera.SetParent(this.transform);        // Player 아래로 다시 붙이기
            mainCamera.localPosition = Vector3.zero;     // 붙인 뒤 위치 초기화
            isAttached = true;
        }
    }
}
