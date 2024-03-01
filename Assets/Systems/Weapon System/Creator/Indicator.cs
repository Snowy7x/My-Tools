using TMPro;
using UnityEngine;

namespace Systems.Weapon_System.Creator
{
    public class Indicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private Transform target;
        
        public void Init(Transform target, string text)
        {
            this.target = target;
            if (this.text) this.text.text = text;
        }
        
        private void Update()
        {
            if (target == null) return;
   transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}