using UnityEngine;

namespace Systems.Interaction_System.Actions
{
    public class DebugArea : IInteractAction
    {
        public string debugText = "This is a debug text";
        public Interactable Interactable { get; set; }
        public void Interact(Interactor interactor)
        {
            Debug.Log(debugText);
        }
    }
}