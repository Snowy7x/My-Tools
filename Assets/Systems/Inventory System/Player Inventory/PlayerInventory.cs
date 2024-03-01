using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Snowy.Input;
using Systems.FPS_Movement.Scripts;
using Systems.IK.Base;
using UnityEngine;

namespace Snowy.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] Transform itemsHolder;
        public Inventory inventory = new Inventory(10);
        [SerializeField] private List<InvItem> items = new List<InvItem>();
        
        public event System.Action<InventorySlot> OnInventoryItemAdded;
        public event System.Action<InventorySlot> OnInventoryItemRemoved;
        public event System.Action<InventorySlot> OnInventoryChanged;
        public event System.Action<int> OnCurrentSlotChanged;
        public int MaxSlots => inventory.MaxSlots;
        public InvItem currentItem;
        private int currentSlot = 0;
        private Player player;

        private void Start()
        {
            if (inventory == null)
                inventory = new Inventory();
            
            if (itemsHolder == null) 
                itemsHolder = transform;
            
            inventory.OnInventoryItemAdded += InventoryItemAdded;
            inventory.OnInventoryItemRemoved += InventoryItemRemoved;
            inventory.OnInventoryStackDecreased += (slot, amount) => OnInventoryChanged?.Invoke(slot);
            inventory.OnInventoryStackIncreased += (slot, amount) => OnInventoryChanged?.Invoke(slot);
            
            player = GetComponentInParent<Player>();
            if (InputManager.Instance) Initialize(InputManager.Instance);
            if (player.IK)player.IK.OnIKResolved += UpdateIK;
        }

        private void Initialize(InputManager input)
        {
            input.OnMouseWheel += (diff) =>
            {
                if (diff > 0)
                {
                    currentSlot++;
                    if (currentSlot >= inventory.MaxSlots)
                        currentSlot = 0;
                    
                }
                else if (diff < 0)
                {
                    currentSlot--;
                    if (currentSlot < 0)
                        currentSlot = inventory.MaxSlots - 1;
                }
                foreach (var item in items)
                {
                    item.gameObject.SetActive(item.slot.index == currentSlot);
                    if (player.AnimationManager) 
                        item.SetIKTargets(player.AnimationManager.rightHandTarget, player.AnimationManager.leftHandTarget);
                }
                
                currentItem = items.Find(x => x.slot.index == currentSlot);
                if (!currentItem)
                {
                    player.AnimationManager.PlayIdle();
                    player.AnimationManager.ResetIKHandTargets();
                }
                else
                {
                    player.AnimationManager.PlayOverrideAnimation(currentItem.itemData.holdAnimationStateName);
                }
                
                OnCurrentSlotChanged?.Invoke(currentSlot);
            };
            input.OnAttack += (state =>
            {
                if (currentItem)
                    currentItem.Use(state == ButtonState.Pressed, state == ButtonState.Held);
            });
        }

        private void InventoryItemRemoved(InventorySlot slot)
        {
            // Destroy the item
            foreach (var item in items)
            {
                if (item.slot == slot)
                {
                    Destroy(item.gameObject);
                    OnInventoryItemRemoved?.Invoke(slot);
                    items.Remove(item);
                    break;
                }
            }
        }

        private void InventoryItemAdded(InventorySlot slot)
        {
            // Instantiate the item
            InventoryObject prefab = slot.itemData.invObject;
            if (prefab != null)
            {
                InventoryObject item = Instantiate(prefab, itemsHolder);
                item.gameObject.SetActive(currentSlot == slot.index);
                item.gameObject.name = slot.itemData.name;
                item.slot = slot;
                items.Add(item as InvItem);
                if (items.Count == 1)
                {
                    currentItem = item as InvItem;
                    OnCurrentSlotChanged?.Invoke(currentSlot);
                    if (currentItem != null)
                        player.AnimationManager.PlayOverrideAnimation(currentItem.itemData.holdAnimationStateName);
                }

                OnInventoryItemAdded?.Invoke(slot);
            }
        }

        public void AddItem(ItemScriptable item, int amount = 1)
        {
            inventory.AddItem(item, amount);
        }
        
        public void RemoveItem(ItemScriptable item, int amount = 1)
        {
            inventory.RemoveItem(item, amount);
        }
        
        public void RemoveItem(InventorySlot slot, int amount = 1)
        {
            inventory.RemoveItem(slot, amount);
        }

        private void UpdateIK()
        {
            if (player.AnimationManager)
                currentItem?.SetIKTargets(player.AnimationManager.rightHandTarget, player.AnimationManager.leftHandTarget);
        }

        public Inventory GetInventory() => inventory;
        
        public InventorySlot GetCurrentSlot() => inventory.GetItemAt(currentSlot);
        
        [CanBeNull] public InventorySlot GetSlot(int index) => inventory.GetItemAt(index);
        
        public FpCamera FpCamera => player.FpCamera;
        public BaseIK IK => player.IK;
    }
}