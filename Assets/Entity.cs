using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EntityType
{
    Player,
    Enemy
}

public abstract class Entity : MonoBehaviour
{
    public float moveSpeed;
    public bool isMoving;
    public bool isAlive;
    public EntityType entityType;

    protected Vector2 forwardVector;
    protected Animation _animation;
    protected AudioSource audioSource;

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
            Debug.Log(LevelManager.Instance.l1Tilemap.GetTile(GridPosition));
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

    private void Start()
    {
        _animation = GetComponent<Animation>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Die()
    {
        _animation.Play("Die");
        audioSource.PlayOneShot(dieSound);
        isAlive = false;
    }

    protected IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        // check if the entity should die
        if (TileBeneath == null)
            Die();
    }
}
