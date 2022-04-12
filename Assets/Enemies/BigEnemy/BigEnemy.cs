using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CameFromVector3
{
    public CameFromVector3(Vector3Int _value, Vector3Int _cameFrom)
    {
        value = _value;
        cameFrom = _cameFrom;
    }

    public Vector3Int value;
    public Vector3Int cameFrom;
}

public class BigEnemy : Entity
{
    public List<Vector3Int> travelWaypoints = new List<Vector3Int>();
    private Vector3 playerPosition;

    private void Start()
    {
        // setup event listeners
        PlayerController.PlayerMoved += OnPlayerMoved;

        ChasePlayer();
    }

    private void OnDestroy()
    {
        // clear event listeners
        PlayerController.PlayerMoved -= OnPlayerMoved;
    }
    private void OnPlayerMoved()
    {
        playerPosition = PlayerController.Instance.transform.position;
    }

    private void ChasePlayer()
    {
        isMoving = false;
        FindPlayer();
        StartCoroutine(Move());
    }

    private void FindPlayer()
    {
        Vector3Int targetPosition = LevelManager.Instance.grid.WorldToCell(PlayerController.Instance.transform.position);

        travelWaypoints = GetNavigationTo(targetPosition);
    }

    protected IEnumerator Move()
    {
        isMoving = true;
        bool isDone = false;

        Vector3 targetPos = new Vector3(0, 0.25f, 0) + LevelManager.Instance.grid.CellToWorld(travelWaypoints[travelWaypoints.Count - 1]);
        Vector3 lastTargetPosition = playerPosition;

        for (int i = 0; i < travelWaypoints.Count; i++)
        {
            // ensure we are not done
            if (isDone)
                continue;

            // check if the player has moved
            if (Vector3.Distance(playerPosition, lastTargetPosition) > float.Epsilon)
            {
                // re-find the player
                FindPlayer();

                // reset variables
                lastTargetPosition = playerPosition;
                i = 0;
            }

            // get the target position
            targetPos = new Vector3(0, 0.25f, 0) + LevelManager.Instance.grid.CellToWorld(travelWaypoints[i]);

            while (!isDone && (targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

                // check if the entity should die
                if (TileBeneath == null)
                    isDone = true;

                yield return null;
            }
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
