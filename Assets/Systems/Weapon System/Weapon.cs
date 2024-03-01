using System;
using Snowy.Inventory;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Weapon_System
{
    public class Weapon : InvItem
    {
        public Transform shootPoint;
        public WeaponScriptable weaponData => (WeaponScriptable) itemData;
        protected float lastFireTime;
        
        // Recoil
        private Vector3 recoil => weaponData.recoilProfile.recoilAmount;
        private Vector3 aimRecoil => weaponData.recoilProfile.aimRecoilAmount;

        Vector3 currRotational;
        Vector3 targetRotation;

        private void Start()
        {
            if (playerInventory == null)
            {
                Debug.LogError("PlayerInventory is null");
                return;
            }
            if (playerInventory.IK) playerInventory.IK.OnIKResolved += UpdateRecoil;
        }


        public override void Use(bool isPress, bool isHold)
        {
            if (Time.time - lastFireTime < weaponData.fireRate) return;
            if (weaponData.isAutomatic)
            {
                    Shoot();
            }
            else
            {
                if (isPress)
                        Shoot();
            }
            
            // TODO: auto reload
        }

        private void UpdateRecoil()
        {
            // Recoil
            targetRotation = Vector3.Lerp(targetRotation, Vector3.down, Time.deltaTime * weaponData.recoilProfile.recoverySpeed);
            currRotational = Vector3.Slerp(currRotational, targetRotation, Time.fixedDeltaTime * weaponData.recoilProfile.snapiness);
            playerInventory.FpCamera.transform.localRotation = Quaternion.Euler(currRotational);
        }

        public void Shoot()
        {
            if (weaponData.bulletPrefab != null)
            {
                var bullet = Instantiate(weaponData.bulletPrefab, shootPoint.position, shootPoint.rotation);
                bullet.damage = weaponData.damage;
                bullet.range = weaponData.range;
            }
            else
            {
                Debug.LogError("Bullet prefab is null");
            }

            // Recoil
            targetRotation += new Vector3(recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));
            lastFireTime = Time.time;
        }
    }
}