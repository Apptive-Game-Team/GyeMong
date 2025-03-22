using System.Collections;
using System.Event.Controller;
using Creature.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace System.Game.Portal
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

        public IEnumerator TransitScene(PortalID portalID)
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
            StartCoroutine(EffectManager.Instance.FadeIn());
        }
    
    }
}
