using System;
using Snowy.CustomAttributes;
using UnityEditor;
using UnityEngine;

namespace Systems.Weapon_System.Creator.Attempt_01
{
    [Serializable]
    public struct CameraSettings
    {
        public float fov;
        [MinMax(-180, 180)] public float rotationX;
        [MinMax(-180, 180)] public float rotationY;
        [MinMax(-180, 180)] public float rotationZ;
    
        public Quaternion GetRotation()
        {
            return Quaternion.Euler(rotationX, rotationY, rotationZ);
        }
    }

    public struct FingerOpen
    {
        [MinMax(-0.2f, 0.2f)] public float x;
        [MinMax(-0.2f, 0.2f)] public float y;
        [MinMax(-0.2f, 0.2f)] public float z;
    
        public Vector3 tipRotation;
    }


    [Serializable]public class HandSettings
    {
        public float positionX;
        public float positionY;
        public float positionZ;
        [Space]
        [MinMax(-180, 180)] public float rotationX;
        [MinMax(-180, 180)] public float rotationY;
        [MinMax(-180, 180)] public float rotationZ;
    
        public FingerSettings thumb = new FingerSettings();
        public FingerSettings index = new FingerSettings();
        public FingerSettings middle = new FingerSettings();
        public FingerSettings ring = new FingerSettings();
        public FingerSettings pinky = new FingerSettings();
    
        public Vector3 GetPosition()
        {
            return new Vector3(positionX, positionY, positionZ);
        }
    
        public Quaternion GetRotation()
        {
            return Quaternion.Euler(rotationX, rotationY, rotationZ);
        }
    }
    
    public class WeaponCreator : EditorWindow
    {
        public GameObject weaponPrefab;
        bool showHandSettings;
        public HandSettings leftHandSettings;
        public HandSettings rightHandSettings;

        public CameraSettings cameraSettings = new CameraSettings
        {
            fov = 3f
        };
        WeaponCreatorHelper weaponCreatorHelper;
        
        bool foldRightHand;
        private bool foldRhPos;
        private bool foldRhRot;
        private bool foldRhFingers;
        private bool foldRhThumb;
        private bool foldRhIndex;
        private bool foldRhMiddle;
        private bool foldRhRing;
        private bool foldRhPinky;
        bool foldLeftHand;
        private bool foldLhPos;
        private bool foldLhRot;
        private bool foldLhFingers;
        private bool foldLhThumb;
        private bool foldLhIndex;
        private bool foldLhMiddle;
        private bool foldLhRing;
        private bool foldLhPinky;
        bool foldCamera;
        Vector2 scrollPos;
        
        // Add menu named "My Window" to the Window menu
        [MenuItem("Snowy/Weapon Creator")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            WeaponCreator window = (WeaponCreator)EditorWindow.GetWindow(typeof(WeaponCreator));
            window.Show();
        }

        [Obsolete("Obsolete")]
        private void OnBecameVisible()
        {
            // Open the CreatorScene
            // Check if the scene is already open
            if (EditorApplication.currentScene != "Assets/Systems/Weapon System/Creator/CreatorScene.unity")
            {
                showHandSettings = false;
                return;
            }
            
            // Get the WeaponCreatorHelper
            weaponCreatorHelper = FindObjectOfType<WeaponCreatorHelper>();
            if (!weaponCreatorHelper)
            {
                showHandSettings = false;
                return;
            }
            
            if (weaponCreatorHelper.currentWeaponPrefab)
            {
                if (weaponCreatorHelper.currentWeapon == null)
                {
                    showHandSettings = false;
                    return;
                }
                
                LoadFromHelper();
                showHandSettings = true;
                return;
            }
            
            showHandSettings = true;
        }

        [Obsolete("Obsolete")]
        void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos,false,false);
            Undo.RecordObject(this, "Weapon Creator");
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            weaponPrefab = (GameObject) EditorGUILayout.ObjectField("Weapon Prefab", weaponPrefab, typeof(GameObject), true);
            
