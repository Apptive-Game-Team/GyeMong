using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        playerCharacter.PlayerCharacter.Instance.transform.position = portalData.destination;
        yield return new WaitForSeconds(delay);
        StartCoroutine(EffectManager.Instance.FadeIn());
    }
    
}
