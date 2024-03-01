using UnityEngine;

namespace Systems.Advanced_Character.Movement
{
    public enum State
    {
        Idle,
        Walking,
        Running,
        Falling
    } 
    
    public class MovementBrain : MonoBehaviour
    {
        protected State state;
        protected State previousState;
        
        public State State => state;
        
        public virtual bool IsGrounded() => false;
        public virtual float GetSpeed() => 0;
    }
}