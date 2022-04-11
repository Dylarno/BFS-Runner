using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public bool isMoving;

    private Vector2 input;
    void Update()
    {
        if (isMoving) return;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input == Vector2.zero) return;
        if (input.x != 0) input.y = 0;

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

        // check for player digging
        if (Input.GetKeyDown(KeyCode.Space))
            Dig();
    }

    private void Dig()
    {
        // ensure the player is not moving
        if (isMoving)
            return;

        // get the tile benenath the player
        
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
    }
}
