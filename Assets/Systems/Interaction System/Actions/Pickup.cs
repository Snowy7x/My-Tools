using Snowy.Inventory;
using UnityEngine;

namespace Systems.Interaction_System.Actions
{
    public enum AfterPickupAction
    {
        Destroy,
        DisableThePickup,
        Nothing
    }
    public class Pickup : IInteractAction
    {
        public ItemScriptable itemScriptable;
        public int amount;
        public bool pickupOneAtATime = false;
        public AfterPickupAction afterPickupAction = AfterPickupAction.Destroy;
        private int left = -1;
        public bool canPickup = true;

        public Interactable Interactable { get; set; }
        
        public void Interact(Interactor interactor)
        {
            if (!canPickup) return;
            bool done = false;
            if (pickupOneAtATime)
            {
                if (left == -1) left = amount;
                if (left == 0) return;
                left--;
                interactor.GetComponent<Player>()?.Inventory.AddItem(itemScriptable, 1);
                done = left == 0;
            }
            else
            {
                interactor.GetComponent<Player>()?.Inventory.AddItem(itemScriptable, amount);
                done = true;
            }

            if (done)
            {
                switch (afterPickupAction)
                {
                    case AfterPickupAction.Destroy:
                        Object.Destroy(Interactable.gameObject);
                        break;
                    case AfterPickupAction.DisableThePickup:
                        Interactable.isActive = false;
                        break;
                    case AfterPickupAction.Nothing:
                        break;
                }
            }
            
            interactor.RemoveInteractable(Interactable);
        }
    }
}