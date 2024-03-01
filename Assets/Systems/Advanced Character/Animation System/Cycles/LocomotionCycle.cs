 using UnityEngine;

namespace Systems.Advanced_Character.Animation_System.Cycles
{
    [CreateAssetMenu(fileName = "Locomotion Cycle", menuName = "Snowy/Animation/Locomotion Cycle", order = 0)]
    public class LocomotionCycle : ScriptableObject
    {
        public AnimationState idle = new AnimationState("Idle", true);
        public AnimationState walk = new AnimationState("Walk", true);
        public AnimationState run = new AnimationState("Run", true);
        public AnimationState jump = new AnimationState("Jump", true);
        public AnimationState fall = new AnimationState("Fall", true);
        [HideInInspector] public bool nameReadOnly = true;
    }
}