using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private CustomColider customCollider;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    private Vector2 moveInput;
    private bool playerHasHorizontalSpeed;
    private bool playerHasVerticalSpeed;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [Header("Climbing")]
    [SerializeField] private float climbingSpeed = 5f;
    
    private float startGravScale;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        customCollider = GetComponent<CustomColider>();
        startGravScale = rb.gravityScale;
    }

    void Update()
    {

        playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;

        Climbing();
        Run();
        FlipSprite();
    }

    private void Climbing()
    {
        // check if player can climb
        if (customCollider.isTouching(LayerMask.GetMask("Ladder"))) {
            //customCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))
        
        // climbing movement
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x, moveInput.y * climbingSpeed);

        // climbing animations
        animator.SetBool("isClimbIdle", true);
        animator.SetBool("isClimbing", playerHasVerticalSpeed);

        }
        else {
            //Reset Gravity
            rb.gravityScale = startGravScale;

            animator.SetBool("isClimbIdle", false);
            animator.SetBool("isClimbing", false);
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value) {
        if(!customCollider.isTouching(LayerMask.GetMask("Ground"), Vector2.down))return; 
        if(value.isPressed) {
            rb.velocity += new Vector2 (0f, jumpForce);
        }
    }
    
    void Run() {

        // check wall collision
        float hInput = moveInput.x;
        if(customCollider.isTouching(LayerMask.GetMask("Ground"), Vector2.right)) hInput = Mathf.Clamp(hInput, float.MinValue, 0f);
        else if(customCollider.isTouching(LayerMask.GetMask("Ground"), Vector2.left)) hInput = Mathf.Clamp(hInput, 0f, float.MaxValue);

        //run calculations
        Vector2 playerVelocity = new Vector2 (hInput * moveSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;
        
        //animation
       animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }
    
    void FlipSprite() {

        playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), transform.localScale.y);
        }
    }
}
