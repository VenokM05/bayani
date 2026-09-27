# Template: Location / Zone

> Copy this file to `content/locations/<slug>.md` (e.g. `content/locations/limasawa.md`) and fill it in.
> Every location must bind to exactly one Unity scene (`SC_00`–`SC_05`, per ggd §56.5) or one named zone within a scene.
> Memory Stability rules that apply to zones are defined in ggd.md §56.3.

```markdown
---
id: LOC_0001                   ⚑ stable unique id (prefix LOC_)
name: ""                       ⚑ display name (e.g. Limasawa Coast)
scene: SC_02_Limasawa          ⚑ one of SC_00_FutureManila | SC_01_Balikan | SC_02_Limasawa | SC_03_Cebu | SC_04_Mactan | SC_05_MemoryVoid
era: 1521                      ⚑ 2187 | 1521
type: outdoor                  ⚑ outdoor | hub | interior | boss_arena | hidden
stability_start: 80            ⚑ % — see ggd §56.3 (100 − 20 damage-on-arrival default)
historical_status: historical  ⚑ ggd §51 enum
source_reference: ""           required if historical_status ≠ fictional
art_brief: ""                  1–2 sentence visual target (ggd §49 art direction)
audio_brief: ""                ambient palette (blueprints §46: wind, ocean, bamboo percussion…)
---

## Layout
ASCII or image reference (keep blueprints.md §12 style). List sub-areas.

## Zones / Sub-areas
| Sub-area | Purpose | Locked behind |
|---|---|---|
| Beach | arrival, first artifact | — |
| Forest | duwende quest | Intro quest complete |

## NPC Placements
| NPC id | Name | Schedule / position |
|---|---|---|

## Enemies
| Enemy id | Count | Spawn condition | (from ggd §56.3: stability-linked spawns go here) |
|---|---|---|---|

## Artifacts / Treasure
| Artifact id | Discovery method | Scannable? |
|---|---|---|

## Quests
| Quest id | Title | Giver |
|---|---|---|

## Visual Gradient Notes
How the zone looks at 80% → 40% → 10% Memory Stability (desaturation, fog, glitch level — ggd §56.3).

## Open Questions
```
