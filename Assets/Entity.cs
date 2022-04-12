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
    [Header("Entity Config")]
    public float moveSpeed;
    public bool isMoving;
    public bool isAlive;
    public EntityType entityType;

    protected Vector2 forwardVector;

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

    public void Die()
    {
        // ensure isn't already dead
        if (!isAlive)
            return;

        GetComponent<Animation>().Play("Die");
        GetComponent<AudioSource>().PlayOneShot(dieSound);
        isAlive = false;
    }

    protected IEnumerator Move(Vector3 targetPos)
    {
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

        // check if the entity should die
        if (TileBeneath == null)
        {
            Die();
        }
        else
        {
            transform.position = targetPos;
        }
    }
}
