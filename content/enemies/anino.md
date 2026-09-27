---
id: EN_0001                        ⚑
name: "Limot Anino"
category: limot                    ⚑
tier: standard                     ⚑
scenes: [SC_03_Cebu, SC_04_Mactan]
historical_status: fictional       ⚑
weakness: [heavy-attacks, parry-window, fire-torch-light]
resistance: []
---

## Behavior / AI
- Patrol → detect (aggro radius ~8 m, sight) → chase at 3.2 m/s → telegraph → swipe → recover, repeat
- Never blocks; spacing is its only defense — pressure it and it flinches (stagger threshold low)
- Forms packs of 2–3 only when local Memory Stability < 60% (ggd §56.3 spawn rule)
- NavMesh agent, ground-only; dissolves to smoke on death (no corpse)

## Attacks
| Name | Windup (s) | Damage | Telegraph | Player counter |
|---|---|---|---|---|
| Swipe | 0.7 | 12 HP | crouch + arm-glow, red tint | dodge → +8 Diwa; block chip; parry → +12 Diwa + stagger (ggd §56.2) |

## Stats
HP: 30 · Stamina-drain-on-hit: — (hits player HP) · Aggro radius: 8 m · Post-kill Diwa grant: +6 (default, ggd §56.2)
Runtime values live in `Game/Assets/Data/Combat/Enemy_Anino.asset` (SO = single source at runtime; this file = design source).

## Memory Stability Impact (ggd §56.3)
Kill → +5 stability; zone visibly brightens one step. Aninos ARE the stability drain's symptom — leave-one-alive bleeds −1/min in its nest radius.

## Drops
| Item id | Rarity (1–5) | Chance | Notes |
|---|---|---|---|
| — (none; Diwa is the drop) | — | — | material drops start Phase 2 economy |

## Codex Entry
codex_category: CREATURES
unlock_condition: "Survive SEQ 07B First Shadow (auto)"

## Art / VFX Brief
- Black smoke drawn into a wrong human silhouette; edges constantly dripping back into the ground
- Eyes: two absences, not lights. Telegraph glow = the ONLY light it ever makes
- Dies by unmaking: limbs detach as smoke, body forgets its shape last

## Audio Brief
Spawn: ambient drop-out (air gets thin). Attack: cloth-whip hiss. Death: inverted inhale. Voice: none — never speaks.

## Notes / Open Questions
- The implemented Phase 1 graybox version IS this spec's numbers; keep this file updated when SOs are tuned.
