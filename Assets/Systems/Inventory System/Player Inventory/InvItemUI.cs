using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Snowy.Inventory
{
    public class InvItemUI : MonoBehaviour
    {
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text amountText;
        [SerializeField] Image icon;
        [SerializeField] Image background;
        public InventorySlot item;
        public string Name => item?.Name ?? "";
        public int index = 0;
        public int Amount => item?.amount ?? 0;

        public void Initalize(InventorySlot slot, int i)
        {
            item = slot;
            index = i;
            nameText.text = Name;
            amountText.text = Amount.ToString();
            icon.sprite = item?.itemData.itemSprite;
            
            background.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            icon.color = slot != null ? new Color(1f, 1f, 1f, 0.7f) : new Color(1f, 1f, 1f, 0f);
            
        }
        
        public void SetItem(InventorySlot item)
        {
            this.item = item;
            icon.color = item != null ? new Color(1f, 1f, 1f, 0.7f) : new Color(1f, 1f, 1f, 0f);
        }

        public int GetIndex() => index;

        public void SetCurrent(bool isCurrent)
        {
            if (isCurrent)
            {
                background.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                if (item != null) icon.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                background.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                if (item != null) icon.color = new Color(1f, 1f, 1f, 0.7f);
            }
        }

        public void Update()
        {
            if (item == null) return;
            nameText.text = Name;
            amountText.text = Amount.ToString();
            icon.sprite = item.itemData.itemSprite;
        }
    }
}