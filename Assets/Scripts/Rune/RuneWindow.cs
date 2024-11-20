using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISelectableContainerUI
{
    public void SetSelectableList();
    public void OnKeyInput();
}

public class RuneWindow : MonoBehaviour, ISelectableContainerUI
{
    [SerializeField] ISelectableUI[] selectableUIs;
    [SerializeField] int currentCursorNum;

    public void SetSelectableList()
    {
        selectableUIs = GetComponentsInChildren<ISelectableUI>();
    }

    public void OnKeyInput()
    {
        if(InputManager.Instance.GetKeyDown(ActionCode.MoveUp))
        {
            currentCursorNum++;
        }
    }

    private void Update()
    {
        OnKeyInput();
    }
}
