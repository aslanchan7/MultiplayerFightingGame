using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movements")]
    [SerializeField] KeyCode moveLeftKey = KeyCode.A;
    [SerializeField] KeyCode moveRightKey = KeyCode.D;
    [SerializeField] KeyCode jumpKey = KeyCode.W;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundLayer;
    private bool onGround;

    // References
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = 0f;

        // Left & Right Movements
        if (Input.GetKey(moveRightKey))
        {
            moveX += 1f;
        }

        if (Input.GetKey(moveLeftKey))
        {
            moveX += -1f;
        }

        // Jumping
        float distToGround = GetComponent<Collider2D>().bounds.extents.y;
        onGround = Physics2D.Raycast(transform.position, Vector2.down, distToGround + 0.1f, groundLayer);

        if (Input.GetKeyDown(jumpKey) && onGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    }
}
