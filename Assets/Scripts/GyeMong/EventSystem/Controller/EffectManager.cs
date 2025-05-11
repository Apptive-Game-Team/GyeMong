using System.Collections;
using GyeMong.UISystem.Game.BossUI;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace GyeMong.EventSystem.Controller
{
    /// <summary>
    /// Every Event Management class
    /// </summary>
    public class EffectManager : SingletonObject<EffectManager>
    {
        private HpBarController hpBarController;
        private RawImage hurtEffect;
        private RawImage black;
        private const float FADING_DELTA_TIME = 0.05f;

        public ChatController GetChatController()
        {
            return GetComponent<ChatController>();
        }

        public void UpdateHpBar(float hp, float shield)
        {
            hpBarController.UpdateHp(hp, shield);
        }
    
        public HpBarController GetHpBarController()
        {
            return hpBarController;
        }
        
        /// <summary>
        /// The screen is getting Ligther
        /// </summary>
        /// <returns>return IEnumerator</returns>
        public IEnumerator FadeIn()
        {
            return Fade(black, 0);
        }

        /// <summary>
        /// The screen is getting darker
        /// </summary>
        /// /// <returns>return IEnumerator</returns>
        public IEnumerator FadeOut()
        {
            return Fade(black, 1);
        }

        public IEnumerator FadeInFirst()
        {
            Color color = black.color;
            color.a = 1;
            black.color = color;
            return Fade(black, 0);
        }

        public IEnumerator ChangeToBlackScreen()
        {
            Color color = black.color;
            color.a = 1;
            black.color = color;

            yield return null;
        }

        private IEnumerator Fade(RawImage image, float targetAlpha, float duration = 0.5f)
        {
            Color color = image.color;
            float startAlpha = color.a;
            float deltaAlpha = targetAlpha - startAlpha;
            int targetLoop = (int)(duration / FADING_DELTA_TIME);
            for (int i=0; i<targetLoop; i++)
            {
                color.a = (startAlpha + i * deltaAlpha / targetLoop);
                image.color = color;
                yield return new WaitForSeconds(FADING_DELTA_TIME);
            }
            color.a = targetAlpha;
            image.color = color;
        }

        private void CachingComponents()
        {
            black = transform.Find("Black").GetComponent<RawImage>();
        }
        public void CachingHpBar(HpBarController controller)
        {
            hpBarController = controller;
            hpBarController.gameObject.SetActive(false);
        }
        protected override void Awake()
        {
            base.Awake();
            CachingComponents();
        }
    }
}
