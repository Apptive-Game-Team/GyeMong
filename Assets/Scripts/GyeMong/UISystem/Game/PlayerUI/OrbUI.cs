using System;
using UnityEngine;
using UnityEngine.UI;

namespace GyeMong.UISystem.Game.PlayerUI
{
    public class OrbUI : MonoBehaviour
    {
        private Image fillImage;

        private void Awake()
        {
            fillImage = GetComponent<Image>();
        }

        // 0~1 사이로 채움
        public void SetFill(float amount)
        {
            amount = Mathf.Clamp01(amount);
            if (fillImage != null)
            {
                fillImage.fillAmount = amount;
            }
        }

    }
}