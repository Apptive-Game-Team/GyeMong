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

        private float delayTime = 0.5f;
        private float goalDelay = 1f;

        public Animator animator;
        [SerializeField] private Sprite buttonImage;
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
                
                var button = FindObjectOfType<BallButton>().gameObject;
                button.GetComponent<SpriteRenderer>().sprite = buttonImage;

                if (GoalCheck())
                {
                    Debug.Log("Success");

                    StartCoroutine(GoalAnimation());

                    ConditionManager.Instance.Conditions["SpringTemplePuzzleIsCleared"] = true;
                }
                else
                {
                    Debug.Log("Nope");
                    StartCoroutine(ReturnToStartPosition());
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
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.1f, 0.1f), 0f);

            foreach (Collider2D hit in hits)
            {
                if (hit != null && hit.CompareTag("Tile"))
                {
                    if (hit.gameObject.name == "Goal(Clone)")
                    {
                        return true;
                    }
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

        private IEnumerator ReturnToStartPosition()
        {
            yield return new WaitForSeconds(delayTime);

            float fadeDuration = 1f;
            float elapsedTime = 0f;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Color initialColor = spriteRenderer.color;

            Vector3 originalPosition = transform.position;

            while (elapsedTime < fadeDuration)
            {
                float shakeAmount = Mathf.Sin(Time.time * 20f) * 0.1f;
                transform.position = originalPosition + new Vector3(shakeAmount, 0, 0);

                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

            yield return new WaitForSeconds(delayTime);

            spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
            transform.position = startPosition + new Vector3(0, 5f, 0);

            StartCoroutine(DownBall());

            foreach (TempleTile tile in visitedTiles)
            {
                tile.iswalked = false;
            }
        }

        IEnumerator DownBall()
        {
            float downDuration = 1f;
            float elapsedTime = 0f;

            Vector3 startPos = transform.position;
            Vector3 endPos = startPosition;

            while (elapsedTime < downDuration)
            {
                float t = elapsedTime / downDuration;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                transform.position = Vector3.Lerp(startPos, endPos, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;

            visitedTiles.Clear();

            currentTile = GetCurrentTile();

            yield return null;
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
