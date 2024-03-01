using System;
using Snowy.Input;
using UI;
using UnityEngine;

namespace Systems.Interaction_System
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] bool isPlayer = true;
        [SerializeField] bool canInteract = true;
        public Interactable interactable;

        
        private void Start()
        {
            if (InputManager.Instance) Initialize(InputManager.Instance);
        }

        private void Initialize(InputManager inputManager)
        {
            inputManager.OnInteract += Interact;
        }

        private void Interact(ButtonState obj)
        {
            if (!canInteract || interactable == null) return;

            if (obj == ButtonState.Pressed)
            {
                interactable.Interact(this);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Interactable newInteractable))
            {
                CheckInteractable(newInteractable);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Interactable newInteractable))
            {
                RemoveInteractable(newInteractable);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out Interactable newInteractable))
            {
                CheckInteractable(newInteractable);
            }
        }

        private void CheckInteractable(Interactable newInteractable)
        {
            if (newInteractable.activateWithoutInput)
            {
                newInteractable.Interact(this);
            }else if (newInteractable == interactable && isPlayer)
            {
                if (!NotificationManager.Instance.IsShowingPopup())
                {
                    NotificationManager.Instance.ShowNotification(NotificationType.Popup, interactable.promptText, 9999f);
                }
            }
            else
            {
                if (interactable)
                {
                    float distance = Vector3.Distance(transform.position, newInteractable.transform.position);
                    float oldDistance = Vector3.Distance(transform.position, interactable.transform.position);
                    if (distance >= oldDistance) return;
                }
                
                // Remove the old notification
                if (interactable) RemoveInteractable(interactable);
                if (newInteractable.CanInteract(this)) interactable = newInteractable;
            }
        }

        public bool IsPlayer() => isPlayer;
        
        public void RemoveInteractable(Interactable inter)
        {
            if (interactable == inter) interactable = null;
            if (isPlayer) NotificationManager.Instance.HideNotification(NotificationType.Popup);
        }
    }
}