using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidPlayer;
    [SerializeField] private Transform playerSprite; // Reference to the sprite, not the whole player object
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jump;
    [SerializeField] private float runSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] public float jumpCooldown;
    [SerializeField] public Animator walkingAnimation;
    private bool isFacingRight;
    private bool isGrounded;
    private bool readyToJump;
    private float currentSpeed;

    private void Start()
    {
        readyToJump = true;
        isFacingRight = true;
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        MyInput();

        if (rigidPlayer.velocity.x != 0)
        {
            walkingAnimation.SetBool("isWalking", true);
        }
        else
        {
            walkingAnimation.SetBool("isWalking", false);
        }
        if (rigidPlayer.velocity.y != 0)
        {
            walkingAnimation.SetBool("isJumping", true);
        }
        else
        {
            walkingAnimation.SetBool("isJumping", false);
        }
    }

    // To visualize the ground check in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void MyInput()
    {
        float horizontalInput = 0f;
        float currentSpeed = moveSpeed;


        // Handle left movement
        if (Input.GetKey(KeyCode.A))
        {
            if (isFacingRight)
            {
                Flip();
            }
            horizontalInput = -1f;
        }

        // Handle right movement
        if (Input.GetKey(KeyCode.D))
        {
            if (!isFacingRight)
            {
                Flip();
            }
            horizontalInput = 1f;
        }

        // Handle running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= runSpeed;
        }

        // Apply horizontal movement by modifying Rigidbody2D's velocity
        rigidPlayer.velocity = new Vector2(horizontalInput * currentSpeed, rigidPlayer.velocity.y);

        // Jump Input (only if grounded)
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && isGrounded && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Jump()
    {
        // Reset vertical velocity before jumping
        rigidPlayer.velocity = new Vector2(rigidPlayer.velocity.x, 0);

        // Apply jump force
        rigidPlayer.AddForce(new Vector2(0, jump), ForceMode2D.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        // Flip the sprite by negating the x scale, without affecting the physics
        Vector3 scale = playerSprite.localScale;
        scale.x *= -1;
        playerSprite.localScale = scale;
    }
}
