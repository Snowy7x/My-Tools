using UnityEngine;

namespace Systems.Interaction_System
{
    public interface IInteractAction
    {
        Interactable Interactable { get; set; }

        void SetInteractable(Interactable interactable)
        {
            Interactable = interactable;
        }

        void Interact(Interactor interactor);
    }
}