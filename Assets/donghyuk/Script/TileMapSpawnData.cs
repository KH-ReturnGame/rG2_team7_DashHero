using UnityEngine;
using System.Collections.Generic;
// 이 스크립트는 "이 타일맵의 스폰 정보"만 관리함
public class TileMapSpawnData : MonoBehaviour
{
    // 이 타일맵에서 적을 소환할 모든 위치들
    public List<Transform> enemySpawnPoints = new List<Transform>();
    public List<Transform> wizardSpawnPoints = new List<Transform>();
    public List<Transform> flyingSpawnPoints = new List<Transform>();
}
