using Util;

namespace Gyemong.UISystem.Option.SoundControl
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
