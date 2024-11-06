using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PortalManager : SingletonObject<PortalManager>
{
    [SerializeField] SceneDataList sceneDataList;
    [SerializeField] PortalDataList portalDataList;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void TransitScene(PortalID portalID)
    {
        PortalData portalData = portalDataList.GetPortalDataByID(portalID);
        SceneData sceneData = sceneDataList.GetSceneDataByID(portalData.sceneID);
        SceneManager.LoadScene(sceneData.sceneName);
    }

}
