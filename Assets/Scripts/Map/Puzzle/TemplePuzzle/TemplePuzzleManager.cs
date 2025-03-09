using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplePuzzleManager : MonoBehaviour
{
    public static TemplePuzzleManager instance;

    public GameObject Door;

    public bool isCleared = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
