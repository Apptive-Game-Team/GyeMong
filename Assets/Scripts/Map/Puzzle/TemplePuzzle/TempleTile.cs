using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleTile : MonoBehaviour
{
    public TempleTileData templeTileData;
    private bool isAttached = false;
    private bool isRotating = false;
    public bool iswalked = false;

    public bool up;
    public bool down;
    public bool left;
    public bool right;

    private void Start()
    {
        up = templeTileData.up;
        down = templeTileData.down;
        left = templeTileData.left;
        right = templeTileData.right;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isAttached = true;
        }
    }

    private void Update()
    {
        if (isAttached && !isRotating)
        {
            if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
            {
                StartCoroutine(RotateTile());
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

    public IEnumerator RotateTile()
    {
        isRotating = true;

        float rotationAmount = -90f;
        float rotationSpeed = 1f;
        float time = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + rotationAmount);

        while (time < rotationSpeed)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / rotationSpeed);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRotation;

        UpdateDirection();
    }

    void UpdateDirection()
    {
        bool temp;

        temp = up;
        up = left;
        left = down;
        down = right;
        right = temp;

        Debug.Log("Up: " + up);
        Debug.Log("Down: " + down);
        Debug.Log("Left: " + left);
        Debug.Log("Right: " + right);

        isRotating = false;
    }
}
