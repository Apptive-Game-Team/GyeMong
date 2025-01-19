using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using runeSystem.RuneTreeSystem;
using UnityEngine;

public interface ISelectableContainerUI
{
    public void OnKeyInput();
}

// Rune UI Object
public class RuneWindow : SingletonObject<RuneWindow>, ISelectableContainerUI
{
    // RuneComponent (Connect to BuffComponent)
    RuneComponent playerRuneComp;

    [SerializeField] List<SelectableUI> selectableUIs;
    int currentCursorNum;
    bool isOptionOpened;

    [SerializeField] GameObject cursorUI;
    [SerializeField] GameObject EquipRuneListUI;
    [SerializeField] GameObject AcquiredRuneListUI;
    [SerializeField] RuneTreeNode runeIcon;
    [SerializeField] GameObject runeIcon_Empty;
    [SerializeField] GameObject runeCanvas;
    [SerializeReference] GameObject runeDescriptionUI;
    
    public void OpenOrCloseOption()
    {
        isOptionOpened = !isOptionOpened;
        gameObject.SetActive(isOptionOpened);
        PlayerCharacter.Instance.SetPlayerMove(!isOptionOpened);
    }

    // Initialize RuneWindow (Caching RuneComponent, Draw UI, Draw Cursor)
    public void Init()
    {
        playerRuneComp = PlayerCharacter.Instance.GetComponent<RuneComponent>();
        ReDrawUI();
        StartCoroutine(DrawUICursor(0));
    }

    // managing Move Cursor and interact
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
        yield return DrawDescriptionUI();
    }
    IEnumerator SetCursorNum(int step)
    {
        int listLength = selectableUIs.Count;
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
    
    // move cursor to selectable UI
    IEnumerator MoveUICursor()
    {
        cursorUI.GetComponent<RectTransform>().position = selectableUIs[currentCursorNum].GetComponent<RectTransform>().position;
        yield return null;
    }

    IEnumerator DrawDescriptionUI()
    {
        IDescriptionalUI descriptionalUI = selectableUIs[currentCursorNum] as IDescriptionalUI;
        IDescriptionUI descriptionUI =  runeDescriptionUI.GetComponent<IDescriptionUI>();
        descriptionalUI.SetDescription(descriptionUI);
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

        selectableUIs.Clear();
        
        int maxSlotNum = playerRuneComp.MaxRuneEquipNum;
        int equippedSlotNum = playerRuneComp.EquippedRuneList.Count;
        int emptySlotNum = maxSlotNum- equippedSlotNum;
        foreach(var runeData in playerRuneComp.EquippedRuneList)
        {
            RuneUIObject runeUI = Instantiate(runeIcon, EquipRuneListUI.transform);
            runeUI.uiState = RuneUIState.EQUIPPED;
            runeIcon.Init(runeData);
            selectableUIs.Add(runeUI);
        }

        for(int i = 0; i < emptySlotNum; i++)
        {
            Instantiate(runeIcon_Empty, EquipRuneListUI.transform);
        }

        foreach(var runeData in playerRuneComp.AcquiredRuneList)
        {
            RuneUIObject runeUI = Instantiate(runeIcon, AcquiredRuneListUI.transform);
            runeUI.uiState = RuneUIState.UNEQUIPPED;
            runeIcon.Init(runeData);
            selectableUIs.Add(runeUI);
        }

    }

    protected override void Awake()
    {
        base.Awake();   
        DontDestroyOnLoad(transform.parent.gameObject);
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