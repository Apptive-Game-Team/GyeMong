using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneID
{
    NONE = 0,
    SPRING_MAIN = 1,
    SPRING_BOSS = 2,
    SPRING_PUZZLE2 = 3,
    SPRING_MID_BOSS = 4,
    TUTORIAL = 5,
    AboveRoad1 = 6,
    AboveRoad2 = 7,
    AboveRoad3 = 8,
    BelowRoad1 = 9,
    BelowRoad2 = 10,
    LeftRoad1 = 11,
    LeftRoad2 = 12,
    RightRoad1 = 13,
    RightRoad2 = 14,
    ReTutorialScene = 15,
    ReSpringGuardian = 16,
    ReSpringMidBoss = 17,
    ReSpringMain = 18,
    Maze = 19,
}


[Serializable]
public class SceneData
{
    public SceneID sceneID;
    public string sceneName;

}

[CreateAssetMenu(fileName = "SceneDataList", menuName = "ScriptableObject/New SceneDataList")]
public class SceneDataList : ScriptableObject
{
    public List<SceneData> sceneDatas;

    public SceneData GetSceneDataByID(SceneID id)
    {
        SceneData sceneData = sceneDatas.Find(x => x.sceneID == id);
        if (sceneData == null) Debug.LogErrorFormat("There's No Scene! Please Check SceneDataList!");
        return sceneData;
    }
}