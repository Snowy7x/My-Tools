﻿// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Editor.UIManager.Windows;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UISliderId), true)]
    public class UISliderIdDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            CategoryNameIdUtils.CreateDrawer
            (
                property,
                () => UISliderIdDatabase.instance.database.GetCategories(),
                targetCategory => UISliderIdDatabase.instance.database.GetNames(targetCategory),
                EditorSpriteSheets.UIManager.Icons.UISliderDatabase,
                SlidersDatabaseWindow.Open,
                "Open Sliders Database Window",
                UISliderIdDatabase.instance,
                EditorSelectableColors.UIManager.UIComponent
            );
    }
}