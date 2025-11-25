using UnityEngine;

namespace GyeMong.UISystem.Game.BossUI
{
    public abstract class AbstractHpBarController : MonoBehaviour
    {
        public abstract void Clear();
        public abstract void UpdateHp(float currentHp, float currentShield);

    }
}