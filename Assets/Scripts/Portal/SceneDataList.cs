using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneID
{
    NONE = 0,
    SPRING_MAIN = 1
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