using System;
using System.Collections.Generic;
using UnityEngine;

public class HpBarController : MonoBehaviour
{
    private RectTransform _curHpBar;
    private RectTransform _curShieldBar;
    
    public Boss boss;
    
    private float _hpBarWidth;
    private float _shieldBarWidth;
    private float _curHp;
    private float _curShield;
    private float _maxHp = 100;
    private bool _isBossSetUp = false;
    
    public const float DEFAULT_HP = 100;
    
    private int currentPhase = 0;
    private List<float> maxHps = new List<float>();
    private void Awake()
    {
        _curHpBar = transform.Find("CurHp").GetComponent<RectTransform>();
        _curShieldBar = transform.Find("CurShield").GetComponent<RectTransform>();
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
        if (boss != null)
        {
            if (_isBossSetUp)
            {
                UpdateHp(boss.CurrentHp, boss.CurrentShield);
                if (currentPhase != boss.CurrentPhase)
                {
                    currentPhase = boss.CurrentPhase;
                    _maxHp = maxHps[currentPhase - 1];
                }
            }
            else
            {
                Tuple<int, float, float> bossInfo = boss.BossInfo;
                maxHps.Add(bossInfo.Item2);
                maxHps.Add(bossInfo.Item3);
                currentPhase = boss.CurrentPhase;
                _maxHp = maxHps[currentPhase - 1];
                _isBossSetUp = true;
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
        Rect hpRect = _curHpBar.rect;
        Rect shieldRect = _curShieldBar.rect;
        hpRect.width = (_hpBarWidth - 20) * (_curHp / _maxHp);
        shieldRect.width = (_shieldBarWidth - 20) * (_curShield / _maxHp);
        _curHpBar.sizeDelta = new Vector2(hpRect.width, hpRect.height);
        _curHpBar.localPosition = new Vector3((-_hpBarWidth + hpRect.width)/2 + 10, 0, 0);
        _curShieldBar.sizeDelta = new Vector2(shieldRect.width, shieldRect.height);
        _curShieldBar.localPosition = new Vector3((-_shieldBarWidth + shieldRect.width) / 2 + 10, 0, 0);
    }
    
    public void SetBoss(Boss boss)
    {
        this.boss = boss;
    }
    
    public void ClearBoss()
    {
        boss = null;
    }
}