using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Systems.Interaction_System
{
    public class InteractActionsAttribute : PropertyAttribute
    {
        public Type[] types;
        
        public InteractActionsAttribute()
        {
            // Get all the types that implement the IInteractAction interface
            types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IInteractAction).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToArray();
        }
    }
    
    [CustomPropertyDrawer(typeof(InteractActionsAttribute))]
    public class InteractActionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check if the type is a Type
            if (property.propertyType == SerializedPropertyType.String)
            {
                var iAttribute = attribute as InteractActionsAttribute;
                if (iAttribute != null)
                {
                    EditorGUI.BeginProperty(position, label, property);
                    if (iAttribute.types == null) return;
                    var types = iAttribute.types;

                    var interactActions = new string[types.Length];

                    for (var i = 0; i < types.Length; i++)
                    {
                        interactActions[i] = types[i].Name;
                    }

                    // Check if there is a current tag
                    var index = 0;
                    for (var i = 0; i < interactActions.Length; i++)
                    {
                        if (property.stringValue == types[i].FullName)
                        {
                            index = i;
                            break;
                        }
                    }

                    // Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, interactActions);

                    // Set the type as a string to use in the Interactable class
                    property.stringValue = types[index].FullName;

                } else
                {
                    EditorGUI.PropertyField(position, property, label);
                }

                EditorGUI.EndProperty();
            } else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}