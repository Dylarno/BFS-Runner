using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : Entity
{
    public static PlayerController Instance { get; private set; }

    public Vector2 input;

    public TileBase thisTileBeneath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _animation = GetComponent<Animation>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        thisTileBeneath = TileBeneath;

        // ensure is alive
        if (!isAlive)
            return;

        HandleMovement();
        HandleDigging();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.GetComponent<Entity>().entityType)
        {
            case EntityType.Enemy:
                Die();
                break;
        }
    }

    private void HandleMovement()
    {
        if (isMoving) return;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input == Vector2.zero) return;
        if (input.x != 0) input.y = 0;

        // calculate forward vector
        forwardVector = input;

        var targetPos = transform.position;

        if (input.x != 0)
        {
            targetPos.x += input.x / 2f;
            targetPos.y -= 0.25f * Mathf.Sign(input.x);
        }

        else if (input.y != 0)
        {
            targetPos.y += input.y / 4f;
            targetPos.x += 0.5f * Mathf.Sign(input.y);
        }

        StartCoroutine(Move(targetPos));
    }

    private void HandleDigging()
    {
        // check for player digging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ensure the player is not moving
            if (isMoving)
                return;

            LevelManager.Instance.l1Tilemap.SetTile(GridLookingLocation, null);
        }
    }
}
