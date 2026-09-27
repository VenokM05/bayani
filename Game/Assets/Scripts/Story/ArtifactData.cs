// BAYANI — Artifact data (phasing 2.2): what a scannable relic is, and what scanning it
// grants. Rewards follow ggd: +15 Diwa (§56.2, per phasing 2.2), +3 Memory Stability
// (§56.3), 50 XP (§56.4 artifact award — stored now, XP ledger lands with Phase 2 economy).
// historical_status badge values are the canonical §51 enum — never invent new ones.

using UnityEngine;

namespace Bayani.Story
{
    [CreateAssetMenu(menuName = "BAYANI/Artifact")]
    public class ArtifactData : ScriptableObject
    {
        [Header("Identity")]
        public string artifactId = "AR_0001";           // stable key matching content/ files
        public string displayName = "";
        [TextArea(3, 6)] public string description = "";   // TALA's scan read-out

        [Header("Codex (ggd §51 / §56.1 #3)")]
        public string historicalStatus = "archaeological"; // historical | archaeological | ethnographic | folklore | fictional
        public string codexCategory = "HERITAGE";          // one of the 7 canonical tabs

        [Header("Rewards (data-driven — no hard-coded numbers in scanner)")]
        public float diwaReward = 15f;        // §56.2
        public float stabilityReward = 3f;    // §56.3
        public float xpReward = 50f;          // §56.4 (logged until the XP ledger exists)
    }
}
