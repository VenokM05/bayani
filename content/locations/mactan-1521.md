---
id: LOC_0003                       ⚑
name: "Mactan — Village, Shrine & Under-Shore Cave"
scene: SC_04_Mactan                ⚑
era: 1521                          ⚑
type: hub                          ⚑ (+ interior cave sub-zone)
stability_start: 80                ⚑ drops to 0 trigger for 10B (storyline SEQ 10B)
historical_status: historical      ⚑
source_reference: "NHCP — Mactan Shrine / Lapu-Lapu Monument markers (https://nhcp.gov.ph/); ggd §56.7"
art_brief: "Prepared village, not panicked camp: forges, net-mending, war canoes hauled up, elders teaching children the old refusal. SEQ 09 — 'the community preparing for confrontation.'"
audio_brief: "Forge rhythm, saws on bamboo sharpening, war-drums practiced (not performed), children quiet for once — the SEQ 10 weight."
---

## Layout
Village gate → forge row → longhouse circle → **shore shrine** (SEQ 09B) → headland path to **the cave beneath the shore** (SEQ 10B) → fire circle for the night-before talk (SEQ 10).

## Zones / Sub-areas
| Sub-area | Purpose | Locked behind |
|---|---|---|
| Village gate & streets | explore hub, quests, NPC roster (ancestor CH_0002 visible in the line) | — |
| **Shore shrine** | **SEQ 09B — THE EDGE AWAKENS**: Balikan Edge unfold, ARTIFACT SCAN, Memory Stability HUD intro | Arrival at Mactan |
| Forge row | blacksmith who bows to the Edge (beat 09B); gear/codex HERITAGE seeds | Edge granted |
| **Under-shore cave** | **SEQ 10B — CORRUPTED ANITO mini-boss arena**; stability collapse to 0% is the door event | Before dawn (SEQ 10 end) |
| Fire circle | SEQ 10 cinematic — the doc's emotional peak | — |

## NPC Placements
| NPC id | Name | Schedule / position |
|---|---|---|
| CH_0002 | The Ancestor | village line + forge visit; moves to beach at SEQ 11 |
| CH_0004 | Lapu-Lapu | headland, distant watch only — Beat Rule 3 (camera distance) |
| CH_0006 | Warrior of the Night Before (proposed id) | fire circle, SEQ 10 |
| Blacksmith | bows first to the Edge (09B) | forge row |

## Enemies
| Enemy id | Count | Spawn condition (stability-linked per ggd §56.3) |
|---|---|---|
| EN_0001 Anino | packs 2–3 | spawn as village stability drops; clear = +5 stability, zone brightens |
| EN_0002 Lingid | 1–2 with packs | stability < 60% — ranged pressure enters as memory weakens |
| EN_0003 Bantay | 1 | stability < 40% — the "something is feeding" argument made flesh |
| EN_0004 Corrupted Anito | 1 | SEQ 10B scripted mini-boss, cave arena |

## Artifacts / Treasure
| Artifact id | Discovery method | Scannable? |
|---|---|---|
| AR_ARTIFACT → **Balikan Edge** (unfolded) | SEQ 09B shrine | it IS the scanner |

## Quests
| Quest id | Title | Giver |
|---|---|---|
| QU_001 (reserved) | "The Shrine Drinks the Tide" | Blacksmith elder |
| QU_002 (reserved) | "What's Feeding Under the Village" | CH_0006 |

## Visual Gradient Notes
80→0% arc is the LOC_0003 gameplay spine: village songs fade layers, colors pull to ash, faces blank out, houses go half-rendered — cave collapse is the floor (0% = everyone sleeps/dissolves until restored). 10B win re-anchors to 100% once, cinematically.

## Open Questions
- Cave's visual link to SC_05_MemoryVoid (the Devourer's domain) — shared wall shaders to sell "one world of memory underneath."
