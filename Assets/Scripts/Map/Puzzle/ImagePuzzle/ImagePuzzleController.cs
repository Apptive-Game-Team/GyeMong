using System.Collections.Generic;
using System.Event.Controller.Condition;
using UnityEngine;
using Util.Interface;

namespace Map.Puzzle.ImagePuzzle
{
    public class ImagePuzzleController : MonoBehaviour
    {
        public static ImagePuzzleController Instance;
        private List<GameObject> puzzleImages = new();
        public bool isPuzzleStart = false;
        public bool isPuzzleCleared = false;
        [SerializeField] GameObject rune;
    
        private Timer timer;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            LoadClearFlag();

            if (rune == null || rune.activeSelf) isPuzzleCleared = true;

            timer = FindFirstObjectByType<Timer>();

            if (!isPuzzleCleared)
            {
                SetUpImage();
                SetUpInitialRotation();
            }
        }

        private void Update()
        {
            if (!isPuzzleCleared && isPuzzleStart && !timer.IsTimerRunning)
            {
                timer.Play(30f, new TimeEndCallback(this));
            }

            if (CheckClear() && !isPuzzleCleared)
            {
                // rune.SetActive(true);
                timer.Stop();
                isPuzzleCleared = true;
                SaveClearFlag();
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

        private void SaveClearFlag()
        {
            ConditionManager.Instance.Conditions.Add("spring_puzzle3_clear", isPuzzleCleared);
        }

        private void LoadClearFlag()
        {
            isPuzzleCleared = ConditionManager.Instance.Conditions.ContainsKey("spring_puzzle3_clear");
        }

        private class TimeEndCallback : ICallback
        {
            private ImagePuzzleController _puzzleController;

            public TimeEndCallback(ImagePuzzleController puzzleController)
            {
                _puzzleController = puzzleController;
            }
            public void OnProcessCompleted()
            {
                _puzzleController.SetUpInitialRotation();
                _puzzleController.isPuzzleStart = false;
            }
        }
    }
}
