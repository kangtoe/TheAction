using FastCampus.InventorySystem.Inventory;
using FastCampus.InventorySystem.Items;
using FastCampus.QuestSystem;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace FastCampus.InventorySystem.Inventory
{
    [CreateAssetMenu(fileName = "New Invnetory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        #region Variables

        public ItemDatabaseObject database;
        public InterfaceType type;

        [SerializeField]
        private Inventory container = new Inventory();

        public Action<ItemObject> OnUseItem;

        #endregion Variables

        #region Properties

        public InventorySlot[] Slots => container.slots;

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                foreach (InventorySlot slot in Slots)
                {
                    if (slot.item.id <= -1)
                    {
                        counter++;
                    }
                }

                return counter;
            }
        }

        #endregion Properties

        #region Unity Methods



        #endregion Unity Methods

        #region Methods
        public bool AddItem(Item item, int amount)
        {
            InventorySlot slot = FindItemInInventory(item);

            if (CalcItemSlot(item) > 0)
            {
                // 인벤토리 슬롯 부족
                if (EmptySlotCount <= 0)
                {
                    Debug.Log("not enough inventroy");
                    return false;
                }

                GetEmptySlot().UpdateSlot(item, amount);
            }
            else
            {
                slot.AddAmount(amount);
            }

            QuestManager.Instance.ProcessQuest(QuestsubpointType.GetItem, item.id);

            return true;            
        }

        public bool AddItem(Item[] items)
        {
            int needSlotCount = CalcItemSlot(items);
            // 인벤토리 슬롯 부족
            if (needSlotCount > EmptySlotCount)
            {
                Debug.Log("not enough inventory :: needSlotCount =" + needSlotCount + " || EmptySlotCount = " + EmptySlotCount);
                return false;
            }

            foreach (Item item in items)
            {
                AddItem(item, 1);
            }

            return true;
        }

        // 여러 아이템이 차지하게 될 인벤토리 슬롯을 계산
        public int CalcItemSlot(Item item)
        {
            int count = 0;
            Dictionary<int, int> itemInfo = new Dictionary<int, int>();

            InventorySlot slot = FindItemInInventory(item);
            // 빈 인벤토리 슬롯이 필요한 경우인가
            if (!database.itemObjects[item.id].stackable || slot == null)
            {
                count = 1;
            }

            return count;
        }

        public int CalcItemSlot(Item[] items)
        {
            int count = 0;

            Dictionary<int, int> itemInfo = new Dictionary<int, int>();
            foreach (Item item in items)
            {
                if (database.itemObjects[item.id].stackable)
                {
                    // 기존에 key가 존재한다면
                    if (itemInfo.ContainsKey(item.id))
                    {
                        itemInfo[item.id] += 1;
                    }
                    else
                    {
                        itemInfo.Add(item.id, 1);
                    }
                }
                else
                {
                    count++;
                }
            }
               
            return itemInfo.Count + count;
        }

        public InventorySlot FindItemInInventory(Item item)
        {
            return Slots.FirstOrDefault(i => i.item.id == item.id);
        }

        public bool IsContainItem(ItemObject itemObject)
        {
            return Slots.FirstOrDefault(i => i.item.id == itemObject.data.id) != null;
        }

        public InventorySlot GetEmptySlot()
        {
            return Slots.FirstOrDefault(i => i.item.id <= -1);
        }

        public void SwapItems(InventorySlot itemA, InventorySlot itemB)
        {
            if (itemA == itemB)
            {
                return;
            }

            if (itemB.CanPlaceInSlot(itemA.ItemObject) && itemA.CanPlaceInSlot(itemB.ItemObject))
            {
                InventorySlot temp = new InventorySlot(itemB.item, itemB.amount);
                itemB.UpdateSlot(itemA.item, itemA.amount);
                itemA.UpdateSlot(temp.item, temp.amount);
            }
        }

        #endregion Methods

        #region Save/Load Methods
        public string savePath;

        [ContextMenu("Save")]
        public void Save()
        {
            #region Optional Save
            //string saveData = JsonUtility.ToJson(Container, true);
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            //bf.Serialize(file, saveData);
            //file.Close();
            #endregion

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, container);
            stream.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            {
                #region Optional Load
                //BinaryFormatter bf = new BinaryFormatter();
                //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
                //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Container);
                //file.Close();
                #endregion

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
                Inventory newContainer = (Inventory)formatter.Deserialize(stream);
                for (int i = 0; i < Slots.Length; i++)
                {
                    Slots[i].UpdateSlot(newContainer.slots[i].item, newContainer.slots[i].amount);
                }
                stream.Close();
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            container.Clear();
        }
        #endregion Save/Load Methods

        public void UseItem(InventorySlot slotToUse)
        {
            if (slotToUse.ItemObject == null || slotToUse.item.id < 0 || slotToUse.amount <= 0)
            {
                return;
            }

            ItemObject itemObject = slotToUse.ItemObject;
            slotToUse.UpdateSlot(slotToUse.item, slotToUse.amount - 1);

            OnUseItem.Invoke(itemObject);
        }
    }
}