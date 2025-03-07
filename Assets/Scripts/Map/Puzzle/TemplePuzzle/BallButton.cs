using Map.Puzzle.ImagePuzzle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallButton : MonoBehaviour
{
    private bool isAttached = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = true;
        }
    }

    private void Update()
    {
        if (isAttached)
        {
            if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
            {
                BallMovement ball = FindObjectOfType<BallMovement>();
                ball.StartMoveBall();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = false;
        }
    }
}
