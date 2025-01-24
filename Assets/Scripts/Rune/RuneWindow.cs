using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using runeSystem.RuneTreeSystem;
using Unity.VisualScripting;
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

    private CursorTargetSelector _cursorTargetSelector;
    private SelectableUI _currentCursoredObject;
    
    public void OpenOrCloseOption()
    {
        isOptionOpened = !isOptionOpened;
        gameObject.SetActive(isOptionOpened);
        PlayerCharacter.Instance.SetPlayerMove(!isOptionOpened);
    }
    
    public void ChangeRuneDataList(int index)
    {
        currentRuneDataListIndex = index;
        StartCoroutine(ReDrawUI());
    }

    // Initialize RuneWindow (Caching RuneComponent, Draw UI, Draw Cursor)
    public void Init()
    {
        playerRuneComp = PlayerCharacter.Instance.GetComponent<RuneComponent>();
        StartCoroutine(ReDrawUI());
        StartCoroutine(DrawUICursor());
    }

    // managing Move Cursor and interact
    public void OnKeyInput()
    {
        int x = InputManager.Instance.GetKeyDown(ActionCode.MoveRight) ? 1 :
            InputManager.Instance.GetKeyDown(ActionCode.MoveLeft) ? -1 : 0;

        int y = InputManager.Instance.GetKeyDown(ActionCode.MoveUp) ? 1 :
            InputManager.Instance.GetKeyDown(ActionCode.MoveDown) ? -1 : 0;
        if (!(x == 0 && y == 0))
        {
            _currentCursoredObject = _cursorTargetSelector.SelectTarget( 
                (x, y)
                , _currentCursoredObject);
            StartCoroutine(DrawUICursor());
        }
        

        if (InputManager.Instance.GetKeyDown(ActionCode.Interaction))
        {
            _currentCursoredObject.OnInteract();
        }
    }

    IEnumerator DrawUICursor() 
    {
        // yield return SetCursorNum(step);
        yield return MoveUICursor();
        if (_currentCursoredObject is IDescriptionalUI)
        {
            yield return DrawDescriptionUI();
        }
    }
    // IEnumerator SetCursorNum(int step)
    // {
    //     int listLength = selectableUIs.Count;
    //     currentCursorNum += step;
    //     if (currentCursorNum >= listLength)
    //     {
    //         currentCursorNum = 0;
    //     }
    //     if(currentCursorNum < 0)
    //     {
    //         currentCursorNum += listLength;
    //     }
    //     yield return null;
    // }
    
    // move cursor to selectable UI
    IEnumerator MoveUICursor()
    {
        cursorUI.GetComponent<RectTransform>().position = _currentCursoredObject.GetComponent<RectTransform>().position;
        yield return null;
    }

    IEnumerator DrawDescriptionUI()
    {
        IDescriptionalUI descriptionalUI = _currentCursoredObject as IDescriptionalUI;
        IDescriptionUI descriptionUI =  runeDescriptionUI.GetComponent<IDescriptionUI>();
        descriptionalUI.SetDescription(descriptionUI);
        yield return null;
    }
    
    public IEnumerator ReDrawUI()
    {
        runeTreeSetter = new RuneTreeSetter(runeDataLists[currentRuneDataListIndex], this);
        
        foreach(Transform obj in EquipRuneListUI.transform)
        {
            Destroy(obj.gameObject);
        }

        foreach (Transform obj in AcquiredRuneListUI.transform)
        {
            if (obj.GetComponent<ITreeLayoutNode>() != null)
                Destroy(obj.gameObject);
        }

        int maxSlotNum = playerRuneComp.MaxRuneEquipNum;
        int equippedSlotNum = playerRuneComp.EquippedRuneList.Count;
        int emptySlotNum = maxSlotNum- equippedSlotNum;
        foreach(var runeData in playerRuneComp.EquippedRuneList)
        {
            RuneUIObject runeUI = Instantiate(runeIcon, EquipRuneListUI.transform);
            runeUI.uiState = RuneUIState.EQUIPPED;
            runeIcon.Init(runeData);
        }

        for(int i = 0; i < emptySlotNum; i++)
        {
            Instantiate(runeIcon_Empty, EquipRuneListUI.transform);
        }
        runeTreeSetter.SetTree();
        selectableUIs = null;
        yield return null;
        selectableUIs = transform.GetComponentsInChildren<SelectableUI>().Where(ui => ui != null && ui.gameObject != null).ToList();
        _cursorTargetSelector = new CursorTargetSelector(selectableUIs);
        _currentCursoredObject = selectableUIs[5];
        
        StartCoroutine(DrawUICursor());
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