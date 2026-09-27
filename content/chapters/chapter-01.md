# Chapter 1 — Echoes of Mactan ("The Blood Remembers")

> Narrative canon: `docs/storyline.md` (SEQ 01–17 + the playable 07B/09B/10B/11 beats).
> Systems canon: `docs/ggd.md` §56; progression layer (XP/levels, death/respawn, skills 1–5): `docs/prd-progression.md`. This file is the **index** — content files below are the data.

## Sequences → what exists where

| SEQ | Beat | Content | Unity status |
|---|---|---|---|
| 01–05 | 2187, argument, transit, activation | `locations/ruins-2187.md` | **graybox built** — Tools → BAYANI → Phase 2 prologue (SC_00_FutureManila) |
| 06–08 | Arrival, Sugbu, court | `locations/cebu-1521.md`, `characters/humabon.md` | **graybox built** — Tools → BAYANI → Phase 2 Cebu (SC_03_Cebu) |
| **07B** | **The First Shadow** (combat tutorial) | `enemies/anino.md` | **graybox built** — scripted `TutorialAmbush` in SC_03_Cebu (no death possible) |
| 09 | Mactan, Lapu-Lapu seen | `locations/mactan-1521.md`, `characters/lapu-lapu.md` | **graybox built** — Tools → BAYANI → Phase 2 Mactan (SC_04_Mactan) |
| **09B** | **The Edge Awakens** (scan + stability HUD) | `characters/kai.md` | **graybox built** — shrine unfold + Balikan Edge scan + story-triggered headland pack in SC_04_Mactan |
| 10 | Night before, the fire circle | `characters/ancestor.md` | **graybox built** — SEQ 10 fire-circle dialogue in SC_04_Mactan (ends on 10B collapse hook) |
| **10B** | **Corrupted Anito** (cave mini-boss) | `enemies/corrupted-anito.md` | **graybox built** — Tools → BAYANI → Phase 2 The Cave (expel-not-kill, stability 0→100 restore) |
| 11 | The battle + **Memory Devourer** | `locations/battle-shore.md`, `enemies/memory-devourer.md` | not built |
| 12–13 | Goodbye, the pull-back | `characters/ancestor.md` | not built |
| 14–17 | Return, erased family, the blank page | `locations/ruins-2187.md` | not built |

## Ids reserved
`CH_0005` Elder of Cebu · `CH_0006` Warrior of the Night Before · `AR_ARTIFACT` · `QU_000`–`QU_002`

## Build state
Phase 0 + Phase 1 combat graybox (`Game/Assets/Scripts/`, `Assets/Data/Combat/`) + SEQ 01–05 prologue (`Assets/Scenes/Chapter1/SC_00_FutureManila.unity`, dialogue in `Assets/Data/Story/`). Phase 1 gate = playtest verdict still open. Chapter 1 scenes `SC_03`–`SC_05` unbuilt — see `docs/phasing.md`.
