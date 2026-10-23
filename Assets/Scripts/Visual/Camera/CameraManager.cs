using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;
using GyeMong.GameSystem.Creature.Player.Component;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Visual.Camera
{
    public class CameraManager : MonoBehaviour
    {
        private List<CinemachineVirtualCamera> virtualCams;
        private CinemachineVirtualCamera currentCam;
        private float cameraSize;
        private Volume mainCamVolume;
        private VolumeProfile mainVolumeProfile;
        private ColorAdjustments ca;
        private ColorAdjustments cloneCa;
        private bool initialCameraPrepared;
        private VolumeProfile ownedProfile;

        protected void Awake() 
        {
            cameraSize = 7f;
            GetCameras();
            mainVolumeProfile = Resources.Load<VolumeProfile>("CameraProfile/MainSetting");
            mainCamVolume = UnityEngine.Camera.main != null ? UnityEngine.Camera.main.GetComponent<Volume>() : null;
            if (mainCamVolume == null || mainVolumeProfile == null) return;
            mainCamVolume.profile = ownedProfile = Instantiate(mainVolumeProfile);
            for (int i = 0; i < mainCamVolume.profile.components.Count; i++)
            {
                var component = Instantiate(mainCamVolume.profile.components[i]);
                mainCamVolume.profile.components[i] = component;
            }
            PlayerChangeListenerCaller.OnPlayerDied += SetVolumeGray;
        }

        private void OnDestroy()
        {
            PlayerChangeListenerCaller.OnPlayerDied -= SetVolumeGray;
            if (ownedProfile != null)
            {
                foreach (var component in ownedProfile.components) Destroy(component);
                Destroy(ownedProfile);
            }
        }

        private void GetCameras()
        {
            virtualCams = new List<CinemachineVirtualCamera>(FindObjectsOfType<CinemachineVirtualCamera>());
            foreach (var virtualCam in virtualCams)
            {
                Collider2D roomCollider = virtualCam.GetComponentInParent<Collider2D>();
                var confiner = virtualCam.GetComponent<CinemachineConfiner2D>();
                if (confiner != null) confiner.m_BoundingShape2D = roomCollider;
                virtualCam.Follow = SceneContext.Character?.gameObject.transform;
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

        public void PrepareInitialCamera()
        {
            if (initialCameraPrepared) return;
            var player = SceneContext.Character;
            if (player == null || virtualCams.Count == 0) return;
            if (currentCam == null)
            {
                var selected = virtualCams.Find(camera =>
                {
                    var room = camera.GetComponentInParent<Collider2D>();
                    return room != null && room.OverlapPoint(player.transform.position);
                });
                ChangeCamera(selected != null ? selected : virtualCams[0]);
            }
            currentCam.PreviousStateIsValid = false;
            currentCam.UpdateCameraState(Vector3.up, -1f);
            var camera = UnityEngine.Camera.main;
            if (camera != null && camera.TryGetComponent<CinemachineBrain>(out var brain)) brain.ManualUpdate();
            initialCameraPrepared = true;
        }

        public IEnumerator CameraMove(Vector3 destination, float speed)
        {
            yield return new WaitUntil(() => currentCam != null);
            
            currentCam.Follow = null;

            yield return currentCam.transform
                .DOMove(destination, speed)
                .SetLink(gameObject)
                .SetEase(Ease.OutQuad)
                .OnUpdate(() =>
                {
                    currentCam.gameObject.GetComponent<CinemachineConfiner2D>().InvalidateCache();
                })
                .WaitForCompletion();
        }

        public void SetVolumeGray()
        {
            StartCoroutine(FadeSaturation(-50f, 2f));
        }
        
        private IEnumerator FadeSaturation(float targetValue, float duration)
        {
            if (!mainCamVolume.profile.TryGet<ColorAdjustments>(out var ca))
                yield break;

            float initialValue = ca.saturation.value;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                ca.saturation.value = Mathf.Lerp(initialValue, targetValue, elapsed / duration);
                yield return null;
            }

            ca.saturation.value = targetValue;
        }
        
        public void CameraFollow(Transform transform)
        {
            currentCam.Follow = transform;
        }

        public IEnumerator CameraZoomInOut(float size, float duration)
        {
            yield return DOTween.To(() => currentCam.m_Lens.OrthographicSize,
                x =>
                {
                    currentCam.gameObject.GetComponent<CinemachineConfiner2D>().InvalidateCache();
                    currentCam.m_Lens.OrthographicSize = x;
                }, size, duration)
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
