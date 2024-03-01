using UnityEngine;

namespace Snowy.Inventory
{
    public class InvItem : InventoryObject
    {
        public PlayerInventory playerInventory;
        
        void Awake()
        {
            playerInventory = GetComponentInParent<PlayerInventory>();
        }
        
        public Transform rightHand;
        public Transform leftHand;

        public void SetIKTargets(Transform rightHandIK, Transform leftHandIK)
        {
            rightHandIK.position = rightHand.position;
            rightHandIK.rotation = rightHand.rotation;
            
            leftHandIK.position = leftHand.position;
            leftHandIK.rotation = leftHand.rotation;
        }

        public override void Use(bool isPress, bool isHold)
        {
            Debug.Log("Using " + name);
        }
    }
}