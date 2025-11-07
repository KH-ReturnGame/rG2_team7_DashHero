using Unity.Hierarchy;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TileMapReposition : MonoBehaviour
{
    public Transform[] tilemaps;
    public BoxCollider2D cameraTrigger;
    public float sectionLength = 14f;

    void Update()
    {
        foreach (Transform map in tilemaps)
        {
            if (map.position.x + sectionLength < cameraTrigger.bounds.min.x)
            {
                float rightMostX = FindRightMostX();
                map.position = new Vector3(rightMostX + sectionLength, map.position.y, map.position.z);
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



