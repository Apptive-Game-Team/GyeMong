using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    bool isAttached = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = true;
        }
    }

    void Update()
    {
        if (isAttached)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Rotate();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = false;
        }
    }

    void Rotate()
    {
        Vector3 newRotation = gameObject.transform.eulerAngles;
        newRotation.z += 90f;
        gameObject.transform.eulerAngles = newRotation;
    }
}
