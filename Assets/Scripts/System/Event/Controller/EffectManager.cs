using System.Collections;
using System.UI.Game.BossUI;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace System.Event.Controller
{
    /// <summary>
    /// Every Event Management class
    /// </summary>
    public class EffectManager : SingletonObject<EffectManager>
    {

        private CameraController cameraController;
    
        private HpBarController hpBarController;
        private RawImage hurtEffect;
        private RawImage black;
        private const float FADING_DELTA_TIME = 0.05f;

        public ChatController GetChatController()
        {
            return GetComponent<ChatController>();
        }

        public CameraController GetCameraController()
        {
            return cameraController;
        }
    
        public void SetCameraController(CameraController cameraController)
        {
            this.cameraController = cameraController;
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
        /// set HurtEffect transparency
        /// </summary>
        /// <param name="amount">0(none)~1(fill) alpha value</param>
        public IEnumerator HurtEffect(float amount)
        {
            return Fade(hurtEffect, amount);
        }

        /// <summary>
        /// vibrate camera
        /// </summary>
        public IEnumerator ShakeCamera(float time=0.25f)
        {
            return cameraController.ShakeCamera(time);
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
            hurtEffect = transform.Find("HurtEffect").GetComponent<RawImage>();
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
