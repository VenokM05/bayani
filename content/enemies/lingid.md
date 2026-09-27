---
id: EN_0002                        ⚑
name: "Limot Lingid"
category: limot                    ⚑
tier: standard                     ⚑
scenes: [SC_03_Cebu, SC_04_Mactan]
historical_status: fictional       ⚑
weakness: [dodge-forward-closing, any-knockback-during-aim]
resistance: []
---

## Behavior / AI
- Ranged harasser: holds ~6 m standoff, retreats if Kai closes inside ~3 m
- Chase logic keeps distance rather than closing — fights are about cornering it
- Telegraph 0.9 s (orange glow, longest of the trio — its shot is readable, never cheap)
- Low poise (stagger at knockback ≥ 2): heavies and even glancing hits cancel the shot mid-aim
- Pack follower — rarely the first enemy in a fight; appears once stability < 60%

## Attacks
| Name | Windup (s) | Damage | Telegraph | Player counter |
|---|---|---|---|---|
| Shadow bolt | 0.9 | 10 HP (projectile, 11 m/s) | orange glow + aim line | dodge through → +8 Diwa; hit it mid-glow to cancel the shot |

## Stats
HP: 20 · Aggro radius: 10 m (standoff 6 m) · Move speed: 2.6 · Recover: 1.1 s · Post-kill Diwa: +6
Runtime source: `Game/Assets/Data/Combat/Enemy_Lingid.asset`. Scale 0.85 — smallest silhouette of the trio.

## Memory Stability Impact (ggd §56.3)
Kill → +5 stability. Its spawn condition doubles as the player's *reading tool*: Lingids on screen = memory weakening here.

## Drops
| Item id | Rarity | Chance | Notes |
|---|---|---|---|
| — (Diwa is the drop) | — | — | |

## Codex Entry
codex_category: CREATURES
unlock_condition: "First Lingid engagement in LOC_0003 (post-SEQ 09B)"

## Art / VFX Brief
- Thin, elongated smoke; never stands square — sways like heat off road
- Bolt is a pulled-off strip of its own body (the shot visibly costs it mass — regeneration shimmer after firing)

## Audio Brief
Spawn: faint string-pull. Aim: rising whistle. Shot: dry snap. Death: unraveling rustle.

## Notes / Open Questions
- Phase 2 question: should bolts be parriable too, or keep parry melee-only? Current answer: melee-only (parry teaches close-the-distance courage).
