using UnityEngine;

namespace Systems.Advanced_Character.Animation_System.Cycles
{
    // Weapon Animation Cycle
    [CreateAssetMenu(fileName = "Animation Cycle", menuName = "Snowy/Animation/Weapon Cycle", order = 0)]
    public class WeaponCycle : LocomotionCycle
    {
        public AnimationState aim;
        public AnimationState reload;
    }
}