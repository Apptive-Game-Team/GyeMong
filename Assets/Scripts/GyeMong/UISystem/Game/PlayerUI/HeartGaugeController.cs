using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GyeMong.GameSystem.Creature.Player.Interface.Listener;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class HeartGaugeController : MonoBehaviour, IHpChangeListener
    {
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private Transform heartContainer;

        private List<Image> hearts = new List<Image>();
        private int maxHp;
        private int currentHp;

        private void Start()
        {
            SceneContext.Character.changeListenerCaller.AddHpChangeListener(this);

            maxHp = (int)SceneContext.Character.stat.HealthMax;
            CreateHearts(maxHp);
            OnChanged(SceneContext.Character.CurrentHp);
        }
        
        private void CreateHearts(int count)
        {
            foreach (Transform child in heartContainer)
                Destroy(child.gameObject);
            hearts.Clear();

            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(heartPrefab, heartContainer);
                var img = obj.GetComponent<Image>();
                hearts.Add(img);
            }
            LayoutHearts();
        }

        private void OnRectTransformDimensionsChange() { LayoutHearts(); }

        private void LayoutHearts()
        {
            if (hearts.Count == 0 || !(heartContainer is RectTransform container)) return;
            var layout = heartContainer.GetComponent<LayoutGroup>();
            if (layout != null) layout.enabled = false;
            float width = container.rect.width;
            float height = container.rect.height;
            if (width <= 0 || height <= 0) return;
            const float gap = 2;
            int columns = Mathf.Min(hearts.Count, Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(hearts.Count * width / height))));
            int rows = Mathf.CeilToInt((float)hearts.Count / columns);
            float size = Mathf.Min((width - gap * (columns - 1)) / columns, (height - gap * (rows - 1)) / rows);
            for (int i = 0; i < hearts.Count; i++)
            {
                var rect = hearts[i].rectTransform;
                rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = new Vector2((i % columns) * (size + gap) + size / 2, -(i / columns) * (size + gap) - size / 2);
            }
        }

        public void OnChanged(float hp)
        {
            currentHp = Mathf.Clamp((int)hp, 0, maxHp);

            for (int i = 0; i < hearts.Count; i++)
            {
                hearts[i].enabled = (i < currentHp);
            }
        }
    }
}
