using System;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.BossUI
{
    public class HpBarController : AbstractHpBarController
    {
        private Slider _hpBar;
        private Slider _shieldBar;
    
        public Boss boss;
        
        private float _curHp;
        private float _curShield;
        private float _maxHp = 100;
        private bool _isBossSetUp = false;
    
        public const float DEFAULT_HP = 100;
    
        private int currentPhase = -1;
        
        private void Awake()
        {
            SceneContext.EffectManager.CachingHpBar(this);
            
            _hpBar = transform.Find("HpBar").GetComponent<Slider>();
            _shieldBar = transform.Find("ShieldBar").GetComponent<Slider>();
        }

        private void Update()
        {
            UpdateBossHp();
        }

        private void UpdateBossHp()
        {
            if (boss != null)
            {
                if (_isBossSetUp)
                {
                    UpdateHp(boss.CurrentHp, boss.CurrentShield);
                    if (currentPhase != boss.CurrentPhase)
                    {
                        currentPhase = boss.CurrentPhase;
                        _maxHp = boss.CurrentMaxHp;
                    }
                }
                else
                {
                    currentPhase = boss.CurrentPhase;
                    _maxHp = boss.CurrentMaxHp;
                    _isBossSetUp = true;
                }
            }
            else
            {
                _isBossSetUp = false;
                _maxHp = DEFAULT_HP;
            }
        }

        public override void Clear()
        {
            _maxHp = DEFAULT_HP;
            boss = null;
        }

        public override void UpdateHp(float hp, float shield)
        {
            _curHp = hp;
            _curShield = shield;
            
            _hpBar.value = _curHp / _maxHp;
            _shieldBar.value = _curShield / _maxHp;
        }
    
        public void SetBoss(Boss boss)
        {
            this.boss = boss;
        }
    }
}