using JetBrains.Annotations;
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
    SelectableUI[] selectableUIs;
    int currentCursorNum;
    int maxRuneEquipNum = 2;


    [SerializeField] GameObject cursorUI;


    public void Init()
    {
        SetSelectableList();
        DrawUICursor(0);
    }

    public void SetSelectableList()
    {
        selectableUIs = GetComponentsInChildren<SelectableUI>();
    }



    public void OnKeyInput()
    {
        if(InputManager.Instance.GetKeyDown(ActionCode.MoveUp) || InputManager.Instance.GetKeyDown(ActionCode.MoveLeft))
        {
            StartCoroutine(DrawUICursor(-1));
        }
        else if(InputManager.Instance.GetKeyDown(ActionCode.MoveDown) || InputManager.Instance.GetKeyDown(ActionCode.MoveRight))
        {
            StartCoroutine(DrawUICursor(1));
        }
    }

    IEnumerator DrawUICursor(int step) 
    {
        yield return SetCursorNum(step);
        yield return MoveUICursor();
    }
    IEnumerator SetCursorNum(int step)
    {
        int listLength = selectableUIs.Length;
        currentCursorNum += step;
        if (currentCursorNum >= listLength)
        {
            currentCursorNum = 0;
        }
        if(currentCursorNum < 0)
        {
            currentCursorNum += listLength;
        }
        yield return null;
    }
    IEnumerator MoveUICursor()
    {
        cursorUI.GetComponent<RectTransform>().position = selectableUIs[currentCursorNum].GetComponent<RectTransform>().position;
        yield return null;
    }

    private void Update()
    {
        OnKeyInput();
    }

    private void Awake()
    {
        Init();
    }
}
