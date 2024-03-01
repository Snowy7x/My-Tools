using System;
using TMPro;
using UnityEngine;

namespace Systems.Weapon_System.Creator.Attempt_01
{
    [ExecuteInEditMode]
    public class Indicator : MonoBehaviour
    {
        public Transform target;
        
        private void Update()
        {
            if (target == null)
                return;
            transform.position = target.position;
        }

        public void tEST()
        {
            
        }
    }
}