using UnityEditor;
using UnityEngine;

namespace Snowy.CustomAttributes
{
    public class MinMaxAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        public MinMaxAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
    
    // Drawer
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // if property is not a float return
            if (property.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(position, label.text, "Use MinMax with float.");
                return;
            }
            
            // Get the min and max values from the attribute
            MinMaxAttribute minMax = attribute as MinMaxAttribute;
            float minValue = minMax.min;
            float maxValue = minMax.max;
            
            // Draw the property like a slider
            EditorGUI.BeginChangeCheck();
            float newValue = EditorGUI.Slider(position, label, property.floatValue, minValue, maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = newValue;
            }
        }
    }
}