using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public Transform mainCamera;   // Main Camera 할당
    private bool isAttached = true;

    private float detachX;         // 떨어질 때 저장되는 x좌표

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        float playerX = transform.position.x;

        // 1. 왼쪽으로 이동 → 카메라 분리 + x좌표 저장
        if (move < 0 && isAttached)
        {
            mainCamera.SetParent(null);
            detachX = playerX;     // 떨어진 시점의 x좌표 저장
            isAttached = false;
        }

        // 2. 오른쪽으로 이동하면서 저장된 x좌표를 지나면 → 다시 붙임
        if (!isAttached && move >= 0)
        {
            if (playerX >= detachX)   // 저장된 x를 다시 지나침
            {
                mainCamera.SetParent(this.transform);
                mainCamera.localPosition = Vector3.zero;  
                isAttached = true;
            }
        }
    }
}
