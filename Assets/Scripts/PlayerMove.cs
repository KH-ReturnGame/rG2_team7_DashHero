using UnityEngine;

public class playerMonoBehavior : MonoBehaviour
{   //변수지정

    public float MoveSpeed = 6f;
    private Rigidbody2D rb;
    private float MoveInput;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
        MoveInput = Input.GetAxisRaw("Horizontal"); //GetAxisRaw는 -1,0,1으로 축의 값 정해줌





    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(MoveInput * MoveSpeed, rb.linearVelocity.y);//방향있는 속력
    }
}