using UnityEngine;

namespace Systems.Advanced_Character.Animation_System
{
    // Default Animation Cycle
    [CreateAssetMenu(fileName = "Animation Cycle", menuName = "Snowy/Animation/Default Cycle", order = 0)]
    public class AnimationCycle : ScriptableObject
    {
        public AnimationState[] animationStates;
    }
}