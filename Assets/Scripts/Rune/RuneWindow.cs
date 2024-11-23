using JetBrains.Annotations;
using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISelectableContainerUI
{
    public void SetSelectableList();
    public void OnKeyInput();
}

public class RuneWindow : SingletonObject<RuneWindow>, ISelectableContainerUI
{
    RuneComponent playerRuneComp;

    [SerializeField] SelectableUI[] selectableUIs;
    int currentCursorNum;
    bool isOptionOpened;

    [SerializeField] GameObject cursorUI;
    [SerializeField] GameObject EquipRuneListUI;
    [SerializeField] GameObject AcquiredRuneListUI;
    [SerializeField] RuneUIObject runeIcon;
    [SerializeField] GameObject runeIcon_Empty;

    public void OpenOrCloseOption()
    {
        isOptionOpened = !isOptionOpened;
        gameObject.SetActive(isOptionOpened);
    }

    public void Init()
    {
        playerRuneComp = PlayerCharacter.Instance.GetComponent<RuneComponent>();
        ReDrawUI();
        SetSelectableList();
        StartCoroutine(DrawUICursor(0));
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
        else if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
        {
            selectableUIs[currentCursorNum].OnInteract();
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

    public void ReDrawUI()
    {
        foreach(Transform obj in EquipRuneListUI.transform)
        {
            Destroy(obj.gameObject);
        }
        foreach(Transform obj in AcquiredRuneListUI.transform)
        {
            Destroy(obj.gameObject);
        }

        int maxSlotNum = playerRuneComp.MaxRuneEquipNum;
        int equippedSlotNum = playerRuneComp.EquippedRuneList.Count;
        int emptySlotNum = maxSlotNum- equippedSlotNum;
        foreach(var runeData in playerRuneComp.EquippedRuneList)
        {
            Instantiate(runeIcon, EquipRuneListUI.transform);
            runeIcon.Init(runeData);
        }

        for(int i = 0; i < emptySlotNum; i++)
        {
            Instantiate(runeIcon_Empty, EquipRuneListUI.transform);
        }

        foreach(var runeData in playerRuneComp.AcquiredRuneList)
        {
            Instantiate(runeIcon, AcquiredRuneListUI.transform);
            runeIcon.Init(runeData);
        }

    }

    private void Update()
    {
        OnKeyInput();
    }

    private void OnEnable()
    {
        Init();
        gameObject.SetActive(isOptionOpened);
    }
}

public class EquipRuneListUI : MonoBehaviour
{

}