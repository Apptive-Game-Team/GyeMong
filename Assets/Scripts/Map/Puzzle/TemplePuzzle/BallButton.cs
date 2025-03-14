using Map.Puzzle.ImagePuzzle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallButton : InteractableObject
{
    protected override void OnInteraction(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BallMovement ball = FindObjectOfType<BallMovement>();
            ball.StartMoveBall();
        }
    }
}
