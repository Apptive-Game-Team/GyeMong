using System;
using System.Collections;
using System.Collections.Generic;
using Gyemong.EventSystem.Controller;
using Gyemong.GameSystem.Creature.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Gyemong.GameSystem.Map.Portal
{
    public class PortalManager : SingletonObject<PortalManager>
    {
        [SerializeField] SceneDataList sceneDataList;
        [SerializeField] PortalDataList portalDataList;

        public static event Action<Scene> sceneUnloading;
        
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        public IEnumerator TransitScene(PortalID portalID, float delay = 0f)
        {
            yield return EffectManager.Instance.FadeOut();
            PortalData portalData = portalDataList.GetPortalDataByID(portalID);
            SceneData sceneData = sceneDataList.GetSceneDataByID(portalData.sceneID);
            if (!sceneData.sceneName.Equals(SceneManager.GetActiveScene().name))
            {
                sceneUnloading?.Invoke(SceneManager.GetActiveScene());
                SceneManager.LoadScene(sceneData.sceneName);
            }
            PlayerCharacter.Instance.transform.position = portalData.destination;
            StartCoroutine(DelayedFadeIn(delay));
        }

        private IEnumerator DelayedFadeIn(float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return EffectManager.Instance.FadeIn();
        }
    }
}
