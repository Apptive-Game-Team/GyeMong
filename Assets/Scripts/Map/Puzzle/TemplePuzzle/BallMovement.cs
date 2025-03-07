using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private float delayTime = 1f;
    public TempleTile currentTile;
    private bool isMoving = false;

    void Start()
    {
        currentTile = GetCurrentTile();
    }

    public void StartMoveBall()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveBall());
        }
    }

    IEnumerator MoveBall()
    {
        if (currentTile != null)
        {
            isMoving = true;

            Vector2 direction = GetMoveDirection();

            while (direction != Vector2.zero)
            {
                Vector3 targetPosition = currentTile.transform.position + (Vector3)direction;

                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                transform.position = targetPosition;

                if (currentTile != null)
                {
                    currentTile.iswalked = true;
                }

                yield return new WaitForSeconds(delayTime);

                currentTile = GetCurrentTile();

                direction = GetMoveDirection();
            }

            if (GoalCheck())
            {
                Debug.Log("Success");
            }

            else
            {
                Debug.Log("Nope");
            }

            isMoving = false;
        }
    }

    Vector2 GetMoveDirection()
    {
        if (CanMoveToTile(currentTile.transform.position, Vector2.up) && currentTile.up)
        {
            return Vector2.up;
        }
        else if (CanMoveToTile(currentTile.transform.position, Vector2.down) && currentTile.down)
        {
            return Vector2.down;
        }
        else if (CanMoveToTile(currentTile.transform.position, Vector2.left) && currentTile.left)
        {
            return Vector2.left;
        }
        else if (CanMoveToTile(currentTile.transform.position, Vector2.right) && currentTile.right)
        {
            return Vector2.right;
        }

        return Vector2.zero;
    }

    bool CanMoveToTile(Vector3 position, Vector2 direction)
    {
        Vector3 checkPosition = position + (Vector3)direction;

        Collider2D hit = Physics2D.OverlapBox(checkPosition, new Vector2(0.1f, 0.1f), 0f);

        if (hit != null && hit.CompareTag("Tile"))
        {
            TempleTile tile = hit.GetComponent<TempleTile>();
            if (tile != null)
            {
                if (direction == Vector2.up && tile.down && !tile.iswalked)
                {
                    return true;
                }
                if (direction == Vector2.down && tile.up && !tile.iswalked)
                {
                    return true;
                }
                if (direction == Vector2.left && tile.right && !tile.iswalked)
                {
                    return true;
                }
                if (direction == Vector2.right && tile.left && !tile.iswalked)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public bool GoalCheck()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(0.1f, 0.1f), 0f);

        if (hit != null && hit.CompareTag("Tile"))
        {
            if (hit.gameObject.name == "Goal(Clone)")
            {
                return true;
            }
        }
        return false;
    }

    TempleTile GetCurrentTile()
    {
        Vector3 currentPosition = transform.position;

        Collider2D hit = Physics2D.OverlapBox(currentPosition, new Vector2(0.1f, 0.1f), 0f);
        if (hit != null && hit.CompareTag("Tile"))
        {
            return hit.GetComponent<TempleTile>();
        }

        return null;
    }
}
