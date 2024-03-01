using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Systems.IK.Base
{
    [ExecuteInEditMode]
    public class BaseIK : MonoBehaviour
    {
        public event Action OnChainIKResolved;
        public event Action OnLookIKResolved;
        public event Action OnFollowIKResolved;
        public event Action OnIKResolved;
        public List<ChainIK> chains = new List<ChainIK>();
        public LookChain[] lookChains = new LookChain[0];
        public FollowTarget[] followTargets = new FollowTarget[0];
        
        [Header("Debug")] public bool debug = false;
        
        void Awake()
        {
            
            foreach (var chain in chains)
            {
                chain.Init();
            }
            
            
            foreach (var chain in followTargets)
            {
                chain.Init();
            }
            
            foreach (var chain in lookChains)
            {
                chain.Init();
            }
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying && !debug) return;
            
            foreach (var chain in lookChains)
            {
                chain.Resolve();
            }
            
            OnLookIKResolved?.Invoke();

            foreach (var chain in followTargets)
            {
                chain.Resolve();
            }
            
            OnFollowIKResolved?.Invoke();
            
            foreach (var chain in chains)
            {
                chain.ResolveIK();
            }
            
            OnChainIKResolved?.Invoke();
            OnIKResolved?.Invoke();
        }
    }
}