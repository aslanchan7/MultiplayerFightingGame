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
        if (Input.GetKeyDown(jumpKey))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    }
}
