# BAYANI — Development Phasing & Guidelines

> **Purpose:** the step-by-step path from "empty Unity project" to "vertical slice", with hard exit criteria per phase.
> **Authority:** scope rulings come from [`ggd.md`](ggd.md) §56 — this file only sequences them. If a phase here conflicts with ggd.md, ggd.md wins; fix this file.
> **Companion docs:** [`../content/_asset_needs.md`](../content/_asset_needs.md) (art gate list) · [`core_of_bayani.md`](core_of_bayani.md) §42–44 (original phase intent).

---

## The Four Iron Rules (apply to every phase)

1. **Windows only** until Phase 4. No mobile/web/macOS work. (ggd §56.1 #10)
2. **Offline only.** Local JSON saves. No Laravel, no API, no cloud. (ggd §56.6)
3. **No content assets until the mechanic is proven** with graybox. (asset tracker Tier gates)
4. **A phase is done when its exit criteria pass — not when its time budget runs out.** Cut scope, never cut the gate.

---

## Phase Map (at a glance)

```text
P0 SETUP          ~1 week     repo + Unity project + one walkable cube
     ▼
P1 COMBAT CORE    4–6 weeks   "Is swinging fun?"            ← GO/NO-GO GATE
     ▼
P2 WORLD LOOP     3–5 weeks   "Is exploring + scanning fun?"
     ▼
P3 VERTICAL SLICE 8–16 weeks  "Does it feel like BAYANI?"
     ▼
P4 POLISH + DEMO  4–8 weeks   "Can strangers enjoy it?"
     ▼
P5 FULL GAME      (only after P3–P4 succeed)
```

Total realistic calendar to a showable slice: **~5–7 months part-time** (drafts in core §42–43 say 4–8 + 8–16 weeks; those assume full-time focus).

---

## Phase 0 — Project Setup (target: 1 week)

**Goal:** a running Unity project with a camera and a cube you can walk around.

| # | Step | Done when |
|---|------|-----------|
| 0.1 | Unity Hub → install **Unity 6 LTS** (3D template) | Editor opens |
| 0.2 | Create project at `d:\bayani\Game`, **URP** template | `Game/Assets/` exists |
| 0.3 | Commit `ProjectSettings/`, `Packages/`, `Assets/` + `.meta` files (LFS already configured) | First Unity commit lands |
| 0.4 | Create folder structure per ggd §56.5 (`Scripts/`, `Art/`, `Data/`, `Scenes/`…) | Folders exist with .gitkeep |
| 0.5 | Build scene `SC_00_Prototype` — floor plane, directional light, capsule "Kai" | Scene opens at 60 FPS |
| 0.6 | Install input package (new Input System) + set up WASD/mouse actions | No legacy Input axes used |
| 0.7 | Version-control hygiene check: `.meta` committed, no `Library/` in git, `git lfs ls-files` empty | Clean `git status` |

**Exit gate:** capsule moves with WASD + mouse camera in a lit graybox scene, committed.

---

## Phase 1 — Combat Core ✅ CLOSED (target: 4–6 weeks) 🔴 THE GATE

> **Verdict 2026-09-27 (playtest):** fighting 4 enemies "feels like decisions" · 0.2 s parry lands *sometimes* (keep) · Bantay reads as "parry me", not a wall. 1A+1B committed (`0e3d678`, `4a2ad1b`).

**Goal:** answer *the* question — **"Is it fun to control Kai in a fight?"** — with cubes and Mixamo only.

| # | Step | Notes |
|---|------|-------|
| 1.1 | Third-person character controller: walk/run, grounded checks, slopes | Arcade feel, ~5 units/s run; camera follows with shoulder offset |
| 1.2 | Cinemachine third-person camera + collision resolution | Most-feel-critical step; budget real time |
| 1.3 | Input actions + a **ComboInputBuffer** (0.2–0.3 s) | Buffer window decided here, tuned later |
| 1.4 | Combat state machine: Idle → Light ×3 → Heavy, dodge, block/parry | Damage numbers fake; timing is everything |
| 1.5 | Hitbox/hurtbox system (attack windows driven by animation events or timers) | Graybox melee ranges = scaled cubes |
| 1.6 | **Game-feel layer:** hit-stop (~0.1 s), camera shake, knockback, hit flash, SFX placeholder | This is where "fun" actually comes from (ggd decision, §56.2 economy) |
| 1.7 | Anino enemy: NavMesh chase → telegraph (≥0.6 s windup) → attack → recover → death | One enemy, one behavior; must be *fair*, not complex |
| 1.8 | Stamina + Diwa meters per ggd §56.2 (drain/regen values as ScriptableObject) | Numerically compliant on first play |
| 1.9 | 2–3 enemy variants by stat tweaking only (fast/ tanky via `EnemyData` SO) | Proves the SO data pipeline |
| 1.10 | Wave arena: 1→3 enemies, dodge-heavy encounters | Combat sandbox scene — ✅ built via `Tools → BAYANI → Phase 1.10` (spawn ring + `WaveData_Arena.asset`, 3 waves → 10 alive) |

**Playtest checkpoints (weekly):** after 1.4 → does the combo rhythm feel right? After 1.6 → do you *feel* a hit land? After 1.7 → is one enemy interesting for 5 minutes?

**Exit gate (all required):**
- [x] 5-minute combat loop is **replayable by choice, not obligation**
- [x] Dodge/parry matter (you get punished for mashing)
- [ ] Runs 60 FPS on target PC with 10 enemies — *final check: play the 1.10 wave arena, watch the Stat overlay*
- [x] Combat values live in ScriptableObjects, not hard-coded

**If this gate fails → STOP.** Redesign combat before any world content exists. This is the whole point of phasing.

---

## Phase 2 — World & Discovery Loop (target: 3–5 weeks)

**Goal:** answer *"Is exploring and scanning fun without combat?"*

| # | Step |
|---|------|
| 2.1 | Quest system skeleton (`QuestData` SO + objectives, per blueprints §41 field shape — stored in SO, NOT backend) |
| 2.2 | Artifact scan interaction: approach → prompt → TALA scan panel → codex entry (+15 Diwa per §56.2) |
| 2.3 | Codex UI with the 7 canonical tabs + `historical_status` badges (ggd §51/§25) |
| 2.4 | Memory Stability v1 per §56.3 (zone score, desaturation/fog gradient, spawn scaling) |
| 2.5 | Duwende puzzle encounter: trick chest → footprint trail → hidden reward (blueprints §17) |
| 2.6 | Small hand-built jungle graybox with 1 artifact + 1 duwende + 2 Anino |
| 2.7 | Dialogue system v1: on-screen speaker + [Continue] (ggd §37), TALA voice via SO-driven lines — ✅ DELIVERED early in the 2187 prologue (`Bayani.Story`, commit `5705ede`) |
| 2.8 | Local save/load (JSON: player state, quests, codex, stability) — §46 Local Save only |
| 2.9 | **Assemble the core §47 5-minute sequence**: jungle walk → duwende → Limot → fight → artifact → scan → hidden path → Kapre silhouette → cut |

**Exit gate:**
- [ ] The 5-minute sequence plays start→finish without a single hard-coded reference
- [ ] Save/quit/reload mid-sequence loses nothing
- [ ] A tester who hasn't read the docs says something like "I want to see that Kapre"

---

## Phase 3 — Vertical Slice (target: 8–16 weeks)

**Goal:** the 60–90 minute slice (ggd §56.1 #1) — Future → Limasawa → Cebu → Mactan → Memory Devourer → return.

Sequence (build in this order — each item reuses the previous):

| # | Step | Scope ref |
|---|------|-----------|
| 3.1 | Scene shell `SC_00` Future Manila + Balikan traversal cinematic | blueprints §2–§9 |
| 3.2 | `SC_02` Limasawa full: beach/village/forest/shrine/cave, quests 01–03 | blueprints §12–§22 |
| 3.3 | Real art pass: Kai + Anino + weapon FBX (Tier 1, asset tracker) | ggd §56.8 |
| 3.4 | Inventory/equipment + crafting v1 + patos economy (§56.4) | core §17–18 |
| 3.5 | `SC_03` Cebu hub: shop, blacksmith, herbalist, 5–8 NPCs | core §17 |
| 3.6 | Skill trees: 15 nodes, 9-point budget (§56.4) + level-up UI | core §21 |
| 3.7 | Duwende/Anito/Kapre encounters final (trials, guardian-puzzle) | blueprints §17–§22 |
| 3.8 | `SC_04` Mactan: quest chain 01–05, Lapu-Lapu scene, battle layers | blueprints §28–§33 |
| 3.9 | `SC_05` Memory Void + **Memory Devourer 4-phase boss** (blueprints §34) | ggd §17 |
| 3.10 | Full narration pass (NarrationManager, blueprints §44) + ending (§56.1 #9) | |
| 3.11 | 10–20 artifacts + codex content + treasure tiers | blueprints §27 |

**Exit gate:**
- [ ] Complete slice in 60–90 min, local-save only, Windows, 60 FPS
- [ ] All content passes the `historical_status` + `source_reference` rule — **no fictional claim about real history** (ggd §52 Golden Rule)
- [ ] A non-developer plays it without verbal help

---

## Phase 4 — Polish & Public Demo (target: 4–8 weeks)

**Goal:** "Can we show this publicly?" — per core §44.

- Animation/VFX/audio upgrade pass (real SFX over placeholders)
- **Then and only then:** Android port + mobile combat rules (ggd §40 — auto-target, assist options)
- Accessibility (subtitles, difficulty, colorblind), controller support final
- Localization prep: English + Filipino (narration system already data-driven)
- 20–30 min public demo cut (ggd §47) → itch.io / Steam page
- *Post-slice only:* decide if the Laravel backend (§56.6 deferred list) is worth building for cloud saves/telemetry

**Exit gate:** demo shipped, downloads > 0 without a personal network, feedback collected.

---

## Phase 5 — Full Game (unbounded)

Only opens after Phase 3+4 succeed. Expansion order from core §45–46 (Revolution → WWII → …). Each new era is its own vertical slice cycle — never parallel.

---

## Standing Guidelines

**Working agreements**
- Commit daily; every phase gate = one commit tagged `phase-N-done`.
- New mechanic → new `ScriptableObject` + a `content/` markdown file (per templates); no hard-coded design values.
- Any contradiction discovered between docs → resolve in **ggd.md §56.1 register** first, then code.
- Fun test beats plan: if a step's output isn't fun when played, fix or cut it before proceeding — that's allowed by every exit gate.

**Anti-scope (things that break this plan)**
- ❌ Building backend "since I know Laravel" → it's ruled out until Phase 4 ends (ggd §56.6).
- ❌ Modeling Kai before combat timing is locked → wrong spec gets baked into art (Iron Rule 3).
- ❌ Adding a second enemy type to "make combat varied" before Phase 1 exit → depth-first, not width-first.
- ❌ Touching mobile/web before the demo ships → Iron Rule 1.
- ❌ "One more mythological creature" — roster is closed at ggd §7 + §8 lists; new creatures need a §56.1 row.

**If stuck, in order:** re-read the current phase's exit gate → cut scope to meet it → ask a tester to play what exists. Never extend the phase to fit the scope.
