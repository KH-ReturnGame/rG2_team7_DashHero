using UnityEngine;   // Unity 기본 기능 사용 (Transform, MonoBehaviour 등)
using TMPro;         // TextMeshPro 사용하기 위한 네임스페이스

// 점수 관리를 담당하는 스크립트
public class ScoreManager : MonoBehaviour

{
    public Transform player;   // 플레이어
    public TMP_Text tmpText;   // TMP 텍스트

    float maxX;

    void Start()
    {
        // 시작 시 현재 위치를 최대값으로 초기화
        maxX = player.position.x;
    }

    void Update()
    {
        // 현재 x좌표가 기존 최대값보다 크면 갱신
        if (player.position.x > maxX)
        {
            maxX = player.position.x;
        }

        // 최대 x좌표를 정수로 변환해서 출력
        int maxXInt = Mathf.FloorToInt(maxX);
        tmpText.text = "Score: " + maxXInt.ToString();
    }
}