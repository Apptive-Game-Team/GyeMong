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

            DisableDuplicateScripts();
        }
    }

    private void DisableDuplicateScripts()
    {
        EventObject[] scripts = Door.GetComponents<EventObject>();

        scripts[0].enabled = true;
        scripts[1].enabled = false;
    }

    public void UnlockDoor()
    {
        EventObject[] scripts = Door.GetComponents<EventObject>();

        scripts[0].enabled = false;
        scripts[1].enabled = true;
    }
}
