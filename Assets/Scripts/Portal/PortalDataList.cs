using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalID
{
    NONE = 0,
    SPRING_MAIN_TO_BOSS = 1,
    SPRING_BOSS_TO_MAIN = 2,
    SPRING_MAIN_TO_PUZZLE2 = 3,
    SPRING_PUZZLE2_TO_MAIN = 4,
    TUTORIAL_TO_MAIN = 5,
    SPRING_MAIN_TO_MID_BOSS = 6,
    SPRING_MID_BOSS_TO_MAIN = 7,
    SPRING_TOWN = 8,
    MAIN_TO_TUTORIAL = 9,
    TUTORIAL_TO_LEFT1 = 10,
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

    public List<PortalData> portalDatas;

    //∞≠∞«º∫¿Ã ∂≥æÓ¡¸
    public PortalData GetPortalDataByID(PortalID id)
    {
        PortalData sceneData = portalDatas.Find(x => x.portalID == id);
        if (sceneData == null) Debug.LogErrorFormat("There's No Portal! Please Check PortalDataList!");
        return sceneData;
    }
}