            // Preview the weapon
            if (weaponPrefab)
            {

                if (showHandSettings && EditorApplication.currentScene == "Assets/Systems/Weapon System/Creator/CreatorScene.unity")
                {

                    GUILayout.Label("Cam Settings", EditorStyles.boldLabel);
                    // Camera settings
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    foldCamera = EditorGUILayout.Foldout(foldCamera, "Camera");
                    if (foldCamera)
                    {
                        cameraSettings.fov = EditorGUILayout.Slider("FOV", cameraSettings.fov, 1, 180);
                        cameraSettings.rotationX =
                            EditorGUILayout.Slider("Rotation X", cameraSettings.rotationX, -180, 180);
                        cameraSettings.rotationY =
                            EditorGUILayout.Slider("Rotation Y", cameraSettings.rotationY, -180, 180);
                        cameraSettings.rotationZ =
                            EditorGUILayout.Slider("Rotation Z", cameraSettings.rotationZ, -180, 180);
                        
                        if (GUILayout.Button("Reset"))
                        {
                            cameraSettings = new CameraSettings
                            {
                                fov = 3f
                            };
                        }
                    }
                    EditorGUILayout.EndVertical();
                    
                    weaponCreatorHelper.UpdateCamera(cameraSettings);
                    
                    // Draw the hand settings as sliders
                    GUILayout.Label("Hand Settings", EditorStyles.boldLabel);
                    // Left hand in group/box fold
                    
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    foldLeftHand = EditorGUILayout.Foldout(foldLeftHand, "Left Hand");
                    if (foldLeftHand)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            // Position
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            foldLhPos = EditorGUILayout.Foldout(foldLhPos, "Position");
                            if (foldLhPos)
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    leftHandSettings.positionX =
                                        EditorGUILayout.Slider("Position X", leftHandSettings.positionX, -5, 5);
                                    leftHandSettings.positionY =
                                        EditorGUILayout.Slider("Position Y", leftHandSettings.positionY, -5, 5);
                                    leftHandSettings.positionZ =
                                        EditorGUILayout.Slider("Position Z", leftHandSettings.positionZ, -5, 5);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        
                            // Rotation
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            foldLhRot = EditorGUILayout.Foldout(foldLhRot, "Rotation");
                            if (foldLhRot)
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    leftHandSettings.rotationX =
                                        EditorGUILayout.Slider("Rotation X", leftHandSettings.rotationX, -180, 180);
                                    leftHandSettings.rotationY =
                                        EditorGUILayout.Slider("Rotation Y", leftHandSettings.rotationY, -180, 180);
                                    leftHandSettings.rotationZ =
                                        EditorGUILayout.Slider("Rotation Z", leftHandSettings.rotationZ, -180, 180);
                                }
                            }
                            EditorGUILayout.EndVertical();
                            
                            // Fingers
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            foldLhFingers = EditorGUILayout.Foldout(foldLhFingers, "Fingers");
                            if (foldLhFingers)
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldLhThumb = EditorGUILayout.Foldout(foldLhThumb, "Thumb");
                                    if (foldLhThumb)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            leftHandSettings.thumb.openAmount =
                                                EditorGUILayout.Slider("Open Amount", leftHandSettings.thumb.openAmount, -0.2f, 1.2f);
                                            leftHandSettings.thumb.distance =
                                                EditorGUILayout.Slider("Distance", leftHandSettings.thumb.distance, -0.2f, 0.2f);
                                            leftHandSettings.thumb.power =
                                                EditorGUILayout.Slider("Power", leftHandSettings.thumb.power, -0.2f, 0.2f);
                                            
                                            leftHandSettings.thumb.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", leftHandSettings.thumb.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldLhIndex = EditorGUILayout.Foldout(foldLhIndex, "Index");
                                    if (foldLhIndex)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            leftHandSettings.index.openAmount =
                                                EditorGUILayout.Slider("Open Amount", leftHandSettings.index.openAmount, -0.2f, 0.2f);
                                            leftHandSettings.index.distance =
                                                EditorGUILayout.Slider("Distance", leftHandSettings.index.distance, -0.2f, 0.2f);
                                            leftHandSettings.index.power =
                                                EditorGUILayout.Slider("Power", leftHandSettings.index.power, -0.2f, 0.2f);
                                            
                                            leftHandSettings.index.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", leftHandSettings.index.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldLhMiddle = EditorGUILayout.Foldout(foldLhMiddle, "Middle");
                                    if (foldLhMiddle)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            leftHandSettings.middle.openAmount =
                                                EditorGUILayout.Slider("Open Amount", leftHandSettings.middle.openAmount, -0.2f, 0.2f);
                                            leftHandSettings.middle.distance =
                                                EditorGUILayout.Slider("Distance", leftHandSettings.middle.distance, -0.2f, 0.2f);
                                            leftHandSettings.middle.power =
                                                EditorGUILayout.Slider("Power", leftHandSettings.middle.power, -0.2f, 0.2f);
                                            
                                            leftHandSettings.middle.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", leftHandSettings.middle.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldLhRing = EditorGUILayout.Foldout(foldLhRing, "Ring");
                                    if (foldLhRing)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            leftHandSettings.ring.openAmount =
                                                EditorGUILayout.Slider("Open Amount", leftHandSettings.ring.openAmount, -0.2f, 0.2f);
                                            leftHandSettings.ring.power =
                                                EditorGUILayout.Slider("Distance", leftHandSettings.ring.power, -0.2f, 0.2f);
                                            leftHandSettings.ring.distance =
                                                EditorGUILayout.Slider("Power", leftHandSettings.ring.distance, -0.2f, 0.2f);
                                            
                                            leftHandSettings.ring.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", leftHandSettings.ring.tipRotation);
                                        }
                                    }
                                    
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldLhPinky = EditorGUILayout.Foldout(foldLhPinky, "Pinky");
                                    if (foldLhPinky)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            leftHandSettings.pinky.openAmount =
                                                EditorGUILayout.Slider("Open Amount", leftHandSettings.pinky.openAmount, -0.2f, 0.2f);
                                            leftHandSettings.pinky.distance =
                                                EditorGUILayout.Slider("Distance", leftHandSettings.pinky.distance, -0.2f, 0.2f);
                                            leftHandSettings.pinky.power =
                                                EditorGUILayout.Slider("Power", leftHandSettings.pinky.power, -0.2f, 0.2f);
                                            
                                            leftHandSettings.pinky.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", leftHandSettings.pinky.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            
                            EditorGUILayout.EndVertical();
                        }
                        
                        if (GUILayout.Button("Reset Left Hand")) 
                        {
                            leftHandSettings = new HandSettings();
                        }
                    }

                    EditorGUILayout.EndVertical();
                    
                    // Right hand in group/box
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    foldRightHand = EditorGUILayout.Foldout(foldRightHand, "Right Hand");
                    if (foldRightHand)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            // Position
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            foldRhPos = EditorGUILayout.Foldout(foldRhPos, "Position");
                            if (foldRhPos)
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    rightHandSettings.positionX =
                                        EditorGUILayout.Slider("Position X", rightHandSettings.positionX, -5, 5);
                                    rightHandSettings.positionY =
                                        EditorGUILayout.Slider("Position Y", rightHandSettings.positionY, -5, 5);
                                    rightHandSettings.positionZ =
                                        EditorGUILayout.Slider("Position Z", rightHandSettings.positionZ, -5, 5);
                                }
                            }

                            EditorGUILayout.EndVertical();

                            // Rotation
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            foldRhRot = EditorGUILayout.Foldout(foldRhRot, "Rotation");
                            if (foldRhRot)
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    rightHandSettings.rotationX =
                                        EditorGUILayout.Slider("Rotation X", rightHandSettings.rotationX, -180, 180);
                                    rightHandSettings.rotationY =
                                        EditorGUILayout.Slider("Rotation Y", rightHandSettings.rotationY, -180, 180);
                                    rightHandSettings.rotationZ =
                                        EditorGUILayout.Slider("Rotation Z", rightHandSettings.rotationZ, -180, 180);
                                }
                            }

                            EditorGUILayout.EndVertical();
                            
                            // Fingers
                            EditorGUILayout.BeginVertical(GUI.skin.box);
                            foldRhFingers = EditorGUILayout.Foldout(foldRhFingers, "Fingers");
                            if (foldRhFingers)
                            {
                                using (new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldRhThumb = EditorGUILayout.Foldout(foldRhThumb, "Thumb");
                                    if (foldRhThumb)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            rightHandSettings.thumb.openAmount =
                                                EditorGUILayout.Slider("Open Amount", rightHandSettings.thumb.openAmount, -0.2f, 1.2f);
                                            rightHandSettings.thumb.distance =
                                                EditorGUILayout.Slider("Distance", rightHandSettings.thumb.distance, -0.2f, 0.2f);
                                            rightHandSettings.thumb.power =
                                                EditorGUILayout.Slider("Power", rightHandSettings.thumb.power, -0.2f, 0.2f);
                                            
                                            rightHandSettings.thumb.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", rightHandSettings.thumb.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldRhIndex = EditorGUILayout.Foldout(foldRhIndex, "Index");
                                    if (foldRhIndex)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            rightHandSettings.index.openAmount =
                                                EditorGUILayout.Slider("Open Amount", rightHandSettings.index.openAmount, -0.2f, 0.2f);
                                            rightHandSettings.index.distance =
                                                EditorGUILayout.Slider("Distance", rightHandSettings.index.distance, -0.2f, 0.2f);
                                            rightHandSettings.index.power =
                                                EditorGUILayout.Slider("Power", rightHandSettings.index.power, -0.2f, 0.2f);
                                            
                                            rightHandSettings.index.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", rightHandSettings.index.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldRhMiddle = EditorGUILayout.Foldout(foldRhMiddle, "Middle");
                                    if (foldRhMiddle)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            rightHandSettings.middle.openAmount =
                                                EditorGUILayout.Slider("Open Amount", rightHandSettings.middle.openAmount, -0.2f, 0.2f);
                                            rightHandSettings.middle.distance =
                                                EditorGUILayout.Slider("Distance", rightHandSettings.middle.distance, -0.2f, 0.2f);
                                            rightHandSettings.middle.power =
                                                EditorGUILayout.Slider("Power", rightHandSettings.middle.power, -0.2f, 0.2f);
                                            
                                            rightHandSettings.middle.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", rightHandSettings.middle.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldRhRing = EditorGUILayout.Foldout(foldRhRing, "Ring");
                                    if (foldRhRing)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            rightHandSettings.ring.openAmount =
                                                EditorGUILayout.Slider("Open Amount", rightHandSettings.ring.openAmount, -0.2f, 0.2f);
                                            rightHandSettings.ring.distance =
                                                EditorGUILayout.Slider("Distance", rightHandSettings.ring.distance, -0.2f, 0.2f);
                                            rightHandSettings.ring.power =
                                                EditorGUILayout.Slider("Power", rightHandSettings.ring.power, -0.2f, 0.2f);
                                            
                                            rightHandSettings.ring.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", rightHandSettings.ring.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                    
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    foldRhPinky = EditorGUILayout.Foldout(foldRhPinky, "Pinky");
                                    if (foldRhPinky)
                                    {
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            rightHandSettings.pinky.openAmount =
                                                EditorGUILayout.Slider("Open Amount", rightHandSettings.pinky.openAmount, -0.2f, 0.2f);
                                            rightHandSettings.pinky.distance =
                                                EditorGUILayout.Slider("Distance", rightHandSettings.pinky.distance, -0.2f, 0.2f);
                                            rightHandSettings.pinky.power =
                                                EditorGUILayout.Slider("Power", rightHandSettings.pinky.power, -0.2f, 0.2f);
                                            
                                            rightHandSettings.pinky.tipRotation = EditorGUILayout.Vector3Field("Tip Rotation", rightHandSettings.pinky.tipRotation);
                                        }
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    
                    if (GUILayout.Button("Reset Fingers")) 
                    {
                        weaponCreatorHelper.ResetFingers();
                    }

                    EditorGUILayout.EndVertical();
                    
                    weaponCreatorHelper.UpdateRightHandSettings(rightHandSettings);
                    weaponCreatorHelper.UpdateLeftHandSettings(leftHandSettings);
                    
                }
                
                if (GUILayout.Button("Setup Weapon"))
                {
                    // Open the CreatorScene
                    // Check if the scene is already open
                    if (EditorApplication.currentScene != "Assets/Systems/Weapon System/Creator/CreatorScene.unity")
                    {
                        EditorApplication.OpenScene("Assets/Systems/Weapon System/Creator/CreatorScene.unity");
                    }
                    
                    // Get the WeaponCreatorHelper
                    weaponCreatorHelper = FindObjectOfType<WeaponCreatorHelper>();
                    
                    // Set the weapon prefab
                    weaponCreatorHelper.CreateWeaponSetup(weaponPrefab);
                    showHandSettings = true;
                    
                    leftHandSettings = new HandSettings();
                    rightHandSettings = new HandSettings();
                }
                
                
                // Record for undo
            }
            
            GUILayout.EndScrollView();
            
        }

        private void LoadFromHelper()
        {
            Debug.Log("Loading from helper");
            weaponPrefab = weaponCreatorHelper.currentWeaponPrefab;
            leftHandSettings = weaponCreatorHelper.GetLeftHandSettings();
            rightHandSettings = weaponCreatorHelper.GetRightHandSettings();
            cameraSettings = weaponCreatorHelper.GetCameraSettings();
        }
    }
}