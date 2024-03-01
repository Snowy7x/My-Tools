using Systems.Interaction_System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Systems.Editor
{
    [UnityEditor.CustomEditor(typeof(Interactable))]
    public class InteractionEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // un serialize the action object
            base.OnInspectorGUI();
            // if the action object has no serializeable fields, draw a box field under it
            if (serializedObject.FindProperty("action").hasVisibleChildren == false)
            {
                // Draw the box as a child of the action object
                UnityEditor.EditorGUILayout.BeginVertical(GUI.skin.box);
                UnityEditor.EditorGUILayout.LabelField("This action has no fields", UnityEditor.EditorStyles.boldLabel);
                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
    }
}