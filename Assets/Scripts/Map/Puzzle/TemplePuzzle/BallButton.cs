using Map.Puzzle.ImagePuzzle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map.Puzzle.TemplePuzzle
{
    public class BallButton : InteractableObject
    {
        [SerializeField] private Sprite buttonImage;
        protected override void OnInteraction(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !ConditionManager.Instance.Conditions.GetValueOrDefault("SpringTemplePuzzleIsCleared", false))
            {
                GetComponent<SpriteRenderer>().sprite = buttonImage;
                BallMovement ball = FindObjectOfType<BallMovement>();
                ball.StartMoveBall();
            }
        }
    }
}
