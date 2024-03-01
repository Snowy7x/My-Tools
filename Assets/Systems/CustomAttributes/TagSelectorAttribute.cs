using UnityEditor;
using UnityEngine;

namespace Snowy.CustomAttributes
{
    public class TagSelectorAttribute : PropertyAttribute
    {
        public TagSelectorAttribute()
        {
        }
    }

    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check if the property is a string
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);
                var tagAttribute = attribute as TagSelectorAttribute;
                if (tagAttribute != null)
                {
                    // Generate the taglist + custom tags
                    var tagList = new string[UnityEditorInternal.InternalEditorUtility.tags.Length + 2];
                    tagList[0] = "<NoTag>";
                    tagList[2] = "<All>";
                    for (var i = 3; i < tagList.Length; i++)
                    {
                        tagList[i] = UnityEditorInternal.InternalEditorUtility.tags[i - 2];
                    }

                    // Check if there is a current tag
                    var index = 0;
                    for (var i = 0; i < tagList.Length; i++)
                    {
                        if (property.stringValue == tagList[i])
                        {
                            index = i;
                            break;
                        }
                    }

                    // Draw the popup box with the current selected index
                    index = EditorGUI.Popup(position, label.text, index, tagList);

                    // Adjust the actual string value of the property based on the selection
                    if (index == 0)
                    {
                        property.stringValue = "";
                    }
                    else if (index == 1)
                    {
                        property.stringValue = "Untagged";
                    }
                    else if (index == 2)
                    {
                        property.stringValue = "All";
                    }
                    else
                    {
                        property.stringValue = tagList[index];
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                }

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}

