using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleImageController : MonoBehaviour
{
    public List<GameObject> puzzleImages = new();

    void Start()
    {
        SetUpImage();
        SetUpInitialRotation();
    }

    void SetUpImage()
    {
        foreach (Transform image in transform)
        {
            puzzleImages.Add(image.gameObject);
        }
    }

    void SetUpInitialRotation()
    {
        float[] rotations = { 0f, 90f, 180f, 270f };
        foreach (GameObject image in puzzleImages)
        {
            float randomRotation = rotations[Random.Range(0, rotations.Length)];
            image.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
        }
    }
}
