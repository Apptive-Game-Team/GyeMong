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
            LoadSceneMode(portalID);
            yield return new WaitWhile(() => SceneLoader.Instance.IsLoading);
            if (delay > 0) yield return new WaitForSecondsRealtime(delay);
            if (SceneContext.EffectManager != null) yield return SceneContext.EffectManager.FadeIn();
        }
        
        public void LoadSceneMode(PortalID portalID)
        {
            PortalData portalData = portalDataList.GetPortalDataByID(portalID);
            SceneData sceneData = sceneDataList.GetSceneDataByID(portalData.sceneID);
            if (!sceneData.sceneName.Equals(SceneManager.GetActiveScene().name))
            {
                sceneUnloading?.Invoke(SceneManager.GetActiveScene());
                SceneLoader.LoadScene(sceneData.sceneName);
            }
        }

    }
}
