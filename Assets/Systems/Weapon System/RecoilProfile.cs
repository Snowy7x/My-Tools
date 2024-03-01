using UnityEngine;

namespace Systems.Weapon_System
{
    [CreateAssetMenu(fileName = "New Recoil Profile", menuName = "Snowy/Weapon/Recoil Profile")]
    public class RecoilProfile : ScriptableObject
    {
        public Vector3 recoilAmount;
        public Vector3 aimRecoilAmount;
        public float snapiness;
        public float recoverySpeed;
        
    }
}