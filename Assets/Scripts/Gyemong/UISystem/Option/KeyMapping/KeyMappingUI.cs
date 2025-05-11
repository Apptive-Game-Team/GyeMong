using Util;

namespace Gyemong.UISystem.Option.KeyMapping
{
    public class KeyMappingUI : SingletonObject<KeyMappingUI>
    {
        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OpenKeyMappingUI()
        {
            gameObject.SetActive(true);
        }
    }
}
