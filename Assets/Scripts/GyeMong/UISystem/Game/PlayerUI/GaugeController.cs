using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public abstract class GaugeController : MonoBehaviour
    {
        private Slider skillSlider;

        protected abstract float GetCurrentGauge();
        protected abstract float GetMaxGauge();
    
        protected virtual void Awake()
        {
            skillSlider = GetComponent<Slider>();
        }

        protected virtual void Update()
        {
            UpdateSkillGauge();
        }

        protected virtual void UpdateSkillGauge()
        {
            skillSlider.value = GetCurrentGauge() / GetMaxGauge();
        }
    }
}
