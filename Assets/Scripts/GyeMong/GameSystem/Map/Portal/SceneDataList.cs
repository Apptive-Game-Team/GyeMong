using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Portal
{
    public enum SceneID
    {
        NONE = 0,
    
        // Spring
        SPRING_BEACH = 1,
        SPRING_TOWN = 2,
        SPRING_NORTH_FOREST = 3,
        SPRING_SOUTH_ROAD = 4,
        SPRING_GARDEN = 5,
        SPRING_SHADOW_ISLAND = 6,
        SPRING_GOLEM_TEMPLE = 7,
        Cinematic = 8,
        SPRING_CLIFF = 9,
        
        // Summer
        SUMMER_OPENING = 50,
        NAGA_ROGUE = 51,
        WANDERER = 70,
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
}