---
id: EN_0004                        ⚑
name: "Corrupted Anito"
category: corrupted_spirit         ⚑
tier: miniboss                     ⚑
scenes: [SC_04_Mactan]             ⚑ cave beneath the shore (SEQ 10B)
historical_status: folklore        ⚑ anito belief = ethnographic; the *corruption* is our fiction (ggd §51, §2/§8)
source_reference: "Anito/bathala belief record — NatMus / ethnographic file (ggd §51 pipeline); corruption layer is fictional"
weakness: [parry-window, diwa-burst]
resistance: [light-attacks-late-phases]
---

## Behavior / AI
Three phases, all telegraph-legible (difficulty verdict from 1A: readability over raw speed):
1. **Veiled** — drifting shrine-spirit, sweeps at melee range; teaches spacing
2. **Reaching** — summons 2 Anino from wall-shadow; teaches wave-priority (kill adds first or the arena floods)
3. **Wounded** — at 50% HP it drives itself into the seam-wall; the win state is **expelling it, not killing it** — finisher is a Diwa-burst prompt, and "driven back, not deleted" (storyline SEQ 10B)

## Attacks
| Name | Windup (s) | Damage | Telegraph | Player counter |
|---|---|---|---|---|
| Shrine sweep | 0.9 | 14 HP | glow crawls along the arm like incense ash | dodge back or side |
| Shadow-call | 1.4 | — (summon) | both arms raised, walls darken | interrupt with heavy (staggerable) — or kill the Anino first |
| Seam-drag (phase 3) | 2.0 | — (scripted) | whole room tilts toward the wall | Diwa-burst prompt window |

## Stats
HP: 120 (phase-gated; effective ~90 before the expel prompt) · Arena: cave, waist-deep tidal water (slight player-mobility flavor for both sides) · Post-kill Diwa: +6

## Memory Stability Impact (ggd §56.3)
**The narrative machine's first shown gear:** its nest drains the village −1/min continuously; the LOC_0003 collapse to 0% (door event for SEQ 10B) is *this* enemy's work. Expulsion restores the village anchor to 100% once, cinematically — proof-of-concept that restoring memory is the actual game.

## Drops
| Item id | Rarity | Chance | Notes |
|---|---|---|---|
| (one guaranteed story drop: Shrine Bell token) | — | 100% | codex HERITAGE entry; later quests' recall key |

## Codex Entry
codex_category: SPIRITS
unlock_condition: "Defeat in SEQ 10B; entry text frames the corruption as the Devourer's work, not the anito's nature"

## Art / VFX Brief
- A village shrine spirit wearing its own corruption: gold-leaf patterns under oil-slick smudge
- Carries one *clean* element throughout (a clay pot, a woven strip) — the visual argument it can be saved
- Cave palette: ink-wash bleed toward SC_05 the deeper the fight goes

## Audio Brief
Ambience: water drops + detuned bell. Phase 3: the bell stops. Expulsion cue: one clean bell-strike from off-screen.

## Notes / Open Questions
- First appearance of the "never kill the corrupted, expel them" rule (chapter rule: the Devourer itself is only ever expelled — Beat Rule 4).
