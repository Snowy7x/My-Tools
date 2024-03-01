using System;
using Systems.Interaction_System.Actions;
using UnityEditor;
using UnityEngine;

namespace Systems.Interaction_System
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] public string promptText = "Press [E] to interact";
        [SerializeField] public bool isActive = true;
        public bool activateWithoutInput = false;
        [SerializeField] bool onlyPlayerCanInteract = true;
        [InteractActions] public string actionType;
        [SerializeReference] public IInteractAction action;
        Collider _collider;

        private void OnValidate()
        {
            if (actionType == null || actionType == "") return;
            
            Type type = Utility.GetType(actionType);
            
            if (type == null)
            {
                Debug.LogError("Type " + actionType + " does not exist!");
                return;
            }
            
            if (type.GetInterface("IInteractAction") == null)
            {
                Debug.LogError("Type " + actionType + " does not implement IInteractAction!");
            }else if (action == null || action.GetType() != type)
            {
                // Create it as a subclass of IInteractAction
                action = (IInteractAction) Activator.CreateInstance(type);
                // change the action type to the type name
                action.SetInteractable(this);
            } else
            {
                if (action != null) action.SetInteractable(this);
                else
                {
                    Debug.LogError("Action is null!");
                }
            }
        }

        private void Start()
        {
            if (!TryGetComponent(out _collider))
            {
                Debug.LogError("Interactable object " + gameObject.name + " does not have a collider!\nAdding a BoxCollider by default.");
                _collider = gameObject.AddComponent<BoxCollider>();
            }
            
            _collider.isTrigger = true;
            
            action.SetInteractable(this);
        }
        
        public void Interact(Interactor interactor)
        {
            Debug.Log("Interacting with " + gameObject.name);
            action.Interact(interactor);
        }

        public bool CanInteract(Interactor interactor)
        {
            if (!isActive) return false;
            if (onlyPlayerCanInteract && !interactor.IsPlayer()) return false;
            return true;
        }
        
        public bool IsActive() => isActive;
    }
}