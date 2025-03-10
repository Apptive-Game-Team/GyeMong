using UnityEngine;

namespace System.Game.Buff
{
    public class Buff
    {
        public BuffData buffData;
        public float remainingTime;
        public int curStack;

        public IBuffable target;
    
        public bool CanStack => curStack > buffData.maxStack;
    
        public Buff(BuffData data)
        {
            buffData = data;
            remainingTime = data.duration;
        }

        public void UpdateBuff(float deltaTime)
        {
            if (!buffData.isPermanent)
            {
                remainingTime -= deltaTime;
            }
        }

        public bool IsExpired() => !buffData.isPermanent && remainingTime <= 0;
    }
}
