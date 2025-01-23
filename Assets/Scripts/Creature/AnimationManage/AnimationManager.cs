using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationManager : SingletonObject<AnimationManager>
{
    [SerializeField] AnimationDataList animationDataList;
    private Dictionary<GameObject, AnimationClip> currentPlayingAnimations = new Dictionary<GameObject, AnimationClip>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public AnimationClip TakeAnimation(CreatureType creatureType, string creatureName, string animationName, DirectionType directionType)
    {
        // CreatureTypeData 검색
        var creatureTypeData = animationDataList.creatureTypeData.Find(ct => ct.Type == creatureType);
        
        // CreatureData 검색
        var creatureData = creatureTypeData.creatureData.Find(cd => cd.creatureName == creatureName);

        // AnimationList 검색
        var animationList = creatureData.animationData.Find(al => al.animationName == animationName);
       
        // AnimationData 검색
        var animationData = animationList.animationData.Find(ad => ad.Direction == directionType);

        return animationData.animationClip;
    }

    public void PlayAnimation(GameObject target, CreatureType creatureType, string creatureName, string animationName, DirectionType directionType)
    {
        var animationClip = TakeAnimation(creatureType, creatureName, animationName, directionType);
        if (animationClip == null)
        {
            return;
        }
        if (currentPlayingAnimations.ContainsKey(target) && currentPlayingAnimations[target] == animationClip)
        {
            return;
        }
        var animationComponent = target.GetComponent<Animation>();
        if (animationComponent == null)
        {
            animationComponent = target.AddComponent<Animation>();
        }
        animationComponent.Stop();
        animationComponent.clip = animationClip;
        animationComponent.Play();
        currentPlayingAnimations[target] = animationClip;
    }
}
