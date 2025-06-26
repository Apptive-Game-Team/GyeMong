using Util;

namespace GyeMong.UISystem.Option.SoundControl
{
    public class SoundControlUI : SingletonObject<SoundControlUI>
    {
        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void OpenSoundControlImage()
        {
            gameObject.SetActive(true);
        }
    }
}
