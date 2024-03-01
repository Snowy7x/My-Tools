using System;
using UnityEditor;
using UnityEngine;

namespace Snowy.CustomAttributes
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public string triggerField;
        public bool hideTriggerField;

        public ReadOnlyAttribute(string triggerField = "", bool hide = false)
        {
            this.triggerField = triggerField;
            hideTriggerField = hide;
        }
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
            GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute) attribute;
            // Check if the property we want to draw should be read only
            bool isReadOnly = false;
            if (readOnlyAttribute.triggerField != "")
            {
                // Get the property that should trigger read only
                SerializedProperty triggerProperty =
                    property.serializedObject.FindProperty(readOnlyAttribute.triggerField);
                    
                if (triggerProperty != null)
                {
                    // Get the value of the trigger property
                    bool triggerPropertyValue = triggerProperty.boolValue;
                    // If the triggering property is true, set readonly to true
                    isReadOnly = triggerPropertyValue;
                }
            }
            
            // Set GUI.enabled to the opposite value of isReadOnly
            GUI.enabled = !isReadOnly;
            // Draw the property as a normal property field
            EditorGUI.PropertyField(position, property, label, true);
            // Ensure GUI.enabled is reset to true after the property is drawn
            GUI.enabled = true;
        }
    }

}