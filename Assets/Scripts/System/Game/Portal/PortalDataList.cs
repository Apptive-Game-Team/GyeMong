using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalID
{
    NONE = 0,
    SPRING_BEACH_TO_LOAD = 1,
    SPRING_LOAD_TO_BEACH = 2,
    // SPRING_MAIN_TO_BOSS = 1,
    // SPRING_BOSS_TO_MAIN = 2,
    // SPRING_MAIN_TO_PUZZLE2 = 3,
    // SPRING_PUZZLE2_TO_MAIN = 4,
    // TUTORIAL_TO_MAIN = 5,
    // SPRING_MAIN_TO_MID_BOSS = 6,
    // SPRING_MID_BOSS_TO_MAIN = 7,
    // SPRING_TOWN = 8,
    // MAIN_TO_TUTORIAL = 9,
    // TUTORIAL_TO_LEFT1 = 10,
    // LEFT1_TO_TUTORIAL = 11,
    // LEFT1_TO_LEFT2 = 12,
    // LEFT2_TO_LEFT1 = 13,
    // LEFT2_TO_MAIN = 14,
    // MAIN_TO_LEFT2 = 15,
    // MAIN_TO_BELOW1 = 16,
    // BELOW1_TO_MAIN = 17,
    // BELOW2_TO_BELOW1 = 18,
    // BELOW1_TO_BELOW2 = 19,
    // MAIN_TO_ABOVE1 = 20,
    // ABOVE1_TO_MAIN = 21,
    // ABOVE2_TO_ABOVE1 = 22,
    // ABOVE1_TO_ABOVE2 = 23,
    // ABOVE2_TO_ABOVE3 = 24,
    // ABOVE3_TO_ABOVE2 = 25,
    // ABOVE2_TO_MIDBOSS = 26,
    // MIDBOSS_TO_ABOVE2 = 27,
    // MAIN_TO_RIGHT1 = 28,
    // RIGHT1_TO_MAIN = 29,
    // RIGHT2_TO_RIGHT1 = 30,
    // RIGHT1_TO_RIGHT2 = 31,
    // RIGHT2_TO_GUARDIAN = 32,
    // GUARDIAN_TO_RIGHT2 = 33,
    // ABOVE3_TO_MAZE = 34,
    // MAZE_TO_ABOVE3 = 35,
    // BELOW2_TO_SPRINGPUZZLE2 = 36,
    // SPRINGPUZZLE2_TO_BELOW2 = 37
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