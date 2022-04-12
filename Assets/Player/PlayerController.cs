using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : Entity
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate PlayerMoved;
    public static event PlayerDelegate PlayerStopped;
    public static event PlayerDelegate PlayerDug;
    public static event PlayerDelegate PlayerDied;
    public static event PlayerDelegate PlayerEscaped;
    public static PlayerController Instance { get; private set; }

    public GameObject dugTileSprite;

    [Header("Player Config")]
    public GameObject smellPrefab;

    [Header("Do not touch")]
    public Vector2 input;
    public List<Transform> smells = new List<Transform>();

    private bool hasMoved;

    public GameObject digIndicator;

    public AudioClip digSound;
    public AudioClip portalOpenSound;
    public AudioClip portalEnterSound;
    public AudioClip coinSound;

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
        forwardVector = new Vector3(0, 1);
    }

    void Update()
    {
        // ensure the game is running
        if (!GameController.Instance.isGameRunning)
            return;

        // ensure is alive
        if (!isAlive)
            return;

        HandleMovement();
        HandleDigging();

        if (!isMoving)
        {
            digIndicator.SetActive(true);

            var tileWorldSpace = LevelManager.Instance.l1Tilemap.GetCellCenterWorld(GridLookingLocation);
            var tileWorldSpaceAdjusted = new Vector3(tileWorldSpace.x, tileWorldSpace.y - 0.25f);
            digIndicator.transform.position = tileWorldSpaceAdjusted;
        }

        else
        {
            digIndicator.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ensure that the game is still running
        if (!GameController.Instance.isGameRunning)
            return;

        // check if the other is an enemy
        Entity otherEntity = collision.GetComponent<Entity>();
        if (otherEntity != null && otherEntity.entityType == EntityType.Enemy && otherEntity.isAlive)
        {
            Die();
            if (PlayerDied != null && !isInvincible)
                PlayerDied();

            return;
        }

        switch (collision.tag)
        {
            case "Portal":
                GetComponent<AudioSource>().PlayOneShot(portalEnterSound);

                if (PlayerEscaped != null)
                    PlayerEscaped();

                return;

            case "Coin":
                GetComponent<AudioSource>().PlayOneShot(coinSound);
                Destroy(collision.gameObject);
                return;
        }
    }

    private void HandleMovement()
    {
        // don't move if already moving
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
            if (LevelManager.Instance.l1Tilemap.GetTile(GridLookingLocation) == null)
                return;

            LevelManager.Instance.l1Tilemap.SetTile(GridLookingLocation, null);

            var tileWorldSpace = LevelManager.Instance.l1Tilemap.GetCellCenterWorld(GridLookingLocation);
            var tileWorldSpaceAdjusted = new Vector3(tileWorldSpace.x, tileWorldSpace.y - 0.25f);
            var animatedDugTile = Instantiate(dugTileSprite, tileWorldSpaceAdjusted, Quaternion.identity);

            Destroy(animatedDugTile, 1.0f);

            GetComponent<AudioSource>().PlayOneShot(digSound);

            if (PlayerDug != null)
                PlayerDug();
        }
    }

    private new IEnumerator Move(Vector3 targetPos)
    {
        if (doLog)
            Debug.Log("HERE");

        isMoving = true;
        bool isDone = false;

        while (!isDone && (targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // check if the entity should die
            if (TileBeneath == null)
                isDone = true;

            yield return null;
        }

        isMoving = false;

        if (PlayerStopped != null)
            PlayerStopped();

        // check if the entity should die
        if (TileBeneath == null)
        {
            Die();
            if (PlayerDied != null && !isInvincible)
                PlayerDied();
        }
        else
        {
            transform.position = targetPos;
        }
    }
}
