using UnityEngine;

namespace System.Game.Rune.RuneUI
{
    public enum SelectableState
    {
        INACITVE = 0,
        ACTIVE = 1,
    }

    public abstract class SelectableUI : MonoBehaviour, ISelectableUI
    {
        public abstract void OnInteract();
        public abstract void OnLongInteract();
    }

    public interface ISelectableUI
    {
        public void OnInteract();
        public void OnLongInteract();
    }
}