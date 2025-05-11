using System.Collections;
using Gyemong.EventSystem.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Gyemong.EventSystem.Event.Scene
{
    public class SceneEvent : MonoBehaviour
    {
        public Image fadeImage;
        public Camera mainCamera;
        public Transform cameraTargetPosition;
        public Vector3 originalCameraPosition;

        private float waitCameraMoveDuration = 3.0f;
        public CameraController cameraController;
        public EventObject chatEventObject;

        private void Start()
        {    
            originalCameraPosition = mainCamera.transform.position;
            cameraController = mainCamera.GetComponent<CameraController>();
            StartCoroutine(SceneTransition());
        }

        private IEnumerator SceneTransition()
        {
            yield return StartCoroutine(FadeIn());

            yield return StartCoroutine(MoveCamera(cameraTargetPosition.position, 3f));

            StopCameraFollow();

            Debug.Log("Wait..");
            yield return new WaitForSeconds(waitCameraMoveDuration);
            Debug.Log("Finish");

            yield return StartCoroutine(MoveCamera(originalCameraPosition, 2f));

            StopCameraFollow();

            Debug.Log("Wait..");
            yield return new WaitForSeconds(waitCameraMoveDuration);
            Debug.Log("Finish");

            ResumeCameraFollow();

            chatEventObject.Trigger();
        }

        private IEnumerator FadeIn()
        {
            Debug.Log("FadeIn Start");

            float duration = 3f;
            float elapsedTime = 0f;

            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                fadeImage.color = color;
                yield return null;
            }
            Debug.Log("FadeIn End");

            color.a = 0f;
            fadeImage.color = color;
        }

        private IEnumerator MoveCamera(Vector3 targetPos, float duration)
        {
            Debug.Log("Camera Move Start");

            float elapsedTime = 0f;
            Vector3 startPos = mainCamera.transform.position;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
                yield return null;
            }

            mainCamera.transform.position = targetPos;

            Debug.Log("Camera Move End");
        }

        private void StopCameraFollow()
        {
            cameraController.enabled = false;
        }

        private void ResumeCameraFollow()
        {
            cameraController.enabled = true;
        }
    }
}
