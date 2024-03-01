using UnityEditor;
using UnityEngine;

namespace Snowy.Inventory
{
    [UnityEditor.CustomEditor(typeof(PlayerInventory))]
    public class InventoryEditor : UnityEditor.Editor
    {
        public static readonly GUILayoutOption[] infoTextSize = new GUILayoutOption[] { GUILayout.Width(100f) };
        private static readonly GUILayoutOption[] deleteButtonSize = new GUILayoutOption[] { GUILayout.Width(70f) };
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var pInventory = (PlayerInventory) target;
            var inventory = pInventory.inventory;
            
            // Show the list of items in the inventory
            UnityEditor.EditorGUILayout.LabelField("Items");
            UnityEditor.EditorGUILayout.BeginVertical("box");
            if (inventory.inventorySlots.Count == 0)
            {
                UnityEditor.EditorGUILayout.LabelField("No items in inventory");
                
            }
            else
            {
                try
                {
                    foreach (var item in inventory.inventorySlots)
                    {
                        UnityEditor.EditorGUILayout.BeginHorizontal();
                        UnityEditor.EditorGUILayout.LabelField(item.Name, infoTextSize);
                        UnityEditor.EditorGUILayout.LabelField(item.amount.ToString(), infoTextSize);
                        if (GUILayout.Button("X-(ONE)", deleteButtonSize))
                            pInventory.RemoveItem(item);
                        if (GUILayout.Button("X-(ALL)", deleteButtonSize))
                            pInventory.RemoveItem(item, item.amount);

                        UnityEditor.EditorGUILayout.EndHorizontal();
                    }
                }
                catch
                {
                    // ignored
                }
            }

            UnityEditor.EditorGUILayout.EndVertical();
            
            
        }
    }
}