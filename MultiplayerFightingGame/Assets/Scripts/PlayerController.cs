using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movements")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    private bool onGround;
    float moveX = 0f;

    // Jumping
    private float lastJump;
    private float jumpCooldown = 0.2f;

    // Crouching
    private bool crouching;

    // References
    private Rigidbody2D rb;
    private Vector3 initialScale;
    private PlayerControls playerControls;
    private Animator animator;
    private string currentAnimState;

    // Animation States
    const string PLAYER_IDLE = "PlayerIdle";
    const string PLAYER_JUMP_UP = "PlayerJumpUp";
    const string PLAYER_JUMP_DOWN = "PlayerJumpDown";
    const string PLAYER_WALK = "PlayerWalk";

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
        playerControls.Player.Jump.performed += Jump;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialScale = transform.localScale;
    }

    void FixedUpdate()
    {
        // Left & Right Movements
        moveX = 0f;
        if (playerControls.Player.MoveHorizontal.ReadValue<float>() > 0f)
        {
            moveX += 1f;
        }

        if (playerControls.Player.MoveHorizontal.ReadValue<float>() < 0f)
        {
            moveX -= 1f;
        }

        // Crouching
        float distToGround = GetComponent<Collider2D>().bounds.extents.y;
        onGround = Physics2D.Raycast(transform.position, Vector2.down, distToGround + 0.1f, groundLayer);

        if (playerControls.Player.Crouch.ReadValue<float>() == 1f && onGround)
        {
            // Play Crouch Animation / State
            crouching = true;
        }
        else if (playerControls.Player.Crouch.ReadValue<float>() == 0f)
        {
            crouching = false;
        }

        // Manage Crouching Animations
        if (crouching)
        {
            Crouch();
            if (Mathf.Abs(playerControls.Player.MoveHorizontal.ReadValue<float>()) > 0.7f || rb.velocity.y > 0.1f)
            {
                UnCrouch();
            }
        }
        else
        {
            UnCrouch();
        }

        // Manage Idle & Walking Animations
        if (onGround && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            if (moveX == 0)
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
            else
            {
                ChangeAnimationState(PLAYER_WALK);
            }
        }

        // Manage Jump Animations
        if (!onGround && rb.velocity.y < 0)
        {
            ChangeAnimationState(PLAYER_JUMP_DOWN);
        }

        if (!crouching)
        {
            rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        float distToGround = GetComponent<Collider2D>().bounds.extents.y;
        onGround = Physics2D.Raycast(transform.position, Vector2.down, distToGround + 0.1f, groundLayer);
        if (onGround && Time.time - lastJump > jumpCooldown)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJump = Time.time;
            ChangeAnimationState(PLAYER_JUMP_UP);
            onGround = false;
        }
    }

    private void Crouch()
    {
        // ChangeAnimationState(PLAYER_CROUCH);
        transform.localScale = new Vector3(initialScale.x, 0.5f * initialScale.y, initialScale.z);
        moveX = 0f;
    }

    private void UnCrouch()
    {
        // Check if moving then ChangeAnimationState(PLAYER_MOVE)
        // Or check if not moving then ChangeAnimationState(Player_IDLE)
        crouching = false;
        transform.localScale = initialScale;
    }

    void ChangeAnimationState(string newState)
    {
        if (currentAnimState == newState) return;

        animator.Play(newState);

        currentAnimState = newState;
    }
}
