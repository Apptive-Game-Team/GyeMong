using Util;

namespace System.UI.Option.SoundControl
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
