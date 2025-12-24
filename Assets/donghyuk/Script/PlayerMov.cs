using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public float speed = 5f;
    public Camera cam;

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        float leftLimit =
            cam.transform.position.x
            - cam.orthographicSize * cam.aspect;

        Vector3 pos = transform.position;

        // 🔥 왼쪽 경계에 있고, 왼쪽으로 가려 하면 이동 차단
        if (pos.x <= leftLimit && move < 0)
        {
            move = 0;
        }

        // 이동
        pos.x += move * speed * Time.deltaTime;
        transform.position = pos;
    }
}

