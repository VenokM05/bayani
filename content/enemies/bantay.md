---
id: EN_0003                        ⚑
name: "Limot Bantay"
category: limot                    ⚑
tier: elite                        ⚑
scenes: [SC_04_Mactan]
historical_status: fictional       ⚑
weakness: [parry-window]            ⚑ the ONLY stagger — teaching enemy by design
resistance: [light-attack-flinches, knockback-impunity]
---

## Behavior / AI
- Slow anchor (2.2 m/s) that controls space; fights are scheduled around its swings
- Telegraph 0.8 s magenta glow → massive overhead; trades badly, sidesteps beautifully
- **Poise rule:** ignores every knockback below 6 — light combo hits do NOT interrupt it. Only a **parry** staggers it (recover × 1.8) — it exists to teach the parry
- Spawns from stability < 40%; one per arena, never in opener packs

## Attacks
| Name | Windup (s) | Damage | Telegraph | Player counter |
|---|---|---|---|---|
| Cleaving drop | 0.8 | 18 HP | magenta glow + rising stance | **parry → the only stagger** (+12 Diwa); dodge sideways (its sweep is wide); never trade |

## Stats
HP: 70 · Aggro radius: 9 m · Attack range: 2.2 m · Active: 0.2 s · Recover: 1.2 s · Post-kill Diwa: +6
Runtime source: `Game/Assets/Data/Combat/Enemy_Bantay.asset`. Scale 1.3 — biggest silhouette of the trio.

## Memory Stability Impact (ggd §56.3)
Kill → +5 stability, and because it gates at < 40%, killing one is the player's *visible* recovery swing.

## Drops
| Item id | Rarity | Chance | Notes |
|---|---|---|---|
| — (Diwa is the drop) | — | — | post-Ch.1: guard-shard material idea |

## Codex Entry
codex_category: CREATURES
unlock_condition: "First Bantay kill in LOC_0003"

## Art / VFX Brief
- Dense smoke packed to near-solid; where Anino drips, Bantay *holds* — slab-shouldered, guard-posture even idle
- Faint shield-echo ripple on un-parried hits (visually states "that bounced")
- Death: not unmaking but *collapse* — the shape holds one second too long, then drops all at once

## Audio Brief
Spawn: low thud like a door being leaned on. Telegraph: deep inhale. Slam: wet timber crack. Death: single heavy exhalation.

## Notes / Open Questions
- Named "Bantay" (guard) per ggd §7 roster — but check lore: Bantay-lingin folk motif (the warning bird) vs. our guard archetype. Codex text should acknowledge the naming choice.
