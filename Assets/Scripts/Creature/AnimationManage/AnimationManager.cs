using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationManager : SingletonObject<AnimationManager>
{
    [SerializeField] AnimationDataList animationDataList;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public Animation TakeAnimation(CreatureType creatureType, string creatureName, string animationName, DirectionType directionType)
    {
        // CreatureTypeData �˻�
        var creatureTypeData = animationDataList.creatureTypeData.Find(ct => ct.Type == creatureType);
        
        // CreatureData �˻�
        var creatureData = creatureTypeData.creatureData.Find(cd => cd.creatureName == creatureName);

        // AnimationList �˻�
        var animationList = creatureData.animationData.Find(al => al.animationName == animationName);
       
        // AnimationData �˻�
        var animationData = animationList.animationData.Find(ad => ad.Direction == directionType);

        return animationData.animation;
    }
}
