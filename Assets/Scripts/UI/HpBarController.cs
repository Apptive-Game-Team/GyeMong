using System;
using System.Collections.Generic;
using UnityEngine;

public class HpBarController : MonoBehaviour
{
    private RectTransform _curHpBar;
    
    public Boss boss;
    
    private float _hpBarWidth;
    private float _curHp;
    private float _maxHp = 100;
    private bool _isBossSetUp = false;

    private int currentPhase = 0;
    private List<float> maxHps = new List<float>();
    private void Awake()
    {
        _curHpBar = transform.Find("CurHp").GetComponent<RectTransform>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        _hpBarWidth = rectTransform.rect.width;
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
                UpdateHp(boss.CurrentHp);
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
        }
    }

    public void UpdateHp(float hp)
    {
        _curHp = hp;
        Rect rect = _curHpBar.rect;
        rect.width = (_hpBarWidth - 20) * (_curHp / _maxHp);
        _curHpBar.sizeDelta = new Vector2(rect.width, rect.height);
        _curHpBar.localPosition = new Vector3((-_hpBarWidth + rect.width)/2 + 10, 0, 0);
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