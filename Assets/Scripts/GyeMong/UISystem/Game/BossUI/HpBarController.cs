using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss;
using GyeMong.UISystem.Game.BattleUI;
using UnityEngine;

namespace GyeMong.UISystem.Game.BossUI
{
    public class HpBarController : MonoBehaviour
    {
        private RectTransform _curHpBar;
        private RectTransform _curShieldBar;
    
        public Boss boss;
    
        private RectTransform _rectTransform;
        private float _curHp;
        private float _curShield;
        private float _maxHp = 100;
        private bool _isBossSetUp = false;
    
        public const float DEFAULT_HP = 100;
    
        private int currentPhase = -1;
        private void Awake()
        {
            _curHpBar = transform.Find("CurHp").GetComponent<RectTransform>();
            _curShieldBar = transform.Find("CurShield").GetComponent<RectTransform>();
            _rectTransform = GetComponent<RectTransform>();
            SceneContext.EffectManager.CachingHpBar(this);
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
                    if (currentPhase != boss.CurrentPhase)
                    {
                        currentPhase = boss.CurrentPhase;
                        _maxHp = boss.CurrentMaxHp;
                    }
                    UpdateHp(boss.CurrentHp, boss.CurrentShield);
                }
                else
                {
                    currentPhase = boss.CurrentPhase;
                    _maxHp = boss.CurrentMaxHp;
                    _isBossSetUp = true;
                    UpdateHp(boss.CurrentHp, boss.CurrentShield);
                }
            }
            else
            {
                _isBossSetUp = false;
                _maxHp = DEFAULT_HP;
            }
        }

        public void UpdateHp(float hp, float shield)
        {
            _curHp = hp;
            _curShield = shield;
            // Read the current rect so window resizing cannot leave a stale fill width.
            float width = _rectTransform.rect.width;
            float hpWidth = _maxHp > 0 ? width * Mathf.Clamp01(_curHp / _maxHp) : 0;
            float shieldWidth = _maxHp > 0 ? width * Mathf.Clamp01(_curShield / _maxHp) : 0;
            _curHpBar.sizeDelta = new Vector2(hpWidth, _curHpBar.sizeDelta.y);
            _curHpBar.anchoredPosition = new Vector2((hpWidth - width) / 2, 0);
            _curShieldBar.sizeDelta = new Vector2(shieldWidth, _curShieldBar.sizeDelta.y);
            _curShieldBar.anchoredPosition = new Vector2((shieldWidth - width) / 2, 0);
        }
    
        public void SetBoss(Boss boss)
        {
            this.boss = boss;
        }
    
        public void ClearBoss()
        {
            _maxHp = DEFAULT_HP;
            boss = null;
        }
    }
}
