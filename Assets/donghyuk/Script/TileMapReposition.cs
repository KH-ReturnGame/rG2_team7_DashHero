using Unity.Hierarchy;
using UnityEditor.ShaderGraph.Internal;
using System.Collections.Generic;
using UnityEngine;

public class TileMapReposition : MonoBehaviour
{
    public Transform[] tilemaps;
    public BoxCollider2D cameraTrigger;
    public float sectionLength = 14f;
    
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public GameObject wizardEnemyPrefab;
    public GameObject flyingEnemyPrefab;

    
    void Start()
    {
        // 게임 시작 시, 모든 타일맵에서 한 번씩 몬스터 스폰
        foreach (Transform map in tilemaps)
        {
            SpawnEnemies(map);
        }
    }
    void Update()
    {
        foreach (Transform map in tilemaps)
        {
            if (map.position.x + sectionLength < cameraTrigger.bounds.min.x)
            {
                float rightMostX = FindRightMostX();
                map.position = new Vector3(rightMostX + sectionLength, map.position.y, map.position.z);
                // 이 맵에 있는 모든 스폰 포인트에서 적 소환
                SpawnEnemies(map);
            }
            
        }
    }

    void SpawnEnemies(Transform map)
    {
        
        
        TileMapSpawnData spawnData = map.GetComponent<TileMapSpawnData>();
        if (spawnData == null) return;

        if (enemyPrefabs.Count == 0) return;

        foreach (Transform spawnPoint in spawnData.enemySpawnPoints)
        {
            if (spawnPoint == null) continue;
            
            int randomIndex = Random.Range(0, enemyPrefabs.Count);
            GameObject selectedEnemy = enemyPrefabs[randomIndex];

            Instantiate(selectedEnemy, spawnPoint.position, Quaternion.identity);
        }

        if (wizardEnemyPrefab != null)
        {
            foreach (Transform spawnPoint in spawnData.wizardSpawnPoints)
            {
                if (spawnPoint == null) continue;

                Instantiate(wizardEnemyPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
        
        if (flyingEnemyPrefab != null)
        {
            foreach (Transform spawnPoint in spawnData.flyingSpawnPoints)
            {
                if (spawnPoint == null) continue;

                Instantiate(flyingEnemyPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }

    float FindRightMostX()
    {
        float maxX = float.MinValue;
        foreach (Transform m in tilemaps)
        {
            if (m.position.x > maxX)
                maxX = m.position.x;
        }

        return maxX;
    }
}



