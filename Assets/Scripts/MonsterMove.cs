
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public float attackRange;   // ������ �����ֱ�
    private Transform player;
    private Animator anim;
    
    void Start()
    {
        // Player �±� ���� ������Ʈ ã��
        player = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        
        // X�� �Ÿ��� üũ (�¿� ����)
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);

        if (distanceX <= attackRange)
        {
           // Debug.Log("���ݰ��� �� ������");
            anim.SetBool("isAttack", true);   // ���� �ִϸ��̼�
        }
        else
        {
           // Debug.Log("���� �Ұ���");
            anim.SetBool("isAttack", false);  // ��� �ִϸ��̼�
        }
    }
}
