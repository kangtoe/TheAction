using FastCampus.InventorySystem.Inventory;
using FastCampus.InventorySystem.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FastCampus.InventorySystem.UIs
{
    public class DynamicInventoryUI_New : InventoryUI
    {
        #region Variables

        [SerializeField]
        protected GridLayoutGroup content; // 버튼

        [SerializeField]
        protected GameObject slotPrefab;

        [SerializeField]
        protected Vector2 size;

        [SerializeField]
        protected Vector2 space;
        
        [Min(1), SerializeField]
        protected int numberOfColumn = 4;

        #endregion Variables

        #region Methods

        public override void CreateSlots()
        {            
            slotUIs = new Dictionary<GameObject, Inventory.InventorySlot>();

            for (int i = 0; i < inventoryObject.Slots.Length; ++i)
            {
                GameObject go = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, transform);
                go.GetComponent<RectTransform>().SetParent(content.transform);

                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnter(go); });
                AddEvent(go, EventTriggerType.PointerExit, delegate { OnExit(go); });
                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
                AddEvent(go, EventTriggerType.PointerClick, (data) => { OnClick(go, (PointerEventData)data); });

                inventoryObject.Slots[i].slotUI = go;
                slotUIs.Add(go, inventoryObject.Slots[i]);
                go.name += ": " + i;
            }

            SetContent();
        }

        protected override void OnRightClick(InventorySlot slot)
        {
            inventoryObject.UseItem(slot);
        }

        // 자식 오브젝트의 크기와 배치에 따라 content의 길이 조절
        void SetContent()
        {
            RectTransform rect = content.GetComponent<RectTransform>();

            float width;
            float height;

            width = rect.sizeDelta.x;
            
            float slotSizeY = (content.spacing.y + content.cellSize.y);
            Debug.Log("slotSizeY : " + slotSizeY);
            float row = (content.transform.childCount / content.constraintCount);
            if (content.transform.childCount % content.constraintCount > 0) row++;
            Debug.Log("row : " + row);
            float extra = content.padding.top + content.padding.bottom;
            Debug.Log("extra : " + extra);

            height = slotSizeY * row + extra;
            Debug.Log("height : " + height);

            rect.sizeDelta = new Vector2(width, height);
        }

        #endregion Methods
    }
}
