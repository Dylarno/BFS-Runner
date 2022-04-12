using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnifferEnemyBehaviour : Entity
{
    private List<Vector3> smells = new List<Vector3>();

    private void Start()
    {
        // setup event listeners
        PlayerController.PlayerMoved += OnPlayerMoved;
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

        if (smells.Count > 0)
        {

            // move to the next smell
            StartCoroutine(Move(smells[0]));

            // remove the latest smell from the list
            smells.RemoveAt(0);
        } else
        {
            // the sniffer has reached the player, go to them
            StartCoroutine(Move(PlayerController.Instance.transform.position));
        }
    }

    private void OnPlayerMoved()
    {
        // add a smell to the list of smells
        smells.Add(PlayerController.Instance.smells[PlayerController.Instance.smells.Count - 1].position);
    }
}
