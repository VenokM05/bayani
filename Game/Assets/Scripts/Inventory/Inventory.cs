// BAYANI — Inventory container (docs/prd-progression.md §9, skeleton).
// A DontDestroyOnLoad host holding a STATIC List<ItemData> — the same survival
// trick as ProgressStore: death and SceneManager.LoadScene can't touch a static,
// and every grant path (StoryTrigger, ArtifactScanner, enemy drops) is a live
// runtime call into here. JSON save/load is local-only per ggd §46 — file lives
// in Application.persistentDataPath; loading matches ids against items the
// catalog has seen this session (full cold-load catalog ships with the save UI).
// Crafting is Phase 3 scope — not here.

using System.Collections.Generic;
using UnityEngine;

namespace Bayani.Player
{
    public class Inventory : MonoBehaviour
    {
        [System.Serializable]
        private class SavedSlot { public string id; public int count; }
        [System.Serializable]
        private class SavedInventory { public List<SavedSlot> slots = new List<SavedSlot>(); }

        // THE state: static survives death + scene loads (ProgressStore pattern).
        // One entry per STACK; stackable items merge into an open stack up to maxStack.
        private static readonly List<ItemData> _items = new List<ItemData>();
        private static readonly List<int> _counts = new List<int>();
        private static readonly Dictionary<ItemData, int> _known = new Dictionary<ItemData, int>();

        public static Inventory Instance { get; private set; }

        public const int SlotCount = 8;                       // 6–9 graybox slots (HUD row)
        public static IReadOnlyList<ItemData> Items => _items;
        public static int CountAt(int slot) => slot >= 0 && slot < _counts.Count ? _counts[slot] : 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static int CountOf(ItemData item)
        {
            int n = 0;
            for (int i = 0; i < _items.Count; i++) if (_items[i] == item) n += _counts[i];
            return n;
        }

        /// <summary>The one grant door. Applies one-time stat modifiers on first pickup.</summary>
        public static void Add(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0) return;

            bool firstTime = !_known.ContainsKey(item);
            if (item.stackable)
            {
                // merge into an open stack first, then spill into new slots
                int cap = Mathf.Max(1, item.maxStack);
                for (int i = 0; i < _items.Count && amount > 0; i++)
                    if (_items[i] == item && _counts[i] < cap)
                    {
                        int move = Mathf.Min(amount, cap - _counts[i]);
                        _counts[i] += move;
                        amount -= move;
                    }
            }
            while (amount > 0)
            {
                int cap = item.stackable ? Mathf.Max(1, item.maxStack) : 1;
                int move = Mathf.Min(amount, cap);
                _items.Add(item);
                _counts.Add(move);
                amount -= move;
            }

            if (firstTime)
            {
                _known[item] = 1;
                ApplyStatBonus(item);
            }
            Debug.Log($"[BAYANI] ITEM +{item.itemName} (holding {CountOf(item)})");
            Save();
        }

        public static bool Remove(ItemData item, int amount = 1)
        {
            if (item == null || CountOf(item) < amount) return false;
            for (int i = _items.Count - 1; i >= 0 && amount > 0; i--)
                if (_items[i] == item)
                {
                    int take = Mathf.Min(amount, _counts[i]);
                    _counts[i] -= take;
                    amount -= take;
                    if (_counts[i] <= 0) { _items.RemoveAt(i); _counts.RemoveAt(i); }
                }
            Save();
            return true;
        }

        private static void ApplyStatBonus(ItemData item)
        {
            var res = FindFirstObjectByType<Bayani.Combat.CombatResources>();
            if (res == null) return;
            if (item.hpBonus != 0f) res.GrowMaxHP(item.hpBonus);
            if (item.staminaBonus != 0f)
            {
                // regen is recomputed centrally (level + stat-menu + item terms)
                Bayani.Combat.ProgressStore.ItemBonusStaminaRegen += item.staminaBonus;
                Bayani.Combat.PlayerProgression.Instance?.RefreshStaminaRegen();
            }
        }

        // ---------- local JSON save (ggd §46 — no backend, ever) ----------
        private static string SavePath =>
            System.IO.Path.Combine(Application.persistentDataPath, "bayani_inventory.json");

        public static void Save()
        {
            try
            {
                var data = new SavedInventory();
                for (int i = 0; i < _items.Count; i++)
                    data.slots.Add(new SavedSlot { id = _items[i].itemName, count = _counts[i] });
                System.IO.File.WriteAllText(SavePath, JsonUtility.ToJson(data));
            }
            catch (System.IO.IOException) { }   // skeleton: loss of the flat file is not fatal
        }

        /// <summary>Restore from the local file, resolving names through a caller-supplied
        /// catalog (the SOs the running scenes reference). Unknown ids are skipped.</summary>
        public static void LoadFromSave(IList<ItemData> catalog)
        {
            if (catalog == null || !System.IO.File.Exists(SavePath)) return;
            try
            {
                var data = JsonUtility.FromJson<SavedInventory>(System.IO.File.ReadAllText(SavePath));
                foreach (var slot in data.slots)
                {
                    var item = catalog != null ? FindByName(catalog, slot.id) : null;
                    if (item != null) Add(item, slot.count);
                }
            }
            catch (System.Exception e) { Debug.LogWarning($"[BAYANI] inventory save unreadable: {e.Message}"); }
        }

        private static ItemData FindByName(IList<ItemData> catalog, string id)
        {
            foreach (var it in catalog) if (it != null && it.itemName == id) return it;
            return null;
        }
    }
}
