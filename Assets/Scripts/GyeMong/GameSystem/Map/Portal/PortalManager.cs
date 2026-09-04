using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GyeMong.GameSystem.Map.Portal
{
    public class PortalManager : SingletonObject<PortalManager>
    {
        [SerializeField] public SceneDataList sceneDataList;
        [SerializeField] PortalDataList portalDataList;

        public static event Action<Scene> sceneUnloading;
        
        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        public IEnumerator TransitScene(PortalID portalID, float delay = 0f)
        {
            yield return SceneContext.EffectManager.FadeOut();
            yield return LoadSceneRoutine(portalID);
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            // 새 씬의 EffectManager 는 black 알파가 0인 새 인스턴스다.
            // FadeIn 은 0에서 0으로 가서 아무것도 가리지 못하므로 FadeInFirst 를 쓴다.
            yield return SceneContext.EffectManager.FadeInFirst();
        }

        public void LoadSceneMode(PortalID portalID)
        {
            StartCoroutine(LoadSceneRoutine(portalID));
        }

        private IEnumerator LoadSceneRoutine(PortalID portalID)
        {
            PortalData portalData = portalDataList.GetPortalDataByID(portalID);
            SceneData sceneData = sceneDataList.GetSceneDataByID(portalData.sceneID);
            if (sceneData.sceneName.Equals(SceneManager.GetActiveScene().name))
            {
                yield break;
            }
            sceneUnloading?.Invoke(SceneManager.GetActiveScene());
            yield return SceneLoader.LoadSceneRoutine(sceneData.sceneName);
        }
    }
}
