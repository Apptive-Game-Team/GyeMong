using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] AnimationDataList animationDataList;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public Animation TakeAnimation(CreatureType creatureType, string creatureName, string animationName, DirectionType directionType)
    {
        // CreatureTypeData 검색
        var creatureTypeData = animationDataList.creatureTypeData.Find(ct => ct.Type == creatureType);
        
        // CreatureData 검색
        var creatureData = creatureTypeData.creatureData.Find(cd => cd.creatureName == creatureName);

        // AnimationList 검색
        var animationList = creatureData.animationData.Find(al => al.animationName == animationName);
       
        // AnimationData 검색
        var animationData = animationList.animationData.Find(ad => ad.Direction == directionType);

        return animationData.animation;
    }
}
