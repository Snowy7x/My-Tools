using System;
using System.Collections;
using Snowy.Input;
using UnityEngine;

namespace Systems.FPS_Movement.Scripts
{
    public class FpCamera : SuperCamera
    {
        [Header("References")]
        [SerializeField] private Player player;
        [SerializeField] private Camera cam;
        [SerializeField] private Transform playerBody;
        [SerializeField] private BasicMovement movement;
        
        [Space]
        [Header("Full Body")]
        public bool hasBody = true;
        public bool useFullBody = false;
        public float fullBodyMaxAngle = 60f;
        public Transform spineTarget;
        public Transform headTarget;
        
        [Space(10)]
        [Header("Camera")]
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float yAngleClamp = 90f;
        [SerializeField] private float defaultFOV = 60f;

        [Space(10)] 
        [Header("Bobbing")]
        [SerializeField] private float bobbingSpeed = 14f;
        [SerializeField] private float bobbingAmount = 0.05f;
        private float mouseX;
        private float mouseY;
        
        public event Action OnCameraUpdate;
        private Vector3 _defaultPivotRot;
        private Vector3 _defaultHeadRot;
        #region private vars
        
        private float _xRotation;
        private bool _isFovChanged;
        private float _targetFov;
        private float _currentFov;
        private float _timer;
        private float _midpoint;
        
        Vector3 _defaultCamRot;

        #endregion

        protected override void Start()
        {
            if (!playerBody) playerBody = transform.parent;
            if (!cam) cam = GetComponentInChildren<Camera>();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            cam.fieldOfView = defaultFOV;
            _midpoint = transform.localPosition.y;
            
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
            
            if (!player) player = GetComponentInParent<Player>();
            if (!movement) movement = GetComponentInParent<BasicMovement>();
            
            _defaultPivotRot = spineTarget.localRotation.eulerAngles;
            _defaultHeadRot = headTarget.localRotation.eulerAngles;

            if (player && player.Inventory)
            {
                player.Inventory.OnCurrentSlotChanged += (slot) =>
                {
                    if (!hasBody) return; 
                    useFullBody = player.Inventory.currentItem;
                    if (!useFullBody)
                    {
                        spineTarget.localRotation = Quaternion.Euler(_defaultPivotRot);
                    }else
                    {
                        spineTarget.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                    }
                };
            }
            base.Start();
        }

        protected override void Initialize(InputManager input)
        {
            base.Initialize(input);
            input.OnLook += Look;
        }

        private void Look(Vector2 lookInput) 
        {
            mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        }

        public void ChangeFOV(float target, float transition)
        {
            if (_isFovChanged && Math.Abs(target - _targetFov) < 0.1f) return;
            if (Math.Abs(cam.fieldOfView - target) < 0.1f) return;
            StartCoroutine(ChangeFOVCoroutine(target, transition));
        } 
        
        IEnumerator ChangeFOVCoroutine(float target, float transition)
        {
            _isFovChanged = true;
            _targetFov = target;
            _currentFov = cam.fieldOfView;
            
            float time = 0f;
            while (time < transition)
            {
                time += Time.deltaTime;
                cam.fieldOfView = Mathf.Lerp(_currentFov, _targetFov, time / transition);
                yield return null;
            }
            
            _isFovChanged = false;
        }
        
        public void ZoomIn(float zoom, float transition)
        {
            float target = zoom;
            ChangeFOV(target, transition);
        }
        
        public void ZoomOut(float transition)
        {
            ChangeFOV(defaultFOV, transition);
        }

        private void Update()
        {
            _xRotation -= mouseY;
            float clampAngle = useFullBody ? fullBodyMaxAngle : yAngleClamp;
            _xRotation = Mathf.Clamp(_xRotation, -clampAngle, clampAngle);

            if (hasBody)
            {
                headTarget.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                if (useFullBody)
                    spineTarget.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            }
            else
                transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            
            playerBody.Rotate(Vector3.up * mouseX);
            
            // Bobbing:
            Vector3 localPosition = transform.localPosition;
            if (movement.IsMoving() && movement.IsGrounded())
            {
                Vector3 oldPos = transform.localPosition;
                
                // Bobbing on the Vector3.up axis
                _timer += Time.deltaTime * bobbingSpeed;
                Vector3 newPos = new Vector3(oldPos.x,
                    Mathf.Sin(_timer) * bobbingAmount + _midpoint, oldPos.z);
                transform.localPosition = newPos;
            }
            else
            {
                _timer = 0;
                transform.localPosition = new Vector3(localPosition.x,
                    Mathf.Lerp(localPosition.y, _midpoint, bobbingSpeed * Time.deltaTime), localPosition.z);
            }
            
            OnCameraUpdate?.Invoke();
        }
        
        [ContextMenu("Switch Cam mode")]
        public void SwitchCamMode()
        {
            useFullBody = !useFullBody;
            if (!useFullBody)
            {
                spineTarget.localRotation = Quaternion.Euler(_defaultPivotRot);
            }
            else
            {
                spineTarget.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            }
        }
    }
}