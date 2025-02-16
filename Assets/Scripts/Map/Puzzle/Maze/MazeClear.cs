using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeClear : MonoBehaviour
{
    private GameObject player;
    private bool isInMaze = true;
    public bool isMazeCleared = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (!ConditionManager.Instance.Conditions.ContainsKey("spring_puzzle1_clear"))
        {
            if (other.CompareTag("Player"))
            {
                bool previousState = isInMaze;
                isInMaze = player.transform.position.x > transform.position.x;

                if (previousState != isInMaze)
                {
                    isMazeCleared = true;
                    ConditionManager.Instance.Conditions.Add("spring_puzzle1_clear", isMazeCleared);
                    RuneObjectCreator.Instance.DrawRuneObject(1, new Vector2(-75, 80));
                }
            }
        }
    }
}
