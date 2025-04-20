/*using System.Collections;
using System.Collections.Generic;
using System.Game.Item;
using UnityEngine;

namespace System.Game.Inventory
{
    public class Inventory : SingletonObject<Inventory>
    {
        private InventorySlot[] slots; // 인벤토리 슬롯들

        private List<ItemObject> inventoryItemList; // 플레이어가 소지한 아이템 리스트.
        private List<ItemObject> inventoryTabList; // 선택한 탭에 따라 다르게 보여질 아이템 리스트.

        public Text Description_Text; // 부연 설명.
        public string[] tabDescription; // 탭 부연 설명.

        public Transform tf; // slot 부모객체.

        public GameObject go; // 인벤토리 활성화 비활성화.
        public GameObject[] selectedTabImages;
        public GameObject go_OOC; // 선택지 활성화 비활성화.

        private int selectedItem; // 선택된 아이템.
        private int selectedTab; // 선택된 탭.

        private bool activated; // 인벤토리 활성화시 true;
        private bool tabActivated; // 탭 활성화시 true
        private bool itemActivated; // 아이템 활성화시 true.
        private bool stopKeyInput; // 키입력 제한 (소비할 때 질의가 나올 텐데, 그 때 키입력 방지)
        private bool preventExec; // 중복실행 제한.

        private WaitForSeconds waitTime = new WaitForSeconds(0.01f);


        // Use this for initialization
        void Start()
        {
            inventoryItemList = new List<ItemObject>();
            inventoryTabList = new List<ItemObject>();
            slots = tf.GetComponentsInChildren<InventorySlot>();
        }

        public void RemoveSlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].RemoveItem();
                slots[i].gameObject.SetActive(false);
            }
        } // 인벤토리 슬롯 초기화

        public void ShowTab()
        {
            RemoveSlot();
            SelectedTab();
        } // 탭 활성화

        public void SelectedTab()
        {
            StopAllCoroutines();
            Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
            color.a = 0f;
            for (int i = 0; i < selectedTabImages.Length; i++)
            {
                selectedTabImages[i].GetComponent<Image>().color = color;
            }
            Description_Text.text = tabDescription[selectedTab];
            StartCoroutine(SelectedTabEffectCoroutine());
        } // 선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값 0으로 조정.

        IEnumerator SelectedTabEffectCoroutine()
        {
            while (tabActivated)
            {
                Color color = selectedTabImages[0].GetComponent<Image>().color;
                while (color.a < 0.5f)
                {
                    color.a += 0.03f;
                    selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                    yield return waitTime;
                }
                while (color.a > 0f)
                {
                    color.a -= 0.03f;
                    selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                    yield return waitTime;
                }

                yield return new WaitForSeconds(0.3f);
            }
        } // 선택된 탭 반짝임 효과

        public void ShowItem()
        {
            inventoryTabList.Clear();
            RemoveSlot();
            selectedItem = 0;

            switch (selectedTab)
            {
                case 0:
                    for (int i = 0; i < inventoryItemList.Count; i++)
                    {
                        if (Item.ItemType.Use == inventoryItemList[i].itemType)
                            inventoryTabList.Add(inventoryItemList[i]);
                    }
                    break;
                case 1:
                    for (int i = 0; i < inventoryItemList.Count; i++)
                    {
                        if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                            inventoryTabList.Add(inventoryItemList[i]);
                    }
                    break;
                case 2:
                    for (int i = 0; i < inventoryItemList.Count; i++)
                    {
                        if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                            inventoryTabList.Add(inventoryItemList[i]);
                    }
                    break;
                case 3:
                    for (int i = 0; i < inventoryItemList.Count; i++)
                    {
                        if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                            inventoryTabList.Add(inventoryItemList[i]);
                    }
                    break;
            } // 탭에 따른 아이템 분류. 그것을 인벤토리 탭 리스트에 추가

            for (int i = 0; i < inventoryTabList.Count; i++)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Additem(inventoryTabList[i]);
            } // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가

            SelectedItem();
        } // 아이템 활성화 (inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력)

        public void SelectedItem()
        {
            StopAllCoroutines();
            if (inventoryTabList.Count > 0)
            {
                Color color = slots[0].selected_Item.GetComponent<Image>().color;
                color.a = 0f;
                for (int i = 0; i < inventoryTabList.Count; i++)
                    slots[i].selected_Item.GetComponent<Image>().color = color;
                Description_Text.text = inventoryTabList[selectedItem].itemDescription;
                StartCoroutine(SelectedItemEffectCoroutine());
            }
            else
                Description_Text.text = "해당 타입의 아이템을 소유하고 있지 않습니다.";
        } // 선택된 아이템을 제외하고, 다른 모든 탭의 컬러 알파값을 0으로 조정.

        IEnumerator SelectedItemEffectCoroutine()
        {
            while (itemActivated)
            {
                Color color = slots[0].GetComponent<Image>().color;
                while (color.a < 0.5f)
                {
                    color.a += 0.03f;
                    slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                    yield return waitTime;
                }
                while (color.a > 0f)
                {
                    color.a -= 0.03f;
                    slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                    yield return waitTime;
                }

                yield return new WaitForSeconds(0.3f);
            }
        } // 선택된 아이템 반짝임 효과.

        // Update is called once per frame
        void Update()
        {
            if (!stopKeyInput)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    activated = !activated;

                    if (activated)
                    {
                        theAudio.Play(open_sound);
                        theOrder.NotMove();
                        go.SetActive(true);
                        selectedTab = 0;
                        tabActivated = true;
                        itemActivated = false;
                        ShowTab();
                    }
                    else
                    {
                        theAudio.Play(cancel_sound);
                        StopAllCoroutines();
                        go.SetActive(false);
                        tabActivated = false;
                        itemActivated = false;
                        theOrder.Move();
                    }
                } // 인벤토리 열고 닫기

                if (activated)
                {
                    if (tabActivated)
                    {
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (selectedTab < selectedTabImages.Length - 1)
                                selectedTab++;
                            else
                                selectedTab = 0;
                            theAudio.Play(key_sound);
                            SelectedTab();
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            if (selectedTab > 0)
                                selectedTab--;
                            else
                                selectedTab = selectedTabImages.Length - 1;
                            theAudio.Play(key_sound);
                            SelectedTab();
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            theAudio.Play(enter_sound);
                            Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                            color.a = 0.25f;
                            selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                            itemActivated = true;
                            tabActivated = false;
                            preventExec = true;
                            ShowItem();
                        }

                    } // 탭 활성화시 키입력 처리.

                    else if (itemActivated)
                    {
                        if (inventoryTabList.Count > 0)
                        {
                            if (Input.GetKeyDown(KeyCode.DownArrow))
                            {
                                if (selectedItem < inventoryTabList.Count - 2)
                                    selectedItem += 2;
                                else
                                    selectedItem %= 2;
                                theAudio.Play(key_sound);
                                SelectedItem();
                            }
                            else if (Input.GetKeyDown(KeyCode.UpArrow))
                            {
                                if (selectedItem > 1)
                                    selectedItem -= 2;
                                else
                                    selectedItem = inventoryTabList.Count - 1 - selectedItem;
                                theAudio.Play(key_sound);
                                SelectedItem();
                            }
                            else if (Input.GetKeyDown(KeyCode.RightArrow))
                            {
                                if (selectedItem < inventoryTabList.Count - 1)
                                    selectedItem++;
                                else
                                    selectedItem = 0;
                                theAudio.Play(key_sound);
                                SelectedItem();
                            }
                            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                            {
                                if (selectedItem > 0)
                                    selectedItem--;
                                else
                                    selectedItem = inventoryTabList.Count - 1;
                                theAudio.Play(key_sound);
                                SelectedItem();
                            }
                            else if (Input.GetKeyDown(KeyCode.Z) && !preventExec)
                            {
                                if (selectedTab == 0) // 소모품
                                {
                                    StartCoroutine(OOCCoroutine("사용", "취소"));
                                }
                                else if (selectedTab == 1)
                                {
                                    StartCoroutine(OOCCoroutine("장착", "취소"));
                                }
                                else // 비프음 출력
                                {
                                    theAudio.Play(beep_sound);
                                }

                            }
                        }

                        if (Input.GetKeyDown(KeyCode.X))
                        {
                            theAudio.Play(cancel_sound);
                            StopAllCoroutines();
                            itemActivated = false;
                            tabActivated = true;
                            ShowTab();
                        }
                    } // 아이템 활성화시 키입력 처리.

                    if (Input.GetKeyUp(KeyCode.Z)) // 중복 실행 방지.
                        preventExec = false;
                } // 인벤토리가 열리면 키입력처리 활성화.
            }
        }

        IEnumerator OOCCoroutine(string _up, string _down)
        {
            theAudio.Play(enter_sound);
            stopKeyInput = true;

            go_OOC.SetActive(true);
            theOOC.ShowTwoChoice(_up, _down);

            yield return new WaitUntil(() => !theOOC.activated);
            if (theOOC.GetResult())
            {
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (inventoryItemList[i].itemID == inventoryTabList[selectedItem].itemID)
                    {
                        if (selectedTab == 0)
                        {
                            theDatabase.UseItem(inventoryItemList[i].itemID);

                            if (inventoryItemList[i].itemCount > 1)
                                inventoryItemList[i].itemCount--;
                            else
                                inventoryItemList.RemoveAt(i);

                            // theAudio.Play() // 아이템 먹는 소리 출력.

                            ShowItem();
                            break;
                        }
                        else if (selectedTab == 1)
                        {
                            theEquip.EquipItem(inventoryItemList[i]);
                            inventoryItemList.RemoveAt(i);
                            ShowItem();
                            break;
                        }
                    }
                }
            }
            stopKeyInput = false;
            go_OOC.SetActive(false);
        }
    }
}
*/