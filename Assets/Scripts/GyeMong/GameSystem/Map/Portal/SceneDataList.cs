
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Portal
{
    public enum SceneID
    {
        None = 0,
        Title = 13,
        StageSelect = 14,    
        Loading = 15,
        
        // Spring
        SpringBeach = 1,
        SpringTown = 2,
        SpringNorthForest = 3,
        SpringSouthRoad = 4,
        SpringShadowIsland = 6,
        SpringGolemTemple = 7,
        Cinematic = 8,
        SpringCliff = 9,
        
        // Summer
        Wanderer = 10,
        Sandworm = 11,
        NagaRouge = 12,
        NagaWarrior = 20, //Why Title ID is 13??
    }


    [Serializable]
    public class SceneData
    {
        public SceneID sceneID;
        public string sceneName;
        public string defaultBGM;
    }

    [CreateAssetMenu(fileName = "SceneDataList", menuName = "ScriptableObject/New SceneDataList")]
    public class SceneDataList : ScriptableObject
    {
        public List<SceneData> sceneDatas;
        
        public SceneData GetSceneDataByName(string name)
        {
            SceneData sceneData = sceneDatas.Find(x => x.sceneName.Equals(name));
            if (sceneData == null) Debug.LogErrorFormat("There's No Scene! Please Check SceneDataList!");
            return sceneData;
        }

        public SceneData GetSceneDataByID(SceneID id)
        {
            SceneData sceneData = sceneDatas.Find(x => x.sceneID == id);
            if (sceneData == null) Debug.LogErrorFormat("There's No Scene! Please Check SceneDataList!");
            return sceneData;
        }
    }
}