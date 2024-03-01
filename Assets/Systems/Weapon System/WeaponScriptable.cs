using Snowy.Inventory;
using UnityEngine;

namespace Systems.Weapon_System
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Snowy/Inventory/Weapon")]
    public class WeaponScriptable : ItemScriptable
    {
        public float damage;
        public float range;
        public float fireRate;
        public float reloadTime;
        public int maxAmmo;
        public int clipSize;
        public bool isAutomatic;
        public RecoilProfile recoilProfile;
        public Bullet bulletPrefab;
    }
}