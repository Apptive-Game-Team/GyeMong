using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalManager : SingletonObject<PortalManager>
{
    [SerializeField] SceneDataList sceneDataList;
    [SerializeField] PortalDataList portalDataList;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public IEnumerator TransitScene(PortalID portalID)
    {
        yield return EffectManager.Instance.FadeOut();
        PortalData portalData = portalDataList.GetPortalDataByID(portalID);
        SceneData sceneData = sceneDataList.GetSceneDataByID(portalData.sceneID);
        SceneManager.LoadScene(sceneData.sceneName);
        playerCharacter.PlayerCharacter.Instance.transform.position = portalData.destination;
        StartCoroutine(EffectManager.Instance.FadeIn());
    }
}
