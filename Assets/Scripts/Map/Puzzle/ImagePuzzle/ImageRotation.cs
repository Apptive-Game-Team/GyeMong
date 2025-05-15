using System.Collections;
using System.Input;
using UnityEngine;

namespace Map.Puzzle.ImagePuzzle
{
    public class ImageRotation : MonoBehaviour
    {
        private bool isAttached = false;
        private bool isRotating = true;
        public float rotationSpeed = 300f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isAttached = true;
            }
        }

        private void Update()
        {
            if (isAttached && isRotating && !ImagePuzzleController.Instance.isPuzzleCleared)
            {
                if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
                {
                    StartCoroutine(RotateImage());
                    ImagePuzzleController.Instance.isPuzzleStart = true;
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

        private IEnumerator RotateImage()
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
            isRotating = true;
        }
    }
}
