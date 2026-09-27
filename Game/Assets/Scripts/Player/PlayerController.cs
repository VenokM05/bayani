// BAYANI — Phase 0 minimal player controller (graybox prototype)
// Purpose: satisfy the Phase 0 exit gate — "capsule moves with WASD + mouse camera".
// Uses Unity's New Input System (installed: com.unity.inputsystem 1.18.0).
// Full combat, Stamina/Diwa, and PlayerInput/gamepad mapping arrive in Phase 1 (docs/phasing.md).
//
// SETUP: add a CharacterController component to the "Kai" capsule, then attach this script.
// Assign the Main Camera to `lookCamera` (or leave empty — it auto-finds Camera.main).
// Requires: ProjectSettings > Player Settings > Active Input Handling = "Both" or "Input System Package (new)".

using UnityEngine;
using UnityEngine.InputSystem;

namespace Bayani.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement (arcade feel — ggd §56.8 / phasing 1.1)")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float runSpeed = 5f;          // ~5 units/s per phasing.md 1.1
        [SerializeField] private float acceleration = 12f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.0f;

        [Header("Camera")]
        [SerializeField] public Camera lookCamera;   // public: wired directly by Phase0SceneBuilder
        [SerializeField] private float mouseSensitivity = 0.1f;

        private CharacterController _cc;
        private Vector3 _planarVelocity;
        private float _verticalVelocity;
        private float _cameraPitch;
        private Bayani.Combat.CombatResources _resources;   // optional: sprint drains stamina (§56.2)

        /// <summary>PlayerCombat locks controls during attacks/dodges (phasing 1.4).</summary>
        public bool ControlsEnabled = true;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (lookCamera == null) lookCamera = Camera.main;
            _resources = GetComponent<Bayani.Combat.CombatResources>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;   // clean mouse look for the prototype
            Cursor.visible = false;
        }

        private void Update()
        {
            Vector2 move = ControlsEnabled ? Bayani.Core.BayaniInput.Move : Vector2.zero;

            bool running = ControlsEnabled && Bayani.Core.BayaniInput.Sprint && move.sqrMagnitude > 0.01f;
            // Sprint costs 10 stamina/s (ggd §56.2); no sprint when the meter is empty
            if (running && _resources != null)
            {
                if (_resources.Stamina <= 0f) running = false;
                else _resources.SpendStamina(10f * Time.deltaTime);
            }
            float speed = running ? runSpeed : walkSpeed;

            // Camera-relative movement direction
            float camYaw = lookCamera != null ? lookCamera.transform.eulerAngles.y : transform.eulerAngles.y;
            Quaternion facing = Quaternion.Euler(0f, camYaw, 0f);
            Vector3 desired = facing * new Vector3(move.x, 0f, move.y).normalized;
            Vector3 targetVel = desired * (move.sqrMagnitude > 0.01f ? speed : 0f);
            _planarVelocity = Vector3.Lerp(_planarVelocity, targetVel, acceleration * Time.deltaTime);

            // Rotate body toward movement direction
            if (_planarVelocity.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(_planarVelocity), Time.deltaTime * 10f);
                transform.rotation = look;
            }

            // Grounded gravity + jump (jump suppressed while combat locks controls)
            if (_cc.isGrounded)
            {
                _verticalVelocity = -1f;
                if (ControlsEnabled && Bayani.Core.BayaniInput.JumpPressed)
                    _verticalVelocity = Mathf.Sqrt(2f * -gravity * jumpHeight);
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 motion = _planarVelocity + Vector3.up * _verticalVelocity;
            _cc.Move(motion * Time.deltaTime);

            HandleLook();
        }

        private void HandleLook()
        {
            if (lookCamera == null) return;
            Vector2 delta = Bayani.Core.BayaniInput.Look * mouseSensitivity;
            transform.Rotate(0f, delta.x, 0f);                       // yaw the player
            _cameraPitch = Mathf.Clamp(_cameraPitch - delta.y, -30f, 70f); // pitch the camera
        }
    }
}
