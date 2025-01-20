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
    
    private RuneTreeSetter runeTreeSetter;
    [SerializeField] private List<RuneDataList> runeDataLists;
    private int currentRuneDataListIndex = 0;

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
        runeTreeSetter = new RuneTreeSetter(runeDataLists[currentRuneDataListIndex], this);
        
        foreach(Transform obj in EquipRuneListUI.transform)
        {
            Destroy(obj.gameObject);
        }
        foreach(Transform obj in AcquiredRuneListUI.transform)
        {
            if (obj.GetComponent<ITreeLayoutNode>() != null)
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
        
        runeTreeSetter.SetTree();
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
    
    public class RuneTreeSetter
    {
        private RuneDataList runeDataList;
        private RuneWindow runeWindow;
        private List<RuneTreeNode> nodes = new List<RuneTreeNode>();
        
        public RuneTreeSetter(RuneDataList runeDataList, RuneWindow runeWindow)
        {
            this.runeDataList = runeDataList;
            this.runeWindow = runeWindow;
        }

        public void SetTree()
        {
            nodes.Clear();
            foreach(RuneData runeData in runeDataList.runeDataList)
            {
                RuneTreeNode runeUI = Instantiate(runeWindow.runeIcon, runeWindow.AcquiredRuneListUI.transform);
                runeUI.gameObject.name = runeData.name + " Icon";
                
                runeUI.uiState = RuneUIState.UNEQUIPPED;
                runeUI.Init(FindNodeById(runeData.parentID), runeData);
                nodes.Add(runeUI);
                runeWindow.selectableUIs.Add(runeUI);
            }
        }
        
        private RuneTreeNode FindNodeById(int id)
        {
            foreach(RuneTreeNode node in nodes)
            {
                if(id == node.ID)
                {
                    return node;
                }
            }
            return null;
        }
    }
}

public class EquipRuneListUI : MonoBehaviour
{

}