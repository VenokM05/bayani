---
id: LOC_0004                       ⚑
name: "Mactan — The Battle Shore"
scene: SC_04_Mactan                ⚑ (with SC_05_MemoryVoid overlay for Devourer phases)
era: 1521                          ⚑
type: boss_arena                   ⚑
stability_start: 40                ⚑ enters at story-low after SEQ 10B restoration carries only so far
historical_status: historical      ⚑
source_reference: "NHCP — Battle of Mactan markers; Pigafenna's chronicle account (public domain) for shape of the event; ggd §56.7"
art_brief: "Low tide, two walls of people, a narrow green line of beach between — history's stage, played straight (SEQ 11). The Memory Devourer's presence never covers the battle; it exists in the desaturated SEAMS around it."
audio_brief: "Drums, water, shouted Bisayan commands; battle mix stays real and physical — the Devourer's counter-audio is negative space (silence swells, crowd-echoes thinning)."
---

## Layout
Village exit → shore approach → the line of battle (choreographed corridor) → tide flat (final stand) → **the seams** (Devourer pockets, offset geometry) → collapse back to LOC_0001 cave mouth.

## Zones / Sub-areas
| Sub-area | Purpose | Locked behind |
|---|---|---|
| Shore approach | SEQ 11 formation cinematic while player walks the line | — |
| Line of battle | spectacle wave-combat corridor (waves 1–3) | Beat cinematic |
| The seams | Devourer arena pockets during lulls (waves 4–6, phase 3) | Phase triggers |
| Tide flat | final hold + pull-back goodbye | Devourer expelled |

## Enemies
| Enemy id | Count | Spawn condition |
|---|---|---|
| EN_0001 Anino | wave groups | stability-linked tide of battle (waves 1–3, never the historical soldiers) |
| EN_0002 Lingid | 1–2 per wave from wave 2 | seam-crossers only |
| EN_0003 Bantay | 1 per wave 3+ | anchors each wave |
| **EN_0005 Memory Devourer** | 1 | **boss — appears Kai-only, fights in seams, phase-3 pull-back** |

## NPCs
CH_0002 in the battle line (findable during waves 1–2 as a *look-over-there* beat — never interacts). CH_0004 within chronicle-safe framing. Historical outcome untouched (Beat Rule 3).

## Quests
| Quest id | Title | Giver |
|---|---|---|
| — | Chapter finale; no quest UI during SEQ 11 (deliberate) | — |

## Visual Gradient Notes
Battlefield runs the doc's hardest gradient: the *real* battle stays saturated; everything within the Devourer's seams drains toward SC_05's ink-wash void palette. Phase 3 arena = half-battlefield, half-void, the goodbye staged on the seam line.

## Open Questions
- Wave choreography vs. chronicle geography: keep enemies as memory-bleed, never as combatants in the recorded fight.
- Is Kai physically present at the shore at the recorded moment at all? Current: no — pulled into the void seam as it ends. That keeps Beat Rule 3 airtight.
