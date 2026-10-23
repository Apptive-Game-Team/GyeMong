using System;
using GyeMong.InputSystem;
using GyeMong.GameSystem;
using GyeMong.UISystem.Option.Controller;

namespace GyeMong.UISystem.Option
{
    public class OptionUIToggler : UIWindowToggler<OptionUIToggler>
    {
        protected override bool LocksPlayerMovement => false;
        protected override void OnWindowStateChanged(bool opened)
        {
            GameTime.SetPaused(opened);
            InputManager.Instance?.SuppressAdvanceForCurrentFrame();
        }

        protected override void Awake()
        {
            base.Awake();
            toggleKeyActionCode = ActionCode.Option;
        }

        private void Start()
        {
            SoundController soundController = FindObjectOfType<SoundController>(true);
            soundController.InitialSetting();
        }

        public void ToggleOption()
        {
            OpenOrCloseOption();
        }
    }
}
