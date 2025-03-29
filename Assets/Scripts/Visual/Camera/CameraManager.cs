using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using playerCharacter;
using DG.Tweening;

namespace Visual.Camera
{
    public class CameraManager : SingletonObject<CameraManager>
    {
        private List<CinemachineVirtualCamera> virtualCams;
        private CinemachineVirtualCamera currentCam;
        private float cameraSize;

        protected override void Awake() 
        {
            cameraSize = 4f;
            GetCameras();
        }

        private void GetCameras()
        {
            virtualCams = new List<CinemachineVirtualCamera>(FindObjectsOfType<CinemachineVirtualCamera>());
            foreach (var virtualCam in virtualCams)
            {
                Collider2D roomCollider = virtualCam.GetComponentInParent<Collider2D>();
                virtualCam.gameObject.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = roomCollider;
                virtualCam.Follow = PlayerCharacter.Instance.gameObject.transform;
                virtualCam.m_Lens.OrthographicSize = cameraSize;
                virtualCam.Priority = 0;
            }
        }

        public void ChangeCamera(CinemachineVirtualCamera newCam)
        {
            if (currentCam != null)
            {
                currentCam.Priority = 0;
            }
            newCam.Priority = 10;
            currentCam = newCam;
        }

        public IEnumerator CameraMove(Vector3 destination, float speed)
        {
            currentCam.Follow = null;

            yield return currentCam.transform
                .DOMove(destination, speed)
                .SetEase(Ease.OutQuad)
                .WaitForCompletion();
        }

        public void CameraFollow(Transform transform)
        {
            currentCam.Follow = transform;
        }

        public IEnumerator CameraZoomInOut(float size, float duration)
        {
            yield return DOTween.To(() => currentCam.m_Lens.OrthographicSize,
                x => currentCam.m_Lens.OrthographicSize = x, size, duration)
                .WaitForCompletion();
        }

        public void CameraZoomReset()
        {
            currentCam.m_Lens.OrthographicSize = cameraSize;
        }

        public void CameraShake(float force)
        {
            CinemachineImpulseSource impulseSource = currentCam.GetComponent<CinemachineImpulseSource>();
    
            for (int i = 0; i < 5; i++)
            {
                Vector3 randomShake = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
                impulseSource.GenerateImpulse(randomShake * force);
            }
        }
    }
}
