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
        [SerializeField] private float acceleration = 12f;     // exponential damping, fps-independent
        [SerializeField] private float turnSpeed = 16f;        // body rotation toward move direction
        [SerializeField] private float speedBlend = 8f;        // walk↔run ramp — no speed snaps
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 1.0f;

        [Header("Camera")]
        [SerializeField] public Camera lookCamera;   // public: wired directly by Phase0SceneBuilder
        [SerializeField] private float mouseSensitivity = 0.1f;

        [Header("Visuals")]
        public Transform visual;   // body mesh that turns toward movement; set by Kai-model tool, auto-created otherwise

        private CharacterController _cc;
        private Vector3 _planarVelocity;
        private float _verticalVelocity;
        private float _speedCur;            // blended walk/run speed (smooth sprint transitions)
        private float _lastMoveTime;        // idle re-facing delay after releasing keys
        private float _cameraPitch;
        private Bayani.Combat.CombatResources _resources;   // optional: sprint drains stamina (§56.2)

        /// <summary>PlayerCombat locks controls during attacks/dodges (phasing 1.4).</summary>
        public bool ControlsEnabled = true;

        /// <summary>Smoothed planar velocity — read by KaiWalkBob for bob/lean intensity.</summary>
        public Vector3 PlanarVelocity => _planarVelocity;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (lookCamera == null) lookCamera = Camera.main;
            _resources = GetComponent<Bayani.Combat.CombatResources>();

            // Visual/body separation: the graybox capsule's renderer sits ON this root, and a
            // root can't be reparented under its own child — so copy mesh+materials onto a
            // runtime "Visual" child and disable the original. Body facing then never touches root yaw.
            if (visual == null)
            {
                var r = GetComponent<Renderer>();
                if (r != null)
                {
                    var vis = new GameObject("Visual");
                    vis.transform.SetParent(transform, false);
                    var mf = GetComponent<MeshFilter>();
                    if (mf != null) vis.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
                    vis.AddComponent<MeshRenderer>().sharedMaterials = r.sharedMaterials;
                    r.enabled = false;
                    visual = vis.transform;
                }
                else visual = transform;   // no renderer at all — fall back to root rotation
            }
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
            // Ramp toward target speed instead of snapping when sprint starts/stops
            _speedCur = Mathf.Lerp(_speedCur, speed, 1f - Mathf.Exp(-speedBlend * Time.deltaTime));

            // Camera-relative movement direction
            float camYaw = lookCamera != null ? lookCamera.transform.eulerAngles.y : transform.eulerAngles.y;
            Quaternion facing = Quaternion.Euler(0f, camYaw, 0f);
            Vector3 desired = facing * new Vector3(move.x, 0f, move.y).normalized;
            bool moving = move.sqrMagnitude > 0.001f;
            // Analog tilt: partial gamepad-stick push walks slower (keyboard magnitude is always 1)
            Vector3 targetVel = desired * (moving ? _speedCur * Mathf.Clamp01(move.magnitude) : 0f);
            // Frame-rate-independent exponential smoothing
            _planarVelocity = Vector3.Lerp(_planarVelocity, targetVel, 1f - Mathf.Exp(-acceleration * Time.deltaTime));

            // Body facing lives on the VISUAL child, not the root — root yaw is mouse-only,
            // so strafing/backward can't drag the camera through a rotation feedback loop.
            if (moving && ControlsEnabled)
            {
                visual.rotation = Quaternion.Slerp(visual.rotation, Quaternion.LookRotation(desired),
                    1f - Mathf.Exp(-turnSpeed * Time.deltaTime));
                Vector3 e = visual.eulerAngles;
                visual.rotation = Quaternion.Euler(0f, e.y, 0f);        // yaw only, stay upright
            }
            else if (ControlsEnabled && Time.time - _lastMoveTime > 0.15f)
            {
                // idle: settle back to camera-facing
                Quaternion face = Quaternion.Euler(0f, camYaw, 0f);
                visual.rotation = Quaternion.Slerp(visual.rotation, face,
                    1f - Mathf.Exp(-turnSpeed * 0.6f * Time.deltaTime));
            }
            if (moving) _lastMoveTime = Time.time;

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
