using Creature.Player.Component;
using playerCharacter;
using UnityEngine;

namespace System.Game.Buff
{
public abstract class BuffData : ScriptableObject
{
    public float duration;
    public bool isPermanent;
    public int maxStack;
    
    public abstract void ApplyEffect(IBuffable target);
    public abstract void RemoveEffect(IBuffable target);
}

}
