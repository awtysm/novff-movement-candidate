using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Controller))]
public class Player : MonoBehaviour
{
    Controller con;
    
    [SerializeField][Range(10,150)] private int terminalVelocity = 30;
    [SerializeField][Range(1,15)] private int MovementSpeed = 8;
    [SerializeField][Range(1,8)] private int jumpHeight = 12;
    [SerializeField][Range(0.01f,1f)] private float timeToJumpApex = 0.4f;
    [SerializeField][Range(0.01f,0.5f)] private float accelerationTimeGrounded = 0.5f;
    [SerializeField][Range(0.01f,0.5f)] private float accelerationTimeInAir = 0.02f;
    private float gravity;  
    private float jumpVelocity;
    private float currentVelocity;
    Vector3 velocity;
    SpriteRenderer sprite;
    
    void Awake()
    {
        con = GetComponent<Controller>();
        sprite = GetComponent<SpriteRenderer>();

        gravity = (-2* jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }
    void Update()
    {
        if(con.collisions.above || con.collisions.below)
            velocity.y = 0;
        Vector2 PlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        float targetHorVelocity = PlayerInput.x * MovementSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetHorVelocity,ref currentVelocity, (con.collisions.below)?accelerationTimeGrounded:accelerationTimeInAir);
        
        
        if(Input.GetKey(KeyCode.Space) && con.collisions.below)
            velocity.y = jumpVelocity;

        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -terminalVelocity, terminalVelocity);
        
        if(PlayerInput.x != 0)
            sprite.flipX = Mathf.Sign(targetHorVelocity) == -1;

        con.Move(velocity * Time.deltaTime);

    }
}
