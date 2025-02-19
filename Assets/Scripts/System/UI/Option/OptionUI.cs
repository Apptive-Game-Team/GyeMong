using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace System.UI.Option
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
