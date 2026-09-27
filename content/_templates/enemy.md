# Template: Enemy / Creature

> Copy this file to `content/enemies/<slug>.md` (e.g. `content/enemies/anino.md`) and fill it in.
> Canonical Limot roster: ggd.md §7 (Anino, Limot Lingid, Limot Bantay, Limot Lumilipad, Limot Mangkukulam, Limot Tagapuksa, Memory Guardian). The English archetype names in core_of_bayani.md §15 are **superseded** (mapping in ggd §56.1 #5).
> Combat numbers must stay consistent with the Diwa/HP/Stamina economy in ggd.md §56.2.

```markdown
---
id: EN_0001                    ⚑ stable unique id (prefix EN_)
name: ""                       ⚑ display name (e.g. Limot Lingid)
category: limot                ⚑ limot | corrupted_spirit | mythological | wildlife
tier: standard                 ⚑ standard | elite | miniboss | boss
scenes: []                     ⚑ SC_ ids where this enemy appears
historical_status: fictional   ⚑ ggd §51 enum — Limot are always fictional;
                               mythological creatures are folklore (ggd §2/§8)
weakness: []                   damage types / mechanics (e.g. heavy, parry-window, fire)
resistance: []                 e.g. light-attack reduction, immune-to-throwables
---

## Behavior / AI
State machine in plain words: idle → detect → engage → reposition → flee?
3–6 bullets. NavMesh assumptions. (Stealth enemies: describe shadow-despawn rules.)

## Attacks
| Name | Windup (s) | Damage | Telegraph | Player counter |
|---|---|---|---|---|
| Swipe | 0.6 | 15 HP | crouch + glow | dodge → +8 Diwa (ggd §56.2) |

## Stats
HP: — · Stamina-drain-on-hit: — · Aggro radius: — · Post-kill Diwa grant: +6 (default, ggd §56.2)

## Memory Stability Impact (ggd §56.3)
Kill → +5 stability (default). If this enemy is a "corrupted nest", define the −1/min drain here.

## Drops
| Item id | Rarity (1–5, ggd §56.1 #6) | Chance | Notes |
|---|---|---|---|

## Codex Entry
codex_category: CREATURES    ⚑ canonical 7-tab list (ggd §56.1 #3)
unlock_condition: ""         e.g. "Scan after first kill in SC_02_Limasawa"

## Art / VFX Brief
Silhouette + signature visual motif (Limot = black smoke + humanoid). 2–4 bullets.

## Audio Brief
Spawn cue, attack sounds, death sound. Mythological creatures: lean on environmental sound (blueprints §45).

## Notes / Open Questions
```
