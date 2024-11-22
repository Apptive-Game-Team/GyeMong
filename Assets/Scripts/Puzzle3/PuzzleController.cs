using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleController : MonoBehaviour
{
    public static PuzzleController Instance;
    private List<GameObject> puzzleImages = new();
    public bool isPuzzleStart = false;
    public bool isPuzzleCleared = false;
    [SerializeField] GameObject rune;
    private Image timeWatchImage;
    private TextMeshProUGUI timeWatch;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (rune == null || rune.activeSelf) isPuzzleCleared = true;

        Transform canvas = transform.parent.Find("TimeWatch");
        timeWatchImage = canvas.GetChild(0).GetComponent<Image>();
        timeWatch = canvas.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (!isPuzzleCleared)
        {
            SetUpImage();
            SetUpInitialRotation();
        }
    }

    private void Update()
    {
        if (!isPuzzleCleared && isPuzzleStart && !timeWatchImage.gameObject.activeSelf)
        {
            StartCoroutine(StartTimeWatchCoroutine());
        }

        if (CheckClear() && !isPuzzleCleared)
        {
            rune.SetActive(true);
            isPuzzleCleared = true;
        }
    }

    private void SetUpImage()
    {
        foreach (Transform image in transform)
        {
            puzzleImages.Add(image.gameObject);
        }
    }

    private void SetUpInitialRotation()
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

    private bool CheckClear()
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

    private IEnumerator StartTimeWatchCoroutine()
    {
        timeWatchImage.gameObject.SetActive(true);
        float remainTime = 30f;

        while (remainTime > 0)
        {
            if (CheckClear())
            {
                timeWatchImage.gameObject.SetActive(false);
                yield break;
            }

            remainTime -= Time.deltaTime;
            timeWatch.text = remainTime.ToString("F2");
            yield return null;
        }

        timeWatchImage.gameObject.SetActive(false);
        SetUpInitialRotation();
        isPuzzleStart = false;
    }
}
