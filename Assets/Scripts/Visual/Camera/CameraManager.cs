using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using playerCharacter;

namespace Visual.Camera
{
    public class CameraManager : SingletonObject<CameraManager>
    {
        private List<CinemachineVirtualCamera> virtualCams;
        private CinemachineVirtualCamera currentCam;

        protected override void Awake() 
        {
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
    }
}
