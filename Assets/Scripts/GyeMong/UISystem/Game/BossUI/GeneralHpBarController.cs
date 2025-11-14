using GyeMong.GameSystem.Creature;
using UnityEngine;

namespace GyeMong.UISystem.Game.BossUI
{
    public class GeneralHpBarController : MonoBehaviour
    {
        private RectTransform _curHpBar;
        private RectTransform _curShieldBar;
        
        private GameObject _hpBar;
        private GameObject _shieldBar;
        private GameObject _hpBarBackground;
        
        public Creature creature;
    
        private float _hpBarWidth;
        private float _shieldBarWidth;
        private float _curHp;
        private float _curShield;
        private float _maxHp = 100;
        private bool _isBossSetUp = false;
    
        public const float DEFAULT_HP = 100;
    
        private int currentPhase = -1;
        
        private void Awake()
        {
            _curHpBar = transform.Find("CurHp").GetComponent<RectTransform>();
            _hpBar = _curHpBar.gameObject;
            _curShieldBar = transform.Find("CurShield").GetComponent<RectTransform>();
            _shieldBar = _curShieldBar.gameObject;
            _hpBarBackground = transform.Find("HpBarBackground").gameObject;
            RectTransform rectTransform = GetComponent<RectTransform>();
            _hpBarWidth = rectTransform.rect.width;
            _shieldBarWidth = rectTransform.rect.width;
        }

        private void Update()
        {
            UpdateBossHp();
        }

        private void UpdateBossHp()
        {
            if (creature != null)
            {
                if (!_isBossSetUp)
                {
                    _hpBar.SetActive(true);
                    _shieldBar.SetActive(true);
                    _hpBarBackground.SetActive(true);
                }
                _isBossSetUp = true;
                _maxHp = creature.MaxHp;
                UpdateHp(creature.CurrentHp, creature.CurrentShield);
                
            }
            else
            {
                if (_isBossSetUp)
                {
                    _hpBar.SetActive(false);
                    _shieldBar.SetActive(false);
                    _hpBarBackground.SetActive(false);
                }
                _isBossSetUp = false;
                _maxHp = DEFAULT_HP;
            }
        }

        public void UpdateHp(float hp, float shield)
        {
            _curHp = hp;
            _curShield = shield;
            Rect hpRect = _curHpBar.rect;
            Rect shieldRect = _curShieldBar.rect;
            hpRect.width = (_hpBarWidth - 20) * (_curHp / _maxHp);
            shieldRect.width = (_shieldBarWidth - 20) * (_curShield / _maxHp);
            _curHpBar.sizeDelta = new Vector2(hpRect.width, hpRect.height);
            _curHpBar.localPosition = new Vector3((-_hpBarWidth + hpRect.width)/2 + 10, 0, 0);
            _curShieldBar.sizeDelta = new Vector2(shieldRect.width, shieldRect.height);
            _curShieldBar.localPosition = new Vector3((-_shieldBarWidth + shieldRect.width) / 2 + 10, 0, 0);
        }
    }
}