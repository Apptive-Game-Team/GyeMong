using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player.Interface.Listener;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class SeasonOrbController : GaugeController, ISeasonOrbChangeListener
    {
        [Header("UI Refs")]
        [SerializeField] private OrbUI orbPrefab;

        [Header("Options")]
        [Tooltip("true: 최대치 만큼 슬롯을 항상 보여줌 / false: 필요한 개수만 활성화(정수개 + 소수 1칸)")]
        [SerializeField] private bool showEmptySlots = false;
        private RectTransform orbContainer;
        private float _orb;          // 현재 수치 (예: 2.3)
        private float _maxOrb;       // 최대 수치 (예: 3)
        private readonly List<OrbUI> _orbList = new();
        private Material gaugeEffectMaterial;

        protected override float GetCurrentGauge() => _orb;
        protected override float GetMaxGauge() => _maxOrb;

        protected override void Awake()
        {
            orbContainer = transform as RectTransform;
        }

        private void Start()
        {
            SceneContext.Character.changeListenerCaller.AddSeasonOrbChangeListener(this);
            _maxOrb = Mathf.Max(0f, SceneContext.Character.stat.OrbMax);

            EnsurePool(Mathf.CeilToInt(_maxOrb));
            UpdateSeasonOrb();
        }
        

        private void EnsurePool(int count)
        {
            if (orbPrefab == null || orbContainer == null) return;

            // 부족하면 생성
            for (int i = _orbList.Count; i < count; i++)
            {
                var orb = Instantiate(orbPrefab, orbContainer);
                _orbList.Add(orb);
            }

            // 남는 건 파괴(보통은 안 줄어들겠지만 방어)
            for (int i = _orbList.Count - 1; i >= count; i--)
            {
                if (_orbList[i] != null)
                    Destroy(_orbList[i].gameObject);
                _orbList.RemoveAt(i);
            }
        }

        private void UpdateSeasonOrb()
        {
            // 값 정리
            float value = Mathf.Clamp(_orb, 0f, _maxOrb);
            int maxSlots = Mathf.CeilToInt(_maxOrb);     // 슬롯(구슬 자리) 개수
            EnsurePool(maxSlots);

            int full = Mathf.FloorToInt(value);          // 꽉 찬 구슬 개수
            float frac = value - full;                   // 마지막 구슬의 소수부(0~1)

            // float 오차로 근접 정수일 때 깔끔히 처리
            if (frac < 0.001f) frac = 0f;

            for (int i = 0; i < _orbList.Count; i++)
            {
                float fill = 0f;
                bool active;

                if (showEmptySlots)
                {
                    // 슬롯은 항상 보이되, 채움만 조절
                    active = true;
                    if (i < full) fill = 1f;
                    else if (i == full) fill = frac;
                    else fill = 0f;
                }
                else
                {
                    // 필요한 슬롯만 보이기: 정수개 + (소수 있으면 한 칸 더)
                    if (i < full)
                    {
                        active = true; fill = 1f;
                    }
                    else if (i == full && frac > 0f)
                    {
                        active = true; fill = frac;
                    }
                    else
                    {
                        active = false; fill = 0f;
                    }
                }

                var go = _orbList[i].gameObject;
                if (go.activeSelf != active) go.SetActive(active);
                _orbList[i].SetFill(fill);
            }
        }

        // 외부(캐릭터)에서 값이 바뀔 때 호출됨
        public void OnChanged(float data)
        {
            _orb = data;
            UpdateSeasonOrb();
        }

        // 게이지 컨트롤러 기본 Update는 비활성
        protected override void Update() { } // Do not call base.Update()
    }
}
