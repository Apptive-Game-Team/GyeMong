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
            OnChanged(SceneContext.Character.stat.HealthMax);
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
