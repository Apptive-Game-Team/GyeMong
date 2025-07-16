using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Portal
{
    public enum PortalID
    {
        None = 0,
        
        // Spring
        SpringNorthForest = 6,
        SpringBeach = 10,
        SpringShadowIsland = 16,
        SpringGolemTemple = 22,
        Cinematic = 24,
        SpringCliff = 25,
        
        // Summer
        Wanderer = 26,
        Sandworm = 27,
        NagaRouge = 28,
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
}