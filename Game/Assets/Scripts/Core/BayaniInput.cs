// BAYANI — Shared input layer: keyboard+mouse AND gamepad, live-polled each frame.
// Both devices work simultaneously; whichever the player touches wins.
// Mappings (ggd §41 subset): attack LMB/LT · block+parry hold RMB / hold X ·
// dodge LCtrl / RT · skill Q / Y · ACTIVE SKILLS 1–5 / dpad+east · ready-stow weapon G ·
// jump Space / A · sprint Shift / L3 · move WASD / stick.
// Later (Phase 2) this can become PlayerInput actions without touching gameplay code.

using UnityEngine;
using UnityEngine.InputSystem;

namespace Bayani.Core
{
    public static class BayaniInput
    {
        private const float StickDeadzone = 0.15f;

        public static Vector2 Move
        {
            get
            {
                var v = Vector2.zero;
                var kb = Keyboard.current;
                if (kb != null)
                    v += new Vector2(
                        (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f),
                        (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f));
                var gp = Gamepad.current;
                if (gp != null) v += Deadzone(gp.leftStick.ReadValue());
                return Vector2.ClampMagnitude(v, 1f);
            }
        }

        public static Vector2 Look
        {
            get
            {
                if (Mouse.current != null)
                {
                    var d = Mouse.current.delta.ReadValue();
                    if (d.sqrMagnitude > 0.01f) return d;
                }
                var gp = Gamepad.current;
                if (gp != null) return Deadzone(gp.rightStick.ReadValue()) * 12f;   // ~120°/s at sens 0.1
                return Vector2.zero;
            }
        }

        // Sprint: Shift, or left stick pushed to (near) full tilt — standard action-game run.
        public static bool Sprint =>
            (Keyboard.current?.leftShiftKey.isPressed ?? false) ||
            ((Gamepad.current?.leftStick.ReadValue().magnitude ?? 0f) >= 0.9f);

        public static bool JumpPressed =>
            (Keyboard.current?.spaceKey.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false);

        public static bool AttackPressed =>
            (Mouse.current?.leftButton.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.leftTrigger.wasPressedThisFrame ?? false);

        public static bool DodgePressed =>
            (Keyboard.current?.leftCtrlKey.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.rightTrigger.wasPressedThisFrame ?? false);

        public static bool BlockPressed =>
            (Mouse.current?.rightButton.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonWest.wasPressedThisFrame ?? false);

        public static bool BlockHeld =>
            (Mouse.current?.rightButton.isPressed ?? false) ||
            (Gamepad.current?.buttonWest.isPressed ?? false);

        public static bool SkillPressed =>
            (Keyboard.current?.qKey.wasPressedThisFrame ?? false) ||   // was dKey — bug: D is strafe-right
            (Gamepad.current?.buttonNorth.wasPressedThisFrame ?? false);

        // Active skill slots: number keys 1–5 (docs/prd-progression.md §3).
        // i is 0-based; gamepad uses dpad up/right/left/down + buttonEast fallback.
        // Verified against the installed Input System (Unity 6 build): the properties are
        // digit1Key..digit5Key and the enum is Key.DigitN (singular) — NOT digitKeyN/Keys.
        public static bool SkillSlotPressed(int i)
        {
            var kb = Keyboard.current;
            var gp = Gamepad.current;
            return i switch
            {
                0 => (kb?.digit1Key.wasPressedThisFrame ?? false) || (gp?.dpad.up.wasPressedThisFrame ?? false),
                1 => (kb?.digit2Key.wasPressedThisFrame ?? false) || (gp?.dpad.right.wasPressedThisFrame ?? false),
                2 => (kb?.digit3Key.wasPressedThisFrame ?? false) || (gp?.dpad.left.wasPressedThisFrame ?? false),
                3 => (kb?.digit4Key.wasPressedThisFrame ?? false) || (gp?.dpad.down.wasPressedThisFrame ?? false),
                4 => (kb?.digit5Key.wasPressedThisFrame ?? false) || (gp?.buttonEast.wasPressedThisFrame ?? false),
                _ => false,
            };
        }

        // Ready / stow the weapon — flips the armed/unarmed skill bar (PRD §4).
        public static bool WeaponTogglePressed =>
            (Keyboard.current?.gKey.wasPressedThisFrame ?? false);

        // Interact / pick up / dialogue advance: E or F / Enter / gamepad A (no combat use).
        public static bool InteractPressed =>
            (Keyboard.current?.eKey.wasPressedThisFrame ?? false) ||
            (Keyboard.current?.fKey.wasPressedThisFrame ?? false) ||
            (Keyboard.current?.enterKey.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false);

        private static Vector2 Deadzone(Vector2 v) =>
            v.sqrMagnitude >= StickDeadzone * StickDeadzone ? v : Vector2.zero;
    }
}
