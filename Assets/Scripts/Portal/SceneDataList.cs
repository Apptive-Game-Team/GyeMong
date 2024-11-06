using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneID
{
    NONE = 0,
    TEST = 1,
    SPRING_MAP = 2,
    SPRING_ADVENTURE_2 = 3,
    SPRING_BOSS_MAP = 4,
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

    [SerializeField]
    public List<SceneData> sceneDatas;

    public SceneData GetSceneDataByID(SceneID id)
    {
        SceneData sceneData = sceneDatas.Find(x => x.sceneID == id);
        if (sceneData == null) Debug.LogErrorFormat("There's No Scene! Please Check SceneDataList!");
        return sceneData;
    }
}