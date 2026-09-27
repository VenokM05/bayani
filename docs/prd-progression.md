# BAYANI — PRD: Progression, Death & Active Skills

**Status:** implemented (graybox) · **Registered:** ggd §56.1 #13 · **Date:** 2026-09-27
**Scope:** Level/XP system, death & respawn, active skill system (keys 1–5),
armed/unarmed skill categories, animation integration requirements, HUD health
bars. All numbers live in ScriptableObjects — tuning happens in the Inspector.

---

## 1. Level & Experience System

**One XP currency** (ggd §56.4 — "Memory XP" is retired). The slice runs **L1→L10
on a 5,000 XP budget**; Chapter 1's scripted content is tuned to land ~L8, with
codex/exploration closing the gap.

| Source | Band (§56.4) | Current data |
|---|---|---|
| Kill | 20–150 | `EnemyData.xpOnKill` (Anino 40, … boss see below) |
| Boss / mini-boss | 500–1,200 | Corrupted Anito = 500 |
| Quest / story beat | 250–800 | `DialogueAsset.xpReward`, granted by `StoryTrigger` on sequence finish |
| Artifact scan | 50 | `ArtifactData.xpReward`, granted by `ArtifactScanner` |

**Level curve** — `XpTable_Slice.asset` (`Assets/Data/Progression/`):
`xpToNext = {190, 240, 300, 380, 475, 590, 740, 925, 1160}` → sums to exactly 5,000.

**Per level-up** (all fields on the XpTable SO):
- +10 max HP (applied via `CombatResources.GrowMaxHP`, current HP heals along)
- +2 stamina regen/sec (base 25)
- +1 skill point banked in `ProgressStore` (spend UI = the §27 15-node passive
  tree, Phase 3.6 — points accumulate now, nothing is forced-spent)
- full HP/stamina restore + screen flash + HUD toast

**State lives in `ProgressStore` (static):** level, XP-inside-level, skill points,
checkpoint. It survives death *and* scene loads; components re-apply saved level
bonuses on `Awake`, so a reloaded scene resumes at the right power level.

Skills unlock by level (§3 table) — this is the "unlock skill slots" reward until
the passive tree UI exists.

## 2. Death & Respawn

`DeathManager` (on the story host in every playable scene):

1. HP hits 0 → `CombatResources.OnDeath` fires once → controls locked, combat
   and skills disabled.
