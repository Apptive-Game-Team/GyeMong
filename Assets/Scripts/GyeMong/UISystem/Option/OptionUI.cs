using UnityEngine;
using UnityEngine.UI;
using Util;

namespace GyeMong.UISystem.Option
{
    public class OptionUI : SingletonObject<OptionUI>
    {
        private Image optionImage;
        public bool isOptionUITop = true;

        private void Start()
        {
            FindOptionImage();
        }

        private void FindOptionImage()
        {
            Transform optionImageTransform = transform.Find("Canvas/OptionUI");
            optionImage = optionImageTransform.GetComponent<Image>();
        }
    }
}
