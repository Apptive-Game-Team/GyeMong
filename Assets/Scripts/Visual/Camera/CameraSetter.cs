using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Visual.Camera
{
    public class CameraSetter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneContext.CameraManager.ChangeCamera(transform.GetComponentInChildren<CinemachineVirtualCamera>());
            }
        }
    }
}

