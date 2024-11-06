using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalID
{
    NONE = 0,
    TEST = 1,
    SPRING_MAP_TO_ADVENTURE_2 = 2,
}

[Serializable]
public class PortalData
{
    public PortalID portalID;
    public SceneID sceneID;
    public Vector3 destination;
}

[CreateAssetMenu(fileName = "PortalDataList", menuName = "ScriptableObject/New PortalDataList")]
public class PortalDataList : ScriptableObject
{
    [SerializeField]
    public List<PortalData> portalDatas;


    public PortalData GetPortalDataByID(PortalID id)
    {
        PortalData sceneData = portalDatas.Find(x => x.portalID == id);
        if (sceneData == null) Debug.LogErrorFormat("There's No Portal! Please Check PortalDataList!");
        return sceneData;
    }
}