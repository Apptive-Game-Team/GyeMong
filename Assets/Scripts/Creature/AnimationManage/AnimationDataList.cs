using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreatureType
{
    NONE = 0,
    PLAYER = 1,
    NPC = 2,
    BOSS = 3,
    FiELDMONSTER = 4
}
public enum DirectionType
{
    IDLE = 0,
    FRONT = 1,
    BAKC = 2,
    LEFT = 3,
    RIGHT = 4
}

[Serializable]
public class CreatureTypeData
{
    public string Name;
    public CreatureType Type;
    public List<CreatureData> creatureData;
}
[Serializable]
public class CreatureData
{
    public string creatureName;
    public List<AnimationList> animationData;
}
[Serializable]
public class AnimationList
{
    public string animationName;
    public List<AnimationData> animationData;
}
[Serializable]
public class AnimationData
{
    public DirectionType Direction;
    public Animation animation;
}

[CreateAssetMenu(fileName = "AnimationDataList", menuName = "ScriptableObject/new AnimationDataList")]
public class AnimationDataList : ScriptableObject
{
    public List<CreatureTypeData> creatureTypeData;
}