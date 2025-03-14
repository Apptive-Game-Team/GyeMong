using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleTile : InteractableObject
{
    public TempleTileData templeTileData;

    private bool isRotating = false;
    public bool iswalked = false;

    private float rotationAmount = -90f;
    private float rotationSpeed = 0.5f;
    
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

    protected override void OnInteraction(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isRotating)
        {
            StartCoroutine(RotateTile());
        }
    }

    public IEnumerator RotateTile()
    {
        isRotating = true;

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

        Rotate();
    }

    void Rotate()
    {
        bool temp;

        temp = up;
        up = left;
        left = down;
        down = right;
        right = temp;

        isRotating = false;
    }
}
