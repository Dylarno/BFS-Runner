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
    public bool doLog;
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

    protected List<Vector3Int> GetNavigationTo(Vector3 target)
    {
        Vector3Int startingPosition = LevelManager.Instance.grid.WorldToCell(transform.position);
        Vector3Int targetPosition = LevelManager.Instance.grid.WorldToCell(PlayerController.Instance.transform.position);

        if (doLog)
        {
            Debug.LogFormat("Starting Position (local space): {0}", startingPosition);
            Debug.LogFormat("Target Position (local space): {0}", targetPosition);
        }

        List<CameFromVector3> cameFrom = FindTargetBFS(startingPosition, targetPosition);
        List<Vector3Int> travelWaypoints = GetPathToTargetBFS(cameFrom, startingPosition, targetPosition);

        travelWaypoints.Add(startingPosition);
        travelWaypoints.Reverse();

        return travelWaypoints;
    }

    private List<CameFromVector3> FindTargetBFS(Vector3Int startingPosition, Vector3Int targetPosition)
    {
        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        frontier.Enqueue(startingPosition);
        List<CameFromVector3> cameFrom = new List<CameFromVector3>();
        cameFrom.Add(new CameFromVector3(startingPosition, startingPosition));

        // breadth-first search until target is found
        while (frontier.Count > 0)
        {
            // get the last position in the frontier queue
            Vector3Int current = frontier.Dequeue();
            
            // - o -
            // o + o
            // - o -
            //
            // add neighbors to the search
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (y == 0 && (x == -1 || x == 1)
                        || (y != 0 && x == 0))
                    {
                        // get the current neighbor
                        Vector3Int neighbor = new Vector3Int(current.x + x, current.y + y, current.z);

                        if (doLog)
                            Debug.LogFormat("Searching neighboring cell: {0}", neighbor);

                        TileBase neighborTile = LevelManager.Instance.l1Tilemap.GetTile(neighbor);

                        if (cameFrom.FindIndex(c => (c.value == neighbor)) < 0
                            && neighborTile != null)
                        {
                            if (doLog)
                                Debug.LogFormat("Adding {0} which came from {1}", neighbor, current);

                            // add the next place to the queue
                            frontier.Enqueue(neighbor);

                            // set this position as the cameFrom for the next position
                            cameFrom.Add(new CameFromVector3(neighbor, current));

                            // check if this is the target
                            if (neighbor == targetPosition)
                            {
                                if (doLog)
                                    Debug.Log("TARGET FOUND! Came from: " + current);
                                return cameFrom;
                            }
                        }
                    }
                }
            }
        }

        return cameFrom;
    }

    private List<Vector3Int> GetPathToTargetBFS(List<CameFromVector3> cameFrom, Vector3Int startingPosition, Vector3Int targetPosition)
    {
        // now have a BFS list of visited locations

        // reset travelwaypoints
        List<Vector3Int> travelWaypoints = new List<Vector3Int>();
        Vector3Int currentSearch = targetPosition;

        if (doLog)
            Debug.Log("First current search" + currentSearch);

        int i = 0;

        while (currentSearch != startingPosition && i < 9999)
        {
            i++;
            travelWaypoints.Add(currentSearch);

            Vector3Int nextCurrentSearch;
            bool failed = false;
            failed = !cameFrom.Exists(c => c.value == currentSearch);

            Debug.Log(cameFrom.FindAll(c => c.value == currentSearch).Count);

            if (failed)
            {
                if (doLog)
                    Debug.LogFormat("Something went wrong");
                return travelWaypoints;
            }
            else
            {
                nextCurrentSearch = cameFrom.Find(c => c.value == currentSearch).cameFrom;
                if (doLog)
                    Debug.Log("Next position from target (backtracking): " + nextCurrentSearch);
                currentSearch = nextCurrentSearch;
            }
        }

        return travelWaypoints;
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
