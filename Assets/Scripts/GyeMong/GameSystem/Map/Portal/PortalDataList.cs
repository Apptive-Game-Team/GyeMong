using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Portal
{
    public enum PortalID
    {
        NONE = 0,
    
        // Beach
        SPRING_BEACH1 = 1,
        SPRING_BEACH2 = 2,
        SPRING_BEACH3 = 3,
    
        // Town
        SPRING_TOWN_LEFT = 4,
        SPRING_TOWN_RIGHT = 5,
        SPRING_TOWN_UP = 6,
        SPRING_TOWN_DOWN = 7,
    
        // North Forest
        SPRING_NORTH_FOREST = 8,
    
        // South Road
        SPRING_SOUTH_ROAD_UP = 9,
        SPRING_SOUTH_ROAD_LEFT_ = 10,
        SPRING_SOUTH_ROAD_DOWN = 11,
    
        // Garden
        SPRING_GARDEN = 12,
    
        // Shadow Island
        SPRING_SHADOW_ISLAND1 = 13,
        SPRING_SHADOW_ISLAND2 = 14,
        SPRING_SHADOW_ISLAND3 = 15,
        SPRING_SHADOW_ISLAND4 = 16,
        SPRING_SHADOW_ISLAND5 = 17,
    
        // Golem Temple
        SPRING_GOLEM_TEMPLE1 = 18,
        SPRING_GOLEM_TEMPLE2 = 19,
        SPRING_GOLEM_TEMPLE3 = 20,
        SPRING_GOLEM_TEMPLE4 = 21,
        SPRING_GOLEM_TEMPLE5 = 22,
        SPRING_GOLEM_TEMPLE6 = 23,

        //Cinimatic
        Cinematic = 24,
    
        // Cliff
        SPRING_CLIFF1 = 25,
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