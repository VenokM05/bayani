// BAYANI — Item data (docs/prd-progression.md §9, inventory skeleton).
// One SO per item; the inventory stores references to these assets. Icons are
// optional while the project is graybox — slots fall back to the item initials.
// Stat bonuses apply ONCE when the item first enters the inventory (skeleton:
// no unequip path yet, so no removal step).

using UnityEngine;

namespace Bayani.Player
{
    [CreateAssetMenu(menuName = "BAYANI/Item Data", fileName = "ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemName = "River Stone";
        public Sprite icon;                    // null = graybox letter tile
        [TextArea] public string description = "";

        [Header("Stacking")]
        public bool stackable = true;
        public int maxStack = 5;

        [Header("Stat modifiers (applied once on first pickup; 0 = none)")]
        public float hpBonus;
        public float staminaBonus;             // flat + to stamina regen per second
    }
}
