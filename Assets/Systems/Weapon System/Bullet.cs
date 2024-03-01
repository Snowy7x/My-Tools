using System;
using Systems.Actor;
using UnityEngine;

namespace Systems.Weapon_System
{
    public class Bullet : MonoBehaviour
    {
        public GameObject hitEffect;
        public float speed;
        public float lifeTime;
        [SerializeField] public float hitPower = 10f;
        [HideInInspector]public float damage;
        [HideInInspector]public float range;

        private Rigidbody rb;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Destroy(gameObject, lifeTime);
        }
        
        private void Update()
        {
            rb.velocity = transform.forward * (speed * Time.deltaTime * 100f);
        }
        
        private void OnCollisionEnter(Collision other)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, DamageCause.DamagedByActor);
            }
            
            if (hitEffect != null)
            {
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, other.contacts[0].normal);
                var effect = Instantiate(hitEffect, transform.position, rot);
                effect.transform.parent = other.transform;
                Destroy(effect, 10f);
            }
            
            rb.isKinematic = true;
            rb.useGravity = false;
            GetComponent<Collider>().enabled = false;
            
            // Add force to the object that was hit
            if (other.rigidbody != null)
            {
                other.rigidbody.AddForce(transform.forward * hitPower, ForceMode.Impulse);
            }
            
            Destroy(gameObject);
        }
    }
}