using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour // may be, class should be called hero
{
    public float moveSpeed = 5f;
    public Vector2 previousPosition;
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        spriteRenderer.flipX = (horizontalInput < 0);

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f);
        movement.Normalize(); // Normalize to prevent faster diagonal movement

        previousPosition = GetComponent<Rigidbody2D>().position;

        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime, Space.World);
    }
}
