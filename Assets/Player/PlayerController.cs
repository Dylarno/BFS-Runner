using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : Entity
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate PlayerMoved;
    public static event PlayerDelegate PlayerDug;
    public static event PlayerDelegate PlayerDied;
    public static event PlayerDelegate PlayerEscaped;
    public static PlayerController Instance { get; private set; }

    [Header("Player Config")]
    public GameObject smellPrefab;

    [Header("Do not touch")]
    public Vector2 input;
    public List<Transform> smells = new List<Transform>();

    private bool hasMoved;

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
        // create a smell for sniffers
        smells.Add(Instantiate(smellPrefab, transform.position, Quaternion.identity).transform);
    }

    void Update()
    {
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

        Vector3 targetPos = transform.position;

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

        // move the player
        MovePlayer(targetPos);
    }

    private void MovePlayer(Vector3 targetPos)
    {
        if (hasMoved)
        {
            // create a new smell and add it to the list of smells
            smells.Add(Instantiate(smellPrefab, transform.position, Quaternion.identity).transform);
        } else
            hasMoved = true;

        // call playerMoved
        if (PlayerMoved != null)
            PlayerMoved();

        // move the player
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

            // dig
            LevelManager.Instance.l1Tilemap.SetTile(GridLookingLocation, null);
            if (PlayerDug != null)
                PlayerDug();
        }
    }
}
