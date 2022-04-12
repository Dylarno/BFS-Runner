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
    public List<CameFromVector3> cameFrom;
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
        Vector3Int startingPosition = LevelManager.Instance.grid.WorldToCell(transform.position);
        Vector3Int targetPosition = LevelManager.Instance.grid.WorldToCell(PlayerController.Instance.transform.position);

        FindTarget(startingPosition, targetPosition);
        FindPathToTarget(startingPosition, targetPosition);

        travelWaypoints.Add(startingPosition);
        travelWaypoints.Reverse();
    }

    private void FindTarget(Vector3Int startingPosition, Vector3Int targetPosition)
    {
        //Vector3Int startingPosition = Vector3Int.RoundToInt(
        //    LevelManager.Instance.grid.WorldToLocal(transform.position));
        //Vector3Int targetPosition = Vector3Int.RoundToInt(
        //    LevelManager.Instance.grid.WorldToLocal(targetPos));


        Debug.LogFormat("Starting Position (local space): {0}", startingPosition);
        Debug.LogFormat("Target Position (local space): {0}", targetPosition);

        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        frontier.Enqueue(startingPosition);
        // HashSet<Vector3> visited = new HashSet<Vector3>();
        //cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        cameFrom = new List<CameFromVector3>();
        //List<CameFromVector3> cameFrom = new List<CameFromVector3>();
        //cameFrom.Add(new CameFromVector3(transform.position, transform.position));
        cameFrom.Add(new CameFromVector3(startingPosition, startingPosition));

        // breadth-first search until target is found
        while (frontier.Count > 0) {
            // get the last position in the frontier queue
            Vector3Int current = frontier.Dequeue();
            //TileBase currentTile = LevelManager.Instance.GetTileAtPosition(current.x, current.y);

            //Debug.LogFormat("Searching frontier: {0}, tile: {1}", current, currentTile);

            // make sure the position hasn't been visited
            //if (cameFrom.ContainsKey(current))
            //if (cameFrom.FindIndex(c => (Vector3.Distance(c.cameFrom, current) <= Mathf.Epsilon)) > 0)
            //    continue;

            // make sure the tile isn't null
            //if (currentTile == null)
            //    continue;

            // - o -
            // o - o
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

                        Debug.LogFormat("Searching neighboring cell: {0}", neighbor);

                        //TileBase neighborTile = LevelManager.Instance.GetTileAtPosition(neighbor.x, neighbor.y);
                        TileBase neighborTile = LevelManager.Instance.l1Tilemap.GetTile(neighbor);

                        //if (cameFrom.FindIndex(c => (c.value == neighbor) > 0
                        //if (!cameFrom.ContainsKey(neighbor)
                        if (cameFrom.FindIndex(c => (c.value == neighbor)) < 0
                            && neighborTile != null)
                        {
                            Debug.LogFormat("Adding {0} which came from {1}", neighbor, current);

                            // add the next place to the queue
                            frontier.Enqueue(neighbor);

                            // set this position as the cameFrom for the next position
                            //cameFrom.Add(new CameFromVector3(neighbor, current));
                            cameFrom.Add(new CameFromVector3(neighbor, current));

                            // check if this is the target
                            if (neighbor == targetPosition)
                            {
                                Debug.Log("TARGET FOUND! Came from: " + current);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private void FindPathToTarget(Vector3Int startingPosition, Vector3Int targetPosition)
    {
        // now have a BFS list of visited locations

        // reset travelwaypoints
        travelWaypoints.Clear();
        Vector3Int currentSearch = targetPosition;

        Debug.Log("First current search" + currentSearch);

        //allPoints = cameFrom;

        //while currentSearch is not the current position(traced back to the start)
        //while (Vector3.Distance(currentSearch, transform.position) > Mathf.Epsilon)

        int i = 0;

        while (currentSearch != startingPosition && i < 9999)
        {
            i++;
            travelWaypoints.Add(currentSearch);
            //currentSearch = cameFrom.Find(c => (Vector3.Distance(c.cameFrom, currentSearch) <= Mathf.Epsilon)).cameFrom;

            Vector3Int nextCurrentSearch;
            bool failed = false;
            //nextCurrentSearch = cameFrom[currentSearch];
            //failed = cameFrom.TryGetValue(currentSearch, out nextCurrentSearch);
            failed = !cameFrom.Exists(c => c.value == currentSearch);

            Debug.Log(cameFrom.FindAll(c => c.value == currentSearch).Count);

            if (failed)
            {
                Debug.LogFormat("Something went wrong");
                return;
            }
            else
            {
                nextCurrentSearch = cameFrom.Find(c => c.value == currentSearch).cameFrom;
                Debug.Log("Next position from target (backtracking): " + nextCurrentSearch);
                currentSearch = nextCurrentSearch;
            }
        }
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
