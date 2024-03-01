namespace Snowy.Inventory
{
    public class InventorySlot
    {
        public int index = -1;
        public int amount;
        public string Name => itemData.name;
        public bool isStackable => itemData.isStackable;
        public ItemScriptable itemData;
        
        public int MaxStack => itemData.maxStack;
        
        public InventorySlot(ItemScriptable itemData, int amount = 1)
        {
            this.itemData = itemData;
            this.amount = amount;
        }
    }
}