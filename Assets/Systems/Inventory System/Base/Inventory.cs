using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Snowy.Inventory
{
    public class Inventory 
    {
        protected int maxSlots = 10;
        protected bool canDropItems = true;
        protected bool canPickUpItems = true;
        public List<InventorySlot> inventorySlots = new();
        public int MaxSlots => maxSlots;
        
        public event Action<InventorySlot> OnInventoryItemAdded;
        public event Action<InventorySlot> OnInventoryItemRemoved;
        public event Action<InventorySlot, int> OnInventoryStackIncreased;
        public event Action<InventorySlot, int> OnInventoryStackDecreased;
        
        public Inventory(int maxSlots = 10)
        {
            this.maxSlots = maxSlots;
            inventorySlots = new List<InventorySlot>();
        }
        
        public void AddItem(ItemScriptable item, int amount = 1)
        {
            if (item.isStackable)
            {
                InventorySlot inventoryItem = inventorySlots.FirstOrDefault(i => i.itemData == item && i.amount < item.maxStack);
                if (inventoryItem != null)
                {
                    StackItem(inventoryItem, amount);
                }
                else
                {
                    CreateItem(item, amount);
                }
            }else
            {
                CreateItem(item, amount);
            }
        }
        
        protected void StackItem(InventorySlot inventoryItem, int amount)
        {
            OnInventoryStackIncreased?.Invoke(inventoryItem, amount);
            for (var i = 0; i < amount; i++)
            {
                if (inventoryItem.amount < inventoryItem.MaxStack)
                {
                    inventoryItem.amount++;
                }
                else
                {
                    AddItem(inventoryItem.itemData, amount - i);
                    break;
                }
            }
        }
        
        protected int RemoveStackItem(InventorySlot slot, int amount = 1)
        {
            var left = amount;
            for (var i = 0; i < amount; i++)
            {
                if (slot.amount > 1)
                {
                    slot.amount--;
                }
                else
                {
                    // Remove item from inventory
                    OnInventoryItemRemoved?.Invoke(slot);
                    inventorySlots.Remove(slot);
                    left--;
                    break;
                }
            }
            OnInventoryStackDecreased?.Invoke(slot, amount - left);
            return left;
        }
        
        public void RemoveItem(InventorySlot slot, int amount = 1)
        {
            if (slot.isStackable)
            {
                int left = RemoveStackItem(slot, amount);
                if (left > 0)
                {
                    Debug.Log("Not enough items to remove");
                }
            }
            else
            {
                OnInventoryItemRemoved?.Invoke(slot);
                inventorySlots.Remove(slot);
            }
        }
        
        public void RemoveItem(ItemScriptable item, int amount = 1)
        {
            InventorySlot slot = inventorySlots.FirstOrDefault(i => i.itemData == item);
            if (slot != null)
            {
                if (item.isStackable)
                {
                    int left = RemoveStackItem(slot, amount);
                    if (left > 0)
                    {
                        Debug.Log("Not enough items to remove");
                    }
                }
                else
                {
                    OnInventoryItemRemoved?.Invoke(slot);
                    inventorySlots.Remove(slot);
                }
            }
        }
        
        public void CreateItem(ItemScriptable itemScriptable, int amount)
        {
            if (inventorySlots.Count >= maxSlots) return;

            var item = new InventorySlot(itemScriptable);
            item.index = inventorySlots.Count;
            inventorySlots.Add(item);
            OnInventoryItemAdded?.Invoke(item);
            if (amount > 1)
            {
                if (itemScriptable.isStackable)
                {
                    StackItem(item, amount-1);
                }
                else
                {
                    for (var i = 0; i < amount - 1; i++)
                    {
                        CreateItem(itemScriptable, 1);
                    }
                }
            }
        }
        
        [CanBeNull] public InventorySlot GetItemAt(int index) => inventorySlots.FirstOrDefault(i => i.index == index);
        
        
    }
}