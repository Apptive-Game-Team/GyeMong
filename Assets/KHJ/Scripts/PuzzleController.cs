using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public List<GameObject> puzzleImages = new();
    [SerializeField] GameObject rune;

    void Awake()
    {
        SetUpImage();
        SetUpInitialRotation();
    }

    void Update()
    {
        if (CheckClear())
        {
            rune.SetActive(true);
        }
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

        do
        {
            foreach (GameObject image in puzzleImages)
            {
                float randomRotation = rotations[Random.Range(0, rotations.Length)];
                image.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
            }
        } while (CheckClear());
    }

    bool CheckClear()
    {
        foreach (GameObject image in puzzleImages)
        {
            if (image.transform.eulerAngles.z != 0f)
            {
                return false;
            }
        }
        return true;
    }
}
