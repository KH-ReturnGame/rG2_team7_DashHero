using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDead : MonoBehaviour
{
    void Update()
    {
        // Y 좌표가 -6 이하면 destroy
        if (transform.position.y <= -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // 플레이어가 destroy될 때 DeadScene으로 이동
        SceneManager.LoadScene("Dead Scene");
    }
}
