# BAYANI — Echoes of Mactan

*Travel the past. Fight the forgotten. Discover the Bayani.*

A third-person action-adventure RPG set in a stylized recreation of the 1521 Philippines, entered through reconstructed memories from a future (2187) National Memory Archive. Built in **Unity 6 (URP)**, C#, targeting **Windows first** (see platform order in ggd.md §56.1 #10).

## Documentation Map

| File | Role | Authority |
|---|---|---|
| [`docs/ggd.md`](docs/ggd.md) | **Game Design Document — canonical spec** | ✅ **Source of truth.** Includes §56 Canonical Resolutions (conflict register, undefined-mechanics definitions, scope rulings, corrected Unity structure, source references) |
| [`docs/phasing.md`](docs/phasing.md) | Development Phasing — step-by-step P0–P5 with exit gates and anti-scope rules | Sequences ggd.md §56 rulings; ggd.md wins on conflicts |
| [`docs/storyline.md`](docs/storyline.md) | **Chapter 1 Narrative Canon — "The Blood Remembers"** (SEQ 01–17 + Gameplay Beat Map) | Story beats/tone: **wins**. Systems/scope/numbers: ggd.md §56 wins (register #11–12) |
| [`docs/prd-progression.md`](docs/prd-progression.md) | **PRD — Progression, Death & Active Skills** (XP/levels, respawn, armed/unarmed skill bars, HUD bars) | Implements ggd §56.4 budget; canonical for the progression layer (register #13) |
| [`docs/core_of_bayani.md`](docs/core_of_bayani.md) | Design Rationale Appendix — player journey, combat feel, mythology handling, narration philosophy | Supporting; conflicts resolved in ggd.md §56 |
| [`docs/blueprints.md`](docs/blueprints.md) | Production & Narrative Appendix — opening cinematic, scene breakdowns, quest/NPC data shapes | Supporting; conflicts resolved in ggd.md §56 |

> All documents were reconciled on 2026-09-27 (storyline.md added as narrative canon same month). If you find a new contradiction, fix it **in `docs/ggd.md`** and add a row to the §56.1 register. Narrative changes go through `docs/storyline.md` first.

## Current Milestone

**Not the full game.** The order of work is:

1. **5-minute combat + discovery prototype** (graybox, one enemy, one weapon, one artifact scan, Windows-only, offline) — the go/no-go test for the project. Spec: docs/phasing.md Phase 0–2 + core_of_bayani.md §47 + ggd.md §56.8.
2. **Vertical slice** — 60–90 min, local save only, **no backend** (ruling: ggd.md §56.6).
3. Everything else (mobile, web, Laravel cloud features, later eras) comes after the slice proves the game is fun.

## Build Progress (start → today, updated 2026-09-27)

**P0 — Foundation.** Repo + docs reconciliation (ggd §56), Unity 6 (URP, 6000.3.10f1) project in `Game/`, content data layer under `content/`.

**P1 — Combat core (all exit gates passed).** Camera-relative controller, Hitbox/Hurtbox damage pipeline, combo/block/parry + Bantay auto-parry tutorial, graybox HUD with FPS readout, one-click arena builders incl. 6-wave spawner. Playtest verdicts: decisions ✓ · parry ✓ · Bantay ✓. *Open: the wave-arena 60 FPS number is still unreported.*

**P2 — Chapter 1 story slices, graybox-playable:**

| Beat | Scene | Landed |
|---|---|---|
| SEQ 01–05 Prologue (2187 archive, first Diwa) | `SC_01_Prologue` | committed `5705ede` |
| SEQ 06–08 + 07B Cebu 1521 (arrival, people, court) | `SC_03_Cebu` | committed `9baaeb7` — user-verified |
| SEQ 09–10 Mactan village (Lapu-Lapu, shrine, fire circle) | `SC_04_Mactan` | committed `4f84fc0` |
| SEQ 10B Corrupted Anito cave (expel-not-kill mini-boss) | `SC_04_Mactan` east | committed `0638ae0`; boss-damage probe awaiting console report |

**Systems since then:** Memory Stability zones (§56.3) · story-triggered waves · dialogue/artifact/kill XP economy · **progression layer** (`0638ae0`, `bf5b683`): L1→L10 = 5,000 XP per §56.4, 8 arnis skills as data SOs on armed/unarmed bars (1–5, G swap), death → YOU DIED → checkpoint respawn with zero loss, overhead + dedicated boss health bars — spec'd in [`docs/prd-progression.md`](docs/prd-progression.md).

**Still ahead:** SEQ 11 Battle of Mactan + Memory Devourer → SEQ 12–13 return/consequence → Mixamo rig + real skill animations → playtest verdicts (10B boss, Mactan pacing, arena FPS). Local commits only; pushes are manual.

## Repository Layout

```text
bayani/
├── docs/        # design documents (start with ggd.md, then phasing.md)
├── content/     # design data: chapters/ index, characters/, locations/, enemies/ + _templates/ + _asset_needs.md
├── Game/        # Unity project root (created at prototype kickoff)
└── .gitignore   # Unity + OS + IDE ignores; .gitattributes has Git LFS rules
```
