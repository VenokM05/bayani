# BAYANI — Echoes of Mactan

*Travel the past. Fight the forgotten. Discover the Bayani.*

A third-person action-adventure RPG set in a stylized recreation of the 1521 Philippines, entered through reconstructed memories from a future (2187) National Memory Archive. Built in **Unity 6 (URP)**, C#, targeting **Windows first** (see platform order in ggd.md §56.1 #10).

## Documentation Map

| File | Role | Authority |
|---|---|---|
| [`docs/ggd.md`](docs/ggd.md) | **Game Design Document — canonical spec** | ✅ **Source of truth.** Includes §56 Canonical Resolutions (conflict register, undefined-mechanics definitions, scope rulings, corrected Unity structure, source references) |
| [`docs/core_of_bayani.md`](docs/core_of_bayani.md) | Design Rationale Appendix — player journey, combat feel, mythology handling, narration philosophy | Supporting; conflicts resolved in ggd.md §56 |
| [`docs/blueprints.md`](docs/blueprints.md) | Production & Narrative Appendix — opening cinematic, scene breakdowns, quest/NPC data shapes | Supporting; conflicts resolved in ggd.md §56 |

> All three documents were reconciled on 2026-09-27. If you find a new contradiction, fix it **in `docs/ggd.md`** and add a row to the §56.1 register.

## Current Milestone

**Not the full game.** The order of work is:

1. **5-minute combat + discovery prototype** (graybox, one enemy, one weapon, one artifact scan, Windows-only, offline) — the go/no-go test for the project. Spec: core_of_bayani.md §47 + ggd.md §56.8.
2. **Vertical slice** — 60–90 min, local save only, **no backend** (ruling: ggd.md §56.6).
3. Everything else (mobile, web, Laravel cloud features, later eras) comes after the slice proves the game is fun.

## Repository Layout

```text
bayani/
├── docs/        # design documents (start with ggd.md)
├── Game/        # Unity project root (created at prototype kickoff)
└── .gitignore   # Unity + OS + IDE ignores, ready for Game/
```
