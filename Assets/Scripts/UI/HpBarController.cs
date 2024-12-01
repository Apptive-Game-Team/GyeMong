using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour
{
    private RectTransform _curHpBar;
    private float _hpBarWidth;
    private float _curHp;
    private float _maxHp = 100;
    private void Awake()
    {
        _curHpBar = transform.Find("CurHp").GetComponent<RectTransform>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        _hpBarWidth = rectTransform.rect.width;
    }

    public void UpdateHp(float hp)
    {
        _curHp = hp;
        Rect rect = _curHpBar.rect;
        rect.width = (_hpBarWidth - 20) * (_curHp / _maxHp);
        _curHpBar.sizeDelta = new Vector2(rect.width, rect.height);
        _curHpBar.localPosition = new Vector3((-_hpBarWidth + rect.width)/2 + 10, 0, 0);
    }
}