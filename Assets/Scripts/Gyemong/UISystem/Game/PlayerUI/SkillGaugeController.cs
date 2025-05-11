using Gyemong.GameSystem.Creature.Player;
using Gyemong.GameSystem.Creature.Player.Interface.Listener;
using UnityEngine;
using UnityEngine.UI;

namespace Gyemong.UISystem.Game.PlayerUI
{
    public class SkillGaugeController : GaugeController, ISkillGaugeChangeListener
    {
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
            PlayerCharacter.Instance.changeListenerCaller.AddSkillGaugeChangeListener(this);
            _maxSkillGauge = PlayerCharacter.Instance.stat.GrazeMax;
        }


        private void UpdateSkillGauge()
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
    }
}