2. **"YOU DIED"** overlay, ~2.5 s (real time), fade-in vignette.
3. Respawn at the **last visited `Checkpoint`** (trigger volume; every scene has
   `Checkpoint_Spawn` at the player start, installed automatically):
   - same scene → teleport, full HP + stamina;
   - different scene → `LoadScene` then place (falls back to revive-in-place if
     the scene isn't in Build Settings yet — the graybox workflow).
4. **Kept on respawn:** XP, level, skill points, unlocked skills, Diwa (it is the
   earned meter), inventory (arrives Phase 3). **2 s mercy invulnerability.**
5. **No XP loss, no retry counter.** BAYANI punishes with time, not progress —
   aligned with the 07B "pressure without punishment" tutorial rule.

## 3. Active Skill System — keys 1–5

`SkillData` SO (`Assets/Data/Skills/`, created per `BAYANI/Skill` menu) — fields:
name, category, unlock level, **cost type (Stamina | Diwa)**, cost, cooldown,
damage, range, knockback, **hitDelay/hitDuration** (hitbox timing = script-side
animation events), **animatorParam**, **animationClip**, cast VFX color, cast sound.

Rules (`PlayerSkills` on Kai):
- **5 slots, keys 1–5** (gamepad: dpad 1–4 + B for 5). Slot contents depend on
  weapon state (§4); only one cast at a time; cannot cast during combo/dodge/block.
- **Stamina is the shared pool like mana**: skills drain it, it regenerates
  (delay 0.8 s after spend — §56.2 rhythm), and a skill **only fires when fully
  affordable**. Diwa-cost skills draw the earned meter instead.
- Cooldowns tick per slot; the HUD shows them (§6). Locked skills show their level.
- Cast = short lunge → hitbox opens at `hitDelay` for `hitDuration` through the
  existing Hitbox/Hurtbox damage pipeline (parry/stagger/kill rewards unchanged)
  → graybox VFX flash + optional audio cue.

## 4. Armed vs Unarmed Categories

**G toggles ready/stow weapon** (`PlayerSkills.Armed`). The two bars are mutually
exclusive — armed hides unarmed skills and vice versa; slot **5 is RESERVED** for
future skills or passives on both bars (only 4+4 exist by design).

| # | Armed (weapon equipped) | Lvl | Cost | CD | Dmg | Unarmed (stowed) | Lvl | Cost | CD | Dmg |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | **Solo Baston** (single stick) | 1 | 15 stam | 4 s | 12 | **Suntok** (punch) | 1 | 10 stam | 3 s | 8 |
| 2 | **Espada Y Daga** (sword+dagger) | 3 | 25 stam | 8 s | 20 | **Dumog** (wrestle/grapple) | 2 | 20 stam | 7 s | 16 |
| 3 | **Dos Manos** (two-handed) | 5 | 25 diwa | 12 s | 32 | **Buno** (throw) | 4 | 20 diwa | 10 s | 24 |
| 4 | **Baraw** (knife, fast) | 7 | 10 stam | 3 s | 8 | **Sipa** (kick) | 6 | 15 stam | 5 s | 12 |
| 5 | *reserved* | — | — | — | — | *reserved* | — | — | — | — |

Flavor hooks: Dos Manos (kb 5) and Buno (kb 6) stagger even the Corrupted Anito's
Shadow-call; Baraw is the quick poke; the two Diwa skills are the finishers.
**Q remains the Diwa Burst** (established binding, unchanged). All values tunable
on the SOs.

## 5. Animation Integration

Each `SkillData` carries `animatorParam` (`SkillArmed1..4`, `SkillUnarmed1..4`,
Trigger) + optional clip reference. `PlayerSkills`:
- sets the Trigger only if the Animator actually has the parameter (no warnings);
- plays `clip` via the Animator when assigned and a rig exists;
- **procedural fallback while kai.fbx is unrigged:** the lunge drives the motion,
  the VFX flash sells the contact frame — the graybox rule.
- Hit timing is script-owned (`hitDelay`) so animation swaps never break combat:
  align the clip's contact frame to `hitDelay`, or move `hitDelay` in the Inspector.
- Per-cast feedback today: expanding colored quad (SO color) + `AudioSource` clip
  if `castSound` assigned (silent until audio assets exist).

**Pending dependency:** Mixamo rig pass on Kai (asset tracker) → Animator + clip
import → the parameters above light up with zero code changes.

## 6. HUD (CombatHUD, graybox IMGUI)

- **Player bars with numbers:** `HP 78/100` (current/max as specified), stamina,
  diwa, memory stability, and a gold **`LVL n · XP x/need`** row.
- **Enemy health bars:** overhead, screen-projected, name label; drawn for every
  damaged, visible `IBattleHealth` (LimotEnemy and CorruptedAnito implement it;
  registry-based — no per-frame searches). Full-health enemies stay clean.
- **Skill bar:** 5 slots bottom-center showing key number, skill name (or
  `Lvl n` lock, or `RESERVED`), a top-down draining cooldown veil + seconds, and
  the ARMED/UNARMED mode line.
- Toast line for level-ups, slot feedback, respawn confirmation.

## 7. Installation & Scene Coverage

`Tools → BAYANI → Install Progression + Skills` (idempotent, run after any scene
rebuild): authors `XpTable_Slice` + the 8 skill SOs + quest XP on every
`DLG_SEQ*` asset + the §9 skeleton data (2 ItemData SOs, a QuestData template),
then patches **every scene containing a Kai with combat** (prototype, arena,
prologue, Cebu, Mactan + cave) with PlayerProgression, PlayerSkills, DeathManager,
`Checkpoint_Spawn`, and the §9 hosts: PlayerAutoHeal + StatUpgradeMenu on the
story host, one `InventoryHost` per scene.

## 8. Explicit non-scope (this pass)

Crafting and equipment slots on top of the §9 inventory skeleton (Phase 3.4), the
15-node passive tree UI that spends banked skill points (Phase 3.6), real
scan/attack VFX and audio (Tier-2 art), multiplayer-grade death telemetry, and any
change to §56.2/§56.3 resource semantics — Stamina and Diwa keep their established
definitions.

## 9. Phase 2 skeleton systems (Inspector-tunable)

Four lightweight systems, all data-driven via ScriptableObjects. Every value lives
on a component or SO — no tuning requires code. All components follow the
`ProgressStore` survival trick (static state rides through `LoadScene`); the
installer patches their hosts onto every playable scene.

### 9.1 Auto-heal — `PlayerAutoHeal` (story host)
Ticks every `tickSeconds` (default **1.5 s**) and heals a flat amount while HP is
below max and the player is alive: `heal = min(maxHeal, baseHeal + (Level-1)/3 * healPerThreeLevels)`
— i.e. **+2 HP/tick at L1, +1 more per 3 levels, capped at +8**. Diwa and Stamina
pools are untouched (§56.2 non-overlap); every point routes through the new
`CombatResources.Heal(amount)` which clamps to maxHP and fires
`OnResourceChanged("hp", Δ)`.

### 9.2 Inventory — `ItemData` SO + `Inventory` (DontDestroyOnLoad host)
`ItemData` (CreateAssetMenu **BAYANI/Item Data**): itemName, icon (Sprite, optional
while graybox), description, stackable, maxStack, hpBonus, staminaBonus. `Inventory`
holds a `List<ItemData>` of stacks (parallel counts); grants come from
`StoryTrigger.grantItem`, `ArtifactScanner.grantItem`, or enemy drops
(`EnemyData.dropItem` + `dropChance`, **0 = off**, opt-in per enemy). Stat bonuses
apply **once** on first pickup (hp → `GrowMaxHP`, stamina → the central regen formula).
State is static so it survives death and scene loads; a local JSON file
(`Application.persistentDataPath/bayani_inventory.json`, ggd §46 — no backend) is
written on every change and can be restored through a catalog. 8 graybox slots render
under the skill bar with icon/initial tile, stack count, and a hover tooltip.

### 9.3 Stat upgrade menu — `StatUpgradeMenu` (story host, key **I** / Tab / Start)
Overlay listing three upgradable stats — **Max HP (+10/lvl), Stamina Regen (+2/lvl),
Diwa Capacity (+10/lvl)**. Buying level *N* costs *N* skill points (linear, `costPerLevel`
multiplier); each purchase banks the level **and** the bonus total in `ProgressStore`
and applies immediately via `GrowMaxHP` / `RefreshStaminaRegen` / `GrowMaxDiwa`, then
re-applies on `Awake` after any reload. The menu is locked until **Lvl 2** (first skill
point); overspending shows the "No skill points remaining" toast.

### 9.4 Quest guide — `QuestData` SO + `QuestJournal` (static)
`DialogueAsset` gains `objectiveText` + `autoTrack`; when a `StoryTrigger` finishes a
sequence with `autoTrack`, the objective is added to a persistent sidebar
(top-right, under the FPS readout) as `[text] — Active/Complete`. `QuestData`
(CreateAssetMenu **BAYANI/Quest Data**) is the reward-bearing template: questId,
title, objectiveText, rewardXp, rewardDiwa, and a `completionCondition` enum
(**DialogueFinished / ArtifactScanned / EnemyKilledCount**). A later `StoryTrigger.questId`
marks it Complete (never the same beat that opened it); kills and scans advance the
counter conditions. No branching trees, no timers — sequential Phase 2 tracking only.
