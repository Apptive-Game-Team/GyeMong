using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Interface.Listener;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class SkillGaugeController : GaugeController, ISkillGaugeChangeListener
    {
        private PlayerChangeListenerCaller _changeListenerCaller;

        private Material gaugeEffectMaterial;
        private float _skillGauge;
        private float _maxSkillGauge;

        protected override float GetCurrentGauge()
        {
            return _skillGauge;
        }

        protected override float GetMaxGauge()
        {
            return _maxSkillGauge;
        }

        protected override void Awake()
        {
            base.Awake();
            gaugeEffectMaterial = transform.Find("Background").GetComponent<Image>().material;
        }

        private void Start()
        {
            _changeListenerCaller = SceneContext.Character.changeListenerCaller;
            _changeListenerCaller.AddSkillGaugeChangeListener(this);
            _maxSkillGauge = SceneContext.Character.stat.GrazeMax;
        }


        protected override void UpdateSkillGauge()
        {
            base.UpdateSkillGauge();
            if (GetCurrentGauge() >= GetMaxGauge()) 
            {
                gaugeEffectMaterial.SetFloat("_isUsable", 1);
            }
            else
            {
                gaugeEffectMaterial.SetFloat("_isUsable", 0);
            }
        }

        public void OnChanged(float data)
        {
            _skillGauge = data;
            UpdateSkillGauge();
        }

        protected override void Update() { } // Do not call base.Update()
        private void OnDestroy()
        {
            _changeListenerCaller?.RemoveSkillGaugeChangeListener(this);
        }

    }
}
