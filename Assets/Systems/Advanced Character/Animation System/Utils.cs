using System;
using Snowy.CustomAttributes;
using UnityEngine;

namespace Systems.Advanced_Character.Animation_System
{
    [Serializable] public enum AnimationLayer
    {
        Base,
        Override
    }
    
    [Serializable] public class AnimationState {
        [ReadOnly("nameReadOnly", true)] public string stateName;
        public AnimationClip animationClip;
        public bool loop = true;
        public float animationSpeed = 1f;
        public AnimationLayer animationLayer = AnimationLayer.Base;
        public float animationTransitionTime = 0.15f;
        [HideInInspector] public int animationLayerIndex => animationLayer == AnimationLayer.Base ? 0 : 1;

        public AnimationState(string stateName = "", bool nameReadOnly = false)
        {
            this.stateName = stateName;
        }
    }
    
    
}