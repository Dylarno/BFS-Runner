using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnifferEnemyBehaviour : Entity
{
    private List<Vector3> targets = new List<Vector3>();

    private void Start()
    {
        // setup event listeners
        PlayerController.PlayerMoved += OnPlayerMoved;

        foreach (Vector3Int pos in GetNavigationTo(PlayerController.Instance.transform.position))
        {
            targets.Add(new Vector3(0, 0.25f, 0) + LevelManager.Instance.grid.CellToWorld(pos));
        }
    }

    private void OnDestroy()
    {
        // clear event listeners
        PlayerController.PlayerMoved -= OnPlayerMoved;
    }

    private void Update()
    {
        // ensure the sniffer is alive
        if (!isAlive)
            return;
        
        // ensure the sniffer is not already moving
        if (isMoving)
            return;

        if (targets.Count > 0)
        {
            // move to the next smell
            StartCoroutine(Move(targets[0]));

            // remove the latest smell from the list
            targets.RemoveAt(0);
        } else
        {
            // the sniffer has reached the player, go to them
            StartCoroutine(Move(PlayerController.Instance.transform.position));
        }
    }

    private void OnPlayerMoved()
    {
        // add a smell to the list of smells
        targets.Add(PlayerController.Instance.smells[PlayerController.Instance.smells.Count - 1].position);
    }
}
