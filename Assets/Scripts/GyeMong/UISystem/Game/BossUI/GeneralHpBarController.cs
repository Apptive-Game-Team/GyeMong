using GyeMong.GameSystem.Creature;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.BossUI
{
    public class GeneralHpBarController : AbstractHpBarController
    {
        private Slider _hpBar;
        private Slider _shieldBar;
        
        public Creature creature;
        private Creature _creature;
        
        private float _curHp;
        private float _curShield;
        private float _maxHp = 100;
        private bool _isBossSetUp = false;
    
        public const float DEFAULT_HP = 100;
    
        private int currentPhase = -1;
        
        private void Awake()
        {
            _hpBar = transform.Find("HpBar").GetComponent<Slider>();
            _shieldBar = transform.Find("ShieldBar").GetComponent<Slider>();
            _creature = creature;
            SceneContext.EffectManager.CachingHpBar(this);
        }

        private void Update()
        {
            UpdateBossHp();
        }

        private void UpdateBossHp()
        {
            if (creature != null)
            {
                _isBossSetUp = true;
                _maxHp = creature.MaxHp;
                UpdateHp(creature.CurrentHp, creature.CurrentShield);
            }
            else
            {
                _isBossSetUp = false;
                _maxHp = DEFAULT_HP;
            }
        }

        public override void Clear()
        {
            UpdateHp(0, 0);
            creature = null;
        }

        public override void UpdateHp(float hp, float shield)
        {
            _curHp = hp;
            _curShield = shield;
            
            _hpBar.value = _curHp / _maxHp;
            _shieldBar.value = _curShield / _maxHp;
        }

        public void SetCreature()
        {
            creature = _creature;
        }
    }
}