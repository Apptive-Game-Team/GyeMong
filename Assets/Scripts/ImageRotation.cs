using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    bool isAttached = false;
    bool isRotating = true;
    public float rotationSpeed = 300f;
    public float cooldownTime = 1f;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = true;
        }
    }

    void Update()
    {
        if (isAttached && isRotating)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(RotateImage());
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
        newRotation.z -= 90f;
        gameObject.transform.eulerAngles = newRotation;
    }

    IEnumerator RotateImage()
    {
        isRotating = false;
        float targetAngle = transform.eulerAngles.z - 90f;
        float currentAngle = transform.eulerAngles.z;

        while (Mathf.Abs(targetAngle - currentAngle) > 0.1f)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            Vector3 newRotation = new Vector3(0, 0, currentAngle);
            transform.eulerAngles = newRotation;
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, targetAngle);

        yield return new WaitForSeconds(cooldownTime);
        isRotating = true;
    }
}
