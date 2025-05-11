using System;
using Gyemong.GameSystem.Buff.Data;

namespace Gyemong.GameSystem.Buff
{
    public static class BuffEvents
    {
        public static event Action<BuffData, IBuffable> OnBuffApplied;
        public static event Action<BuffData, IBuffable> OnBuffExpired;

        public static void TriggerBuffApplied(BuffData buff, IBuffable target)
        {
            OnBuffApplied?.Invoke(buff, target);
        }

        public static void TriggerBuffExpired(BuffData buff, IBuffable target)
        {
            OnBuffExpired?.Invoke(buff, target);
        }
    }
}
