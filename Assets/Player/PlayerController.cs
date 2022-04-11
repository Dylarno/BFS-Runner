using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public float moveSpeed;
    public bool isMoving;
    public bool isAlive;

    public Vector2 input;
    private Vector2 forwardVector;
    private Animation _animation;
    private AudioSource audioSource;

    public AudioClip dieSound;

    public TileBase TileInFront
    {
        get
        {
            return LevelManager.Instance.l1Tilemap.GetTile(GridLookingLocation);
        }
    }

    public TileBase TileBeneath
    {
        get
        {
            return LevelManager.Instance.l1Tilemap.GetTile(GridPosition);
        }
    }

    public Vector3Int GridPosition
    {
        get
        {
            return LevelManager.Instance.grid.WorldToCell(transform.position);
        }
    }

    public Vector3Int GridLookingLocation
    {
        get
        {
            return GridPosition + Vector3Int.RoundToInt(new Vector3(forwardVector.y, -forwardVector.x, 0));
        }
    }

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
        // ensure is alive
        if (!isAlive)
            return;

        HandleMovement();
        HandleDigging();
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

    public void Die()
    {
        _animation.Play("Die");
        audioSource.PlayOneShot(dieSound);
        isAlive = false;
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        // check if the player should die
        if (TileBeneath == null)
            Die();
    }
}
