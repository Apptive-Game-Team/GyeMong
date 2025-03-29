using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map.Puzzle.TemplePuzzle
{
    public class BallMovement : MonoBehaviour
    {
        public float moveSpeed = 2f;
        public TempleTile currentTile;
        public bool isMoving = false;
        private Vector3 startPosition;
        private List<TempleTile> visitedTiles = new List<TempleTile>();

        private float goalDelay = 1f;

        public Animator animator;

        void Start()
        {
            startPosition = transform.position;
            currentTile = GetCurrentTile();
            animator = GetComponent<Animator>();
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
                    animator.SetBool("isMove", true);
                    animator.SetFloat("xDir", direction.x);
                    animator.SetFloat("yDir", direction.y);
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
                        visitedTiles.Add(currentTile);
                    }

                    currentTile = GetCurrentTile();

                    direction = GetMoveDirection();
                }

                isMoving = false;
                animator.SetBool("isMove", false);
                animator.SetFloat("xDir", 0);
                animator.SetFloat("yDir", 0);

                if (GoalCheck())
                {
                    Debug.Log("Success");

                    StartCoroutine(GoalAnimation());

                    ConditionManager.Instance.Conditions["SpringTemplePuzzleIsCleared"] = true;
                }

                else
                {
                    Debug.Log("Nope");
                    ReturnToStartPosition();
                }
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

            Collider2D[] hits = Physics2D.OverlapBoxAll(checkPosition, new Vector2(0.1f, 0.1f), 0f);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Tile"))
                {
                    TempleTile tile = hit.GetComponent<TempleTile>();
                    if (tile != null && !tile.iswalked)
                    {
                        if ((direction == Vector2.up && tile.down) ||
                            (direction == Vector2.down && tile.up) ||
                            (direction == Vector2.left && tile.right) ||
                            (direction == Vector2.right && tile.left))
                        {
                            return true;
                        }
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

        private IEnumerator GoalAnimation()
        {
            yield return new WaitForSeconds(goalDelay);

            float shrinkDuration = 1f;
            float elapsedTime = 0f;
            Vector3 initialScale = transform.localScale;
            Vector3 targetScale = Vector3.zero;

            while (elapsedTime < shrinkDuration)
            {
                transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / shrinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetScale;
            gameObject.SetActive(false);

            yield return new WaitForSeconds(goalDelay);
        }

        void ReturnToStartPosition()
        {
            transform.position = startPosition;
            currentTile = GetCurrentTile();

            foreach (TempleTile tile in visitedTiles)
            {
                tile.iswalked = false;
            }

            visitedTiles.Clear();
        }

        TempleTile GetCurrentTile()
        {
            Vector3 currentPosition = transform.position;

            Collider2D[] hits = Physics2D.OverlapBoxAll(currentPosition, new Vector2(0.1f, 0.1f), 0f);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Tile"))
                {
                    return hit.GetComponent<TempleTile>();
                }
            }

            return null;
        }
    }
}
