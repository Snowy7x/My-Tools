using UnityEngine;

namespace Snowy.Inventory
{
    [CreateAssetMenu(fileName = "Item", menuName = "Snowy/Inventory/Plane Item", order = 0)]
    public class ItemScriptable : ScriptableObject
    {
        public string itemName;
        public Sprite itemSprite;
        public bool isStackable;
        public int maxStack;
        public string holdAnimationStateName;
        public InventoryObject invObject;
        public GameObject itemPrefab;
    }
}