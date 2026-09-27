// BAYANI — Battle health contract (docs/prd-progression.md §6): anything with a
// health bar (Limot, the Anito, later every enemy family) exposes fraction + name;
// a lightweight registry lets CombatHUD draw overhead bars without FindObjectOfType
// scans every frame. Enemies register on enable, leave on destroy.

using System.Collections.Generic;
using UnityEngine;

namespace Bayani.Combat
{
    public interface IBattleHealth
    {
        float HealthFraction { get; }
        string BarName { get; }
        Vector3 HeadPoint { get; }      // world position above the head for the bar
        bool BarVisible { get; }        // e.g. hide while dormant
    }

    public static class BattleHealthRegistry
    {
        public static readonly List<IBattleHealth> Active = new List<IBattleHealth>();

        public static void Add(IBattleHealth h) { if (!Active.Contains(h)) Active.Add(h); }

        public static void Remove(IBattleHealth h) => Active.Remove(h);
    }
}
