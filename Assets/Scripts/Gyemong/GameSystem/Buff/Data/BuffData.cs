using UnityEngine;

namespace Gyemong.GameSystem.Buff.Data
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
