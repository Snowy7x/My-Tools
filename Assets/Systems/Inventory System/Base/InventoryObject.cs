using UnityEngine;

namespace Snowy.Inventory
{
    public abstract class InventoryObject : MonoBehaviour
    {
        public InventorySlot slot;
        public ItemScriptable itemData => slot.itemData;
        public int amount => slot.amount;
        public string Name => itemData.name;
        public abstract void Use(bool isPress, bool isHold);
    }
}