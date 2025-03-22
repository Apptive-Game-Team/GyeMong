using UnityEngine;

namespace System.Game.Rune.RuneUI
{
    public class RuneUIObjectCreator : MonoBehaviour
    {
        [SerializeField] private RuneDataList _runeDataList;
        [SerializeField] private int _runeID;
        [SerializeField] private RuneUIObject _runeUIObjectPrefab;
        
        private RuneUIObject _runeUIObject;
        
        private void Start()
        {
            CreateRuneUIObject();
        }
        
        private void CreateRuneUIObject()
        {
            RuneData runeData = _runeDataList.GetRuneData(_runeID);
            _runeUIObject = Instantiate(_runeUIObjectPrefab, transform);
            _runeUIObject.transform.position = transform.position;
            _runeUIObject.Init(runeData);
        }
    }
}