# BAYANI

## Game Design Document — Vertical Slice

> ## ⚙ CANONICAL SPEC — SINGLE SOURCE OF TRUTH
> **This document is authoritative.** `core_of_bayani.md` (Design Rationale Appendix) and `blueprints.md` (Production & Narrative Appendix) are supporting documents. Where they disagree with this file or with each other, the resolutions in **§56** apply. All three docs were reconciled on 2026-09-27.

**Working Title:** BAYANI
**Subtitle:** *Echoes of Mactan*
**Genre:** Action-Adventure RPG
**Subgenres:** Historical Fantasy / Exploration / Third-Person Combat / Narrative Adventure
**Target Platforms:** Windows, macOS, Android, iOS, Web
**Engine:** Unity 6
**Target Audience:** Teens, young adults, families, Filipino gamers, history/culture enthusiasts
**Initial Setting:** Philippines, 2187 → 1521
**First Major Historical Event:** The Magellan-Elcano expedition and the Mactan conflict
**Primary Language:** Filipino / English
**Camera:** Third-person

---

# 1. HIGH CONCEPT

BAYANI is a third-person action-adventure RPG about a teenager from the future who discovers a technology capable of entering reconstructed memories of Philippine history.

When the historical archive begins collapsing, the player travels to 1521 to investigate the first encounters between the expedition of Magellan and communities in the Philippine archipelago.

But something is wrong.

The historical memory has been corrupted.

Ancient creatures from Filipino mythology are appearing inside the reconstruction.

The player must explore villages, forests, caves, beaches and ancient settlements; recover lost artifacts; fight supernatural creatures; solve environmental puzzles; meet historical figures; and ultimately survive the events leading to the Battle of Mactan.

The player does not rewrite history.

Instead, the player discovers the hidden stories surrounding it.

---

# 2. CORE GAME PILLARS

## Pillar 1 — ADVENTURE

Explore a stylized recreation of 1521 Philippines.

Players can discover:

* villages
* forests
* beaches
* caves
* rivers
* ancient structures
* hidden shrines
* trading areas
* archaeological locations
* secret paths
* treasure locations

---

## Pillar 2 — BATTLE

Real-time third-person combat.

The player fights:

* Limot
* corrupted spirits
* mythological creatures
* corrupted wildlife
* supernatural bosses

Historical humans are not turned into generic monster enemies.

---

## Pillar 3 — HISTORY

The player encounters documented historical events and people.

The game distinguishes:

**HISTORICAL RECORD**

from

**GAME FICTION**

and

**FOLKLORE / MYTHOLOGY**

This distinction becomes part of the game's Codex.

---

## Pillar 4 — HERITAGE

Players discover objects inspired by Philippine archaeological and cultural heritage.

Examples:

* pottery
* beads
* ornaments
* trade ceramics
* tools
* weapons
* gold objects
* burial-related artifacts
* wooden objects
* boats
* writing-related discoveries
* anito-inspired sculptures

---

## Pillar 5 — FILIPINO MYTHOLOGY

Mythological creatures become part of the supernatural layer of the game.

Potential creatures:

* Duwende
* Kapre
* Tikbalang
* Manananggal
* Aswang-inspired entities
* Diwata
* Bakunawa
* elemental spirits
* Anito
* corrupted guardian spirits

The game should avoid presenting one mythology as universally shared across all Philippine cultures. Different regions and ethnolinguistic traditions can have different beings, names and interpretations.

---

# 3. STORY PREMISE

## YEAR 2187

The Philippines is one of the world's most technologically advanced societies.

History is no longer primarily stored in books.

It is stored inside the:

# PHILIPPINE MEMORY ARCHIVE

The archive contains reconstructed historical environments.

Students can virtually experience:

* ancient settlements
* historical events
* famous figures
* cultural traditions

One day, the archive experiences a catastrophic failure.

Thousands of historical memories disappear.

Then something impossible happens.

The system detects entities that should not exist.

They are classified:

# UNKNOWN ENTITY: LIMOT

The creatures appear to be feeding on damaged memories.

---

# 4. MAIN CHARACTER

## KAI

Age: 16

A Filipino teenager from 2187.

### Personality

* curious
* sarcastic
* intelligent
* impatient
* technologically capable
* initially disconnected from history
* gradually becomes more empathetic

Kai has grown up with simulated historical education.

He knows historical names.

He doesn't understand what those people actually lived through.

That changes during the journey.

---

# 5. TALA

TALA is Kai's AI companion.

She exists inside the BALIKAN device.

### Functions

* historical database
* environmental scanner
* enemy scanner
* translation
* quest tracking
* artifact identification
* combat analysis
* navigation
* lore explanation

### Personality

TALA is intelligent but occasionally literal.

> **Canonical translation rule** (resolves `blueprints.md` §13): in 1521 areas TALA translates *approximately*, with explicit uncertainty ("dialect not fully matched — inferred meaning"). The language barrier is **atmosphere, never a mechanic** — no untranslatable quests or dead ends.

Example:

Kai:

> "How many enemies?"

TALA:

> "Thirty-two."

Kai:

> "You could've warned me!"

TALA:

> "You asked for information, not a warning."

---

# 6. BALIKAN

The BALIKAN is the time-memory traversal device.

It does not literally send Kai into an alternate physical universe.

Instead, it allows Kai to enter a reconstructed historical memory.

However, because the archive is damaged, the reconstruction begins behaving unpredictably.

This gives the game permission to introduce fantasy.

Historical environment:

**REALITY DATA**

*

Historical uncertainty:

**MEMORY GAP**

*

Mythology:

**CULTURAL MEMORY**

*

Corruption:

**LIMOT**

=

BAYANI'S GAME WORLD

---

# 7. THE LIMOT

The primary supernatural enemy faction.

"Limot" represents forgotten or lost memories in the game's fictional terminology.

They are not historical beings.

They are manifestations created by corrupted memory.

## Enemy Classes

### Anino

Basic shadow creature.

Fast and weak.

### Limot Lingid

Stealth enemy.

Can disappear into shadows.

### Limot Bantay

Heavy armored creature.

Protects important artifacts.

### Limot Lumilipad

Flying enemy.

### Limot Mangkukulam

Ranged magical enemy.

### Limot Tagapuksa

Elite enemy.

### Memory Guardian

Mini-boss.

> **Canonical naming:** this Filipino-named roster supersedes the English archetype names in `core_of_bayani.md` §15. Mapping: Basic→Anino, Hunter→Limot Lingid, Flyer→Limot Lumilipad, Guardian→Limot Bantay, Shaman→Limot Mangkukulam, Warden→Limot Tagapuksa.

---

# 8. MYTHOLOGICAL CREATURE SYSTEM

Mythological creatures should not all be enemies.

Some can be:

* neutral
* friendly
* quest-givers
* guardians
* merchants
* bosses
* hidden encounters

## Duwende

Small hidden creatures.

Gameplay:

* trap puzzles
* treasure clues
* secret passages
* misdirection

## Kapre

Large forest guardian.

Potentially neutral.

The player can earn its trust.

## Tikbalang

Fast forest creature.

Could become a mini-boss or challenge encounter.

## Diwata

Rare supernatural NPC.

Provides lore, blessings and quests.

## Anito

Spirit/ancestor representation.

Some are guardians.

Some are corrupted.

## Bakunawa

Major legendary boss.

Recommended as a later boss rather than the first major encounter.

---

# 9. CHAPTER 0 — THE FUTURE

## Location

Neo-Manila, 2187.

The player starts in a futuristic city.

### Gameplay

Tutorial:

* movement
* camera
* interaction
* dialogue
* inventory
* scanning
* combat tutorial
* BALIKAN activation

Kai discovers the damaged archive.

TALA identifies:

> TEMPORAL MEMORY: 1521

The player enters the BALIKAN.

---

# 10. CHAPTER 1 — THE FIRST MEMORY

## Setting

1521.

The player arrives near the beginning of the expedition's interaction with communities in the archipelago.

The chapter establishes:

* maritime travel
* local communities
* trade
* clothing
* weapons
* boats
* environment
* social structures
* cultural objects

The player discovers that the world is much more complex than the history lessons of the future suggested.

---

# 11. CHAPTER 2 — LIMASAWA

The player explores the island environment.

Historical events form the background.

The player is not the person who causes those events.

### Gameplay

Explore:

* coastline
* village
* forest
* hills
* shoreline
* hidden cave

### Missions

**Quest 01 — Strange Footprints**

Something has been attacking fishermen.

The player follows the tracks.

First supernatural encounter.

---

**Quest 02 — The Missing Trader**

A trader disappeared while travelling through the forest.

The player searches for him.

---

**Quest 03 — The Ancient Vessel**

The player discovers a hidden archaeological-style artifact.

TALA scans it.

The object is added to the Heritage Codex.

---

# 12. CHAPTER 3 — CEBU

The player arrives in a much larger settlement.

This becomes the first semi-open hub.

## Cebu Hub

```text
                 CEBU
                   |
       ┌───────────┼───────────┐
       |           |           |
    Village     Market       Coast
       |           |           |
    Shrine       Harbor      Jungle
       |
    Hidden Cave
```

Players can:

* buy items
* upgrade equipment
* talk to NPCs
* accept side quests
* discover artifacts
* train
* craft
* learn lore

---

# 13. CHAPTER 4 — THE ROAD TO MACTAN

The player begins hearing about conflict.

The political situation becomes increasingly tense.

The player learns that different leaders have different interests.

Historical characters should be presented with contextual nuance rather than simplified "good vs evil" labels.

---

# 14. LAPU-LAPU

Lapu-Lapu becomes the major historical figure of the first game chapter.

He should not appear as a fantasy superhero.

He should be presented as a leader within the historical setting.

The player can interact with him.

### First encounter

Kai initially doesn't recognize him.

TALA identifies:

> "Historical record match: Lapulapu."

Kai:

> "That's him?"

TALA:

> "One of the leaders of Mactan."

Kai:

> "He doesn't look like the statue."

TALA:

> "Statues are interpretations."

---

# 15. MACTAN

The final area of the vertical slice.

## Environment

* beach
* mangrove
* village
* forest
* rocky coastline
* elevated areas
* caves
* shoreline
* defensive areas

The player prepares for the approaching conflict.

---

# 16. MACTAN BATTLE

The historical battle takes place on:

# 27 APRIL 1521

The NHCP records the battle as the confrontation in which Mactan warriors killed Magellan.

The player does not replace Lapu-Lapu.

Instead:

### PLAYER OBJECTIVE

Protect the memory.

Kai fights supernatural corruption appearing around the historical event.

This allows:

**History**

and

**Action Gameplay**

to coexist.

---

# 17. MACTAN BOSS

## THE MEMORY DEVOURER

*(formerly draft-named "The Corrupted Anito")*

The final supernatural enemy of the vertical slice.

Important:

This is **not** presented as the real historical religious practice.

It is a fictional manifestation created by the corrupted archive.

The creature has absorbed fragments of lost memories.

> **Canonical resolution:** the boss named "The Corrupted Anito" in earlier drafts and the "Memory Devourer" named in `core_of_bayani.md` §36 / `blueprints.md` §34 are **the same entity**. Canonical name: **THE MEMORY DEVOURER**. "Corrupted Anito" is retained only as the name of the **cave mini-boss** in the demo flow (§48). Phase 4 ends with the recovered-memories sequence (Limasawa → Cebu → Mactan) from `blueprints.md` §34.

### Phase 1

Human-sized spirit.

### Phase 2

Large guardian.

### Phase 3

Corrupted memory monster.

### Phase 4

Memory collapse.

The player must recover the final Memory Fragment.

---

# 18. FINAL SCENE

The historical battle concludes.

Kai watches the memory stabilize.

He realizes:

He cannot change what happened.

But he can remember it.

TALA:

> "Memory restored."

Kai:

> "Is that what being a bayani means?"

TALA:

> "I don't know."

Kai:

> "You're the AI. You're supposed to know."

TALA:

> "Perhaps that's something you have to discover."

CUT TO BLACK.

# BAYANI

---

# 19. COMBAT SYSTEM

## Basic

Light Attack

Heavy Attack

Dodge

Block

Parry

Special Skill

Ultimate

Interact

---

# 20. COMBAT RESOURCE

## DIWA

Diwa represents the player's spiritual/mental energy.

Used for:

* special attacks
* artifact abilities
* Bayani skills
* spirit interactions

---

# 21. BAYANI MEMORY SYSTEM

Each major historical encounter unlocks a Memory.

Example:

## LAPU-LAPU MEMORY

**Skill: PANININDIGAN**

Temporarily increases:

* defense
* stagger resistance
* melee damage

The player isn't literally receiving supernatural powers from Lapu-Lapu.

The Memory system represents Kai learning from the historical figure.

---

# 22. ARTIFACT SYSTEM

Artifacts are divided into categories.

## HERITAGE

Real-world archaeological/cultural inspiration.

Examples:

* pottery
* beads
* ornaments
* trade ceramics
* tools
* burial objects
* gold ornaments
* weapons
* boats

The National Museum's collections provide a useful research foundation for these categories.

---

# 23. LEGENDARY ITEMS

These are fictional game items.

### ANITO FIGURE

Category:

Spirit Artifact

Ability:

**Ancestor's Protection**

Creates a temporary defensive field.

---

### BATHALA RELIC

Category:

Mythic Artifact

Ability:

**Sky's Favor**

Restores Diwa.

Important design note:

The game should clearly identify this as a **mythological/fantasy interpretation**, not a documented archaeological artifact.

---

### DIWATA'S VEIL

Increases exploration detection.

---

### KAPRE'S TOKEN

Reveals hidden objects in forests.

---

### DUWENDE'S COIN

Allows access to hidden treasure paths.

---

# 24. TREASURE SYSTEM

Players discover:

### Common

* food
* crafting materials
* coins
* basic artifacts

### Rare

* ornaments
* weapons
* special materials

### Epic

* historical artifact replicas/inspired objects
* unique equipment

### Mythic

* supernatural relics

---

# 25. CODEX

The Codex is extremely important.

It has seven tabs:

```text
HISTORY
PEOPLE
PLACES
HERITAGE
MYTHOLOGY
CREATURES
ARTIFACTS
```

> **Canonical** (supersedes the 4-tab draft list in this section's UI mockup §34 and any `CHARACTERS` naming; "CHARACTERS" is folded into "PEOPLE").

Each entry clearly identifies its category.

Example:

### HISTORY

**Mactan**

Historical Record

---

### HERITAGE

**Earthenware Vessel**

Archaeological Inspiration

---

### MYTHOLOGY

**Kapre**

Folklore / Mythological Interpretation

---

# 26. PLAYER PROGRESSION

```text
LEVEL
  |
  +-- HP
  +-- STAMINA
  +-- DIWA
  +-- ATTACK
  +-- DEFENSE
  |
  +-- SKILLS
  +-- WEAPONS
  +-- ARTIFACTS
  +-- BAYANI MEMORIES
```

---

# 27. SKILL TREE

Three branches:

## MANDIRIGMA

Combat

* Basic Attack
* Heavy Attack
* Counter
* Perfect Dodge
* Combo Mastery

## MANLALAKBAY

Exploration

* Climb
* Swim
* Track
* Treasure Sense
* Spirit Sight

## TAGAPAGMANA

Memory / Spirit

* Diwa Recovery
* Memory Shield
* Artifact Resonance
* Ancestor's Call
* Memory Burst

> **Canonical node names** (supersedes the divergent lists in `core_of_bayani.md` §21 and earlier drafts of this file). Skill-point budget for the slice is defined in **§56.4**.

---

# 28. UI/UX DESIGN LANGUAGE

## Visual Direction

**Future Filipino + Precolonial Filipino + Fantasy**

The UI should transition depending on the player's era.

### Future

* holographic
* glass panels
* cyan/blue light
* geometric shapes

### Past

* wood
* woven patterns
* carved motifs
* natural materials
* subtle Baybayin-inspired decorative elements

Don't use Baybayin purely as random decoration. Where actual text is used, it should be linguistically meaningful.

---

# 29. MAIN MENU

```text
┌──────────────────────────────────────────────┐
│                                              │
│                    BAYANI                    │
│              ECHOES OF MACTAN                │
│                                              │
│                 [ CONTINUE ]                 │
│                                              │
│                  NEW GAME                    │
│                                              │
│                  CODEX                       │
│                                              │
│                  SETTINGS                    │
│                                              │
│                  EXIT                        │
│                                              │
└──────────────────────────────────────────────┘
```

Background:

Kai standing in front of the BALIKAN.

Behind him:

Future Manila.

Inside the portal:

1521 Philippines.

---

# 30. CHARACTER HUD

```text
┌──────────────────────────────────────────────┐
│ KAI                                          │
│ ██████████ HP                                │
│ ███████░░░ STAMINA                           │
│ █████░░░░░ DIWA                              │
│                                              │
│                              MINI MAP        │
│                            ┌─────────┐       │
│                            │   ◉     │       │
│                            │     △   │       │
│                            └─────────┘       │
│                                              │
│                                              │
│                                              │
│                                              │
│                               ○ ATTACK       │
│                        □ SKILL    △ DODGE   │
│                               × JUMP         │
│                                              │
└──────────────────────────────────────────────┘
```

For desktop, the same HUD adapts to keyboard/controller.

---

# 31. COMBAT HUD

When combat starts:

```text
Enemy
THE LIMOT
████████████████████

                  KAI
              HP ████████

            [TARGET LOCK]
```

Boss fights get:

```text
BOSS NAME
════════════════════════════
████████████████████████████
PHASE 2
```

---

# 32. INVENTORY

```text
┌──────────────────────────────────────────────┐
│ INVENTORY                                     │
├──────────────┬───────────────────────────────┤
│ WEAPONS      │                               │
│ ARTIFACTS    │        ITEM VIEW              │
│ MATERIALS    │                               │
│ QUEST        │        KAMPILAN               │
│ TREASURE     │                               │
│              │        Damage +25             │
│              │        Durability 87%         │
│              │                               │
└──────────────┴───────────────────────────────┘
```

---

# 33. BAYANI MEMORY SCREEN

```text
┌──────────────────────────────────────────────┐
│              BAYANI MEMORIES                 │
│                                              │
│   [ Lapu-Lapu ]  [ ??? ]  [ ??? ]            │
│                                              │
│   LAPU-LAPU                                   │
│   Leader of Mactan                            │
│                                              │
│   MEMORY                                     │
│   PANININDIGAN                               │
│                                              │
│   Defense +20%                               │
│                                              │
│              [ EQUIP ]                       │
└──────────────────────────────────────────────┘
```

---

# 34. CODEX UI

```text
┌──────────────────────────────────────────────┐
│ CODEX                                        │
├──────────────────────────────────────────────┤
│ [HISTORY] [HERITAGE] [MYTHOLOGY] [PEOPLE]   │
│                                              │
│ ┌────────┐ ┌────────┐ ┌────────┐             │
│ │ MACTAN │ │ ANITO  │ │ KAPRE  │             │
│ └────────┘ └────────┘ └────────┘             │
│                                              │
│             ENTRY DETAILS                    │
│                                              │
│     Image / 3D Artifact                     │
│                                              │
│     Description                              │
│     Discovery                                │
│     Related Quest                            │
│     Historical / Mythological status         │
└──────────────────────────────────────────────┘
```

---

# 35. WORLD MAP

The map is not just geographic.

It is a **timeline map**.

```text
                     2187
                       │
                       ↓
                    LIMASAWA
                       │
                       ↓
                     CEBU
                       │
                       ↓
                    MACTAN
                       │
                       ↓
                  MEMORY CORE
```

Later:

```text
1521 ─── Revolution ─── WWII ─── Modern ─── Future
```

---

# 36. QUEST UI

```text
QUEST

THE MEMORY OF MACTAN

Objective:

□ Find the missing artifact
□ Investigate the forest
□ Defeat the Limot Guardian
□ Return to the village

Reward:

XP
Artifact
Memory Fragment
```

---

# 37. DIALOGUE UI

Characters appear on screen rather than using a static RPG dialogue box.

```text
                LAPU-LAPU

        "You do not belong here."

KAI:
"I could say the same about your giant
shadow monster."

              TALA:
"Technically, the creature is
approximately 87% anomalous."

                 [Continue]
```

---

# 38. ARTIFACT DISCOVERY

When an important object is found:

```text
              ✦ ARTIFACT FOUND ✦

                  ANITO

             [3D OBJECT]

          HERITAGE / SPIRIT

       "A representation associated
        with indigenous spiritual
        traditions."

             [ADD TO CODEX]
```

---

# 39. TREASURE CHEST

Avoid generic European fantasy chests.

Treasure containers should reflect the environment:

* woven containers
* wooden boxes
* clay vessels
* carved containers
* hidden compartments
* burial-related containers where appropriate to fictional gameplay

The actual historical context of burial objects should be researched carefully rather than turning graves into generic loot containers.

---

# 40. MOBILE UX

Mobile should use:

```text
LEFT
Virtual joystick

RIGHT
Attack
Dodge
Skill
Jump
Interact
```

Contextual buttons appear only when needed.

Example:

Approach NPC:

**TALK**

Approach treasure:

**SEARCH**

Approach ladder:

**CLIMB**

### Mobile combat rules (vertical slice)

Combo combat does not port 1:1 to a touchscreen. Canonical mobile adjustments:

* **Auto-target** nearest enemy in camera direction; no manual target cycling on mobile.
* **Assist options** (settings, not difficulty nerfs to content): generous parry/dodge windows, hit-flash readability mode, optional auto-dodge-on-release.
* **Context-sensitive skill button** — one skill slot shown at a time, swapped via long-press, instead of 4+ simultaneous action buttons.
* Full mobile control *feel* tuning is a Phase 2 (Android) deliverable per §41 of `core_of_bayani.md`, not a prototype task.

---

# 41. DESKTOP UX

Keyboard:

```text
W A S D     Movement
Mouse       Camera
LMB         Attack
RMB         Block/Aim
SPACE       Jump
SHIFT       Sprint
Q           Skill
E           Interact
TAB         Inventory
M           Map
J           Quest
C           Codex
```

Controller support should be included from the beginning rather than added after development.

---

# 42. GAME WORLD STRUCTURE

The first vertical slice should NOT be a giant open world.

Use:

### Semi-open zones

```text
Future Manila
      ↓
Limasawa
      ↓
Cebu Hub
      ↓
Mactan
      ↓
Final Memory Realm
```

Each zone can have:

* main quests
* side quests
* collectibles
* hidden caves
* mini-bosses
* treasure
* NPCs
* environmental puzzles

---

# 43. DEVELOPMENT STACK

## Game Engine

Unity 6

## Language

C#

## Rendering

URP

## Animation

Unity Animator + Timeline

## Cinematics

Timeline + Cinemachine

## AI

Unity NavMesh + custom state machines

## Audio

Unity Audio initially

FMOD can be introduced later if the audio design becomes more complex.

---

# 44. BACKEND

Laravel

```text
Authentication
Player profile
Cloud save
Achievements
Inventory
Quest progress
Codex
Bayani Memories
Analytics
DLC/content
```

Database:

MySQL

Cache:

Redis

Storage:

S3 / Cloudflare R2

---

# 45. API

```text
POST /api/auth/login

GET /api/player

GET /api/player/progress

POST /api/player/progress

GET /api/chapters

GET /api/quests

GET /api/artifacts

GET /api/memories

GET /api/codex

POST /api/save

GET /api/leaderboard
```

---

# 46. SAVE SYSTEM

Two levels:

### Local Save

Fast and works offline.

### Cloud Save

Synchronizes:

* level
* equipment
* quest progress
* artifacts
* codex
* memories
* achievements

---

# 47. MVP / VERTICAL SLICE

The first development target should be:

## BAYANI — ECHOES OF MACTAN

### Playtime

Approximately **60–90 minutes** for the full vertical slice (reconciles the 30–60 min / 1–2 h figures across drafts — see §56.1). The public demo is a **20–30 minute cut**: START → FUTURE MANILA → TUTORIAL → ARCHIVE FAILURE → TALA DISCOVERY → BALIKAN → TIME TRAVEL → 1521 → EXPLORE → FIRST LIMOT → ARTIFACT SCAN → cut to title. Items 1–20 below still define the full slice; **cloud save and all backend features are removed from slice scope** (§56.6).

### Includes

1. Future tutorial
2. BALIKAN
3. 1521 environment
4. One village
5. One forest zone
6. One cave
7. One beach
8. Basic combat
9. 4–6 enemy types
10. 1 mini-boss
11. 1 major boss
12. 5 main quests
13. 3 side quests
14. 10+ artifacts
15. 5+ treasure types
16. Codex
17. Bayani Memory system
18. Lapu-Lapu encounter
19. Mactan finale
20. Local save system only (no backend required)

---

# 48. FIRST PLAYABLE DEMO FLOW

```text
START
 ↓
FUTURE MANILA
 ↓
TUTORIAL
 ↓
ARCHIVE FAILURE
 ↓
TALA DISCOVERY
 ↓
BALIKAN
 ↓
TIME TRAVEL
 ↓
1521
 ↓
EXPLORE
 ↓
FIRST LIMOT
 ↓
VILLAGE
 ↓
SIDE QUEST
 ↓
FOREST
 ↓
DUWENDE ENCOUNTER
 ↓
CAVE
 ↓
ANITO ARTIFACT
 ↓
MINI-BOSS
 ↓
CEBU
 ↓
MACTAN
 ↓
LAPU-LAPU
 ↓
BATTLE
 ↓
CORRUPTED ANITO MINI-BOSS
 ↓
MEMORY DEVOURER BOSS
 ↓
MEMORY RESTORED
 ↓
RETURN TO FUTURE
 ↓
TITLE
```

---

# 49. ART DIRECTION

## Visual Style

Stylized cinematic 3D.

Not photorealistic.

Characters:

* expressive
* slightly exaggerated
* recognizable silhouettes

Environment:

* lush tropical vegetation
* detailed water
* dramatic skies
* warm sunlight
* fog
* firelight
* atmospheric particles

Future:

Clean futuristic architecture.

Past:

Natural materials, wood, bamboo, stone, woven textures and regionally appropriate architecture.

---

# 50. DESIGN PRINCIPLE

The game should feel:

**Filipino first.**

Not:

"generic fantasy game with Filipino names."

The environment, architecture, weapons, clothing, music, food, boats, artifacts, language, mythology and storytelling should all contribute to the identity.

---

# 51. CULTURAL AUTHENTICITY SYSTEM

Every major asset gets an internal classification:

### HISTORICAL

Supported by historical evidence.

### ARCHAEOLOGICAL

Based on documented archaeological material.

### ETHNOGRAPHIC

Based on documented cultural traditions.

### FOLKLORE

Based on recorded oral/literary traditions.

### FICTIONAL

Created specifically for BAYANI.

This classification can also appear inside the Codex.

---

# 52. THE GOLDEN RULE

BAYANI should never claim:

> "This fictional object definitely existed in 1521."

Instead:

> "Inspired by documented Philippine archaeological materials."

That gives the developers freedom to make the game exciting while respecting history.

---

# 53. FUTURE EXPANSIONS

After Mactan:

### BAYANI II

**The Revolution**

Bonifacio / Jacinto / Katipunan era.

### BAYANI III

**The Last Stand**

World War II.

### BAYANI IV

**The Forgotten Kingdoms**

Earlier precolonial/ancient settings.

### BAYANI V

**The Last Memory**

Future Philippines.

Eventually the entire series becomes a historical fantasy adventure across Philippine history.

---

# 54. LONG-TERM GAME VISION

BAYANI isn't simply:

> "A game about famous Filipino heroes."

The larger concept is:

# "An adventure through Filipino history, culture, heritage and mythology."

The player starts as someone who knows almost nothing about the past.

By the end, they understand why remembering it matters.

---

# 55. CORE TAGLINE

## BAYANI

### **"Before legends were written, they were lived."**

Alternative:

### **"Travel the past. Fight the forgotten. Discover the Bayani."**

Alternative:

### **"The past is calling."**

---

# 56. CANONICAL RESOLUTIONS & SYSTEM DEFINITIONS

*Added 2026-09-27 during the document reconciliation pass. This section resolves every known conflict between `ggd.md`, `core_of_bayani.md` and `blueprints.md`, and defines mechanics that were previously named but never specified.*

## 56.1 Conflict Resolution Register

| # | Topic | Conflict | **Canonical decision** |
|---|-------|----------|------------------------|
| 1 | Vertical slice length | 15-min opening (core §3) vs 1–2 h (blueprints §50) vs 30–60 min (ggd §47) | **Slice = 60–90 min.** The "first 15 minutes" is the opening *pacing target*, not a scope. Public demo = 20–30 min cut (§47). |
| 2 | Final boss | "Memory Devourer" (core §36, blueprints §34) vs "Corrupted Anito" (ggd §17) | **One entity: THE MEMORY DEVOURER.** "Corrupted Anito" demoted to the cave mini-boss. Phase 4 uses the recovered-memories sequence (blueprints §34). |
| 3 | Codex tabs | 7 categories (core §29, blueprints §43) vs 4 tabs (ggd §25 old) | **7 tabs:** HISTORY, PEOPLE, PLACES, HERITAGE, MYTHOLOGY, CREATURES, ARTIFACTS. |
| 4 | Skill tree nodes | core §21 vs ggd §27 differed on Tagapagmana/Manlalakbay nodes | **core §21 names adopted** (canonical list now in ggd §27). |
| 5 | Limot roster | English archetypes (core §15) vs Filipino names (ggd §7) | **Filipino names canonical** (ggd §7); mapping note added to core §15. |
| 6 | Rarity tiers | Common/Uncommon/Rare/Mythic (core §9) vs Common/Rare/Epic/Mythic (blueprints §27) | **5 tiers:** Common, Uncommon, Rare, Epic, Mythic. |
| 7 | First artifact | "Earthenware" vs "ceramic vessel" vs "Ancient Vessel" quest | **MEMORY #001 = EARTHENWARE (hand-in). The trade ceramic is a *separate, later* collectible** that unlocks the Trade Routes map reveal. Quest 03 "The Ancient Vessel" leads to the earthenware. |
| 8 | Language barrier | blueprints §13 (TALA can't translate) vs ggd §5 (translation function) | TALA translates approximately with stated uncertainty; **atmosphere, never a mechanic** (ggd §5 note). |
| 9 | Endings | core §37 vs blueprints §36–37 narration | Both kept; **blueprints §36–37 is the canonical scene order** (Lapu-Lapu exchange → return → TALA "You changed nothing" → final narration). core §37 narration folds into the closing voice-over. |
| 10 | Platforms | 5 platforms listed everywhere | **Windows-only during prototyping and the slice** (core §41 Phase 1). Mobile/web/iOS/macOS targets remain, but no platform-specific work until the slice is fun on PC. |

## 56.2 Core Resources — HP, Stamina, Diwa

Three bars, three non-overlapping jobs:

| Resource | Drained by | Regenerated by |
|---|---|---|
| **HP** | Enemy damage | Potions, rest at shrines, Memory Burst heal, certain artifacts |
| **Stamina** (cap 100) | Dodge −20, heavy attack −15, block drain while held, sprint −10/s | Auto-regen **25/s**, delayed 0.8 s after last stamina spend or hit taken; regen paused while blocking/sprinting |
| **Diwa** (cap 100, +10/level → 190 at L10) | Active skills only: Memory Shield 30, Artifact Resonance 25, Ancestor's Call 40, Memory Burst 50, Spirit Sight 5/s channel | **Combat-gated gains only** (encourages the fight→scan→use loop): hit +2, perfect dodge +8, parry +12, enemy kill +6, artifact scan +15 (once per artifact), shrine rest full restore |

Design intent: Stamina is the **rhythm meter** (combat pacing), Diwa is the **earned meter** (you get it back by playing aggressively and curiously). Diwa never regenerates passively.

## 56.3 Memory Stability

**Definition:** a per-zone score (0–100%) measuring how intact the current reconstructed memory is. It is **not a health bar and never causes failure/game-over**.

**What changes it:**

* Starts at 100% − 20% in every zone (the archive is already damaged on arrival).
* **− stability:** Limot present in the zone over time (−1/min per living corrupted-objective nest), destroying environment props, failing a defense objective.
* **+ stability:** defeating Limot (+5 each), completing quests (+10), scanning artifacts (+3), restoring shrines (+10), completing memory trials (+15).

**What it does:**

* **Visual/audio gradient:** lower stability = heavier desaturation, glitch artifacts, fog, distorted ambient audio.
* **Encounter density:** Limot spawn rate scales with instability (capped so zones never become spawn farms).
* **Rewards:** end-of-zone bonus XP = base × (0.5 + stability%). Codex "integrity notes" vary with stability (low-stability scans show TALA uncertainty lines).
* **Displays** on the Mactan battlefield HUD (blueprints §33) and save screen (§49) exactly as mockuped — those mockups are now spec-compliant.

## 56.4 Progression Budget — One XP, One Currency, 9 Skill Points

* **Single XP currency** named **XP** (retires "Memory XP" — codex/artifact awards are just XP). L1→L10 total: **5,000 XP**, tuned so the slice's scripted content lands the player at ~L8 with exploration/codex reaching L10. Sources: kills 20–150, quests 250–800, artifacts 50, bosses 500–1,200.
* **Skill points:** +1 per level-up from L2 → **9 points across 15 nodes** in the slice. Players must specialize (~60% of one branch). Nodes 4–5 of each branch require the previous node; no point is ever forced-spent.
* **Currency: one gold unit, working name *patos*** (placeholder — final name pending cultural/linguistic review). Earned from quest rewards, selling treasure, duwende stakes; spent at the Cebu blacksmith/merchant. **No paywalls, no premium currency, ever.**
* **Durability: OFF in the vertical slice.** `WeaponData.Durability` remains a reserved field (default 100, unused) so future chapters can add degradation without schema changes. Blacksmith upgrades are permanent.

## 56.5 Unity Project Structure (Corrected)

The structure in `core_of_bayani.md` §38 is **invalid** — Unity only imports from `Assets/`. Canonical layout:

```text
BAYANI/
├── .gitignore
├── .gitattributes
├── ProjectSettings/
├── Packages/
├── UserSettings/
└── Assets/
    ├── Scenes/
    │   ├── 00_FutureManila.unity
    │   ├── 01_Balikan.unity
    │   ├── 02_Limasawa.unity
    │   ├── 03_Cebu.unity
    │   ├── 04_Mactan.unity
    │   └── 05_MemoryVoid.unity
    ├── Scripts/
    │   ├── Core/ Player/ Combat/ Enemy/ NPC/
    │   ├── Quest/ Dialogue/ Artifact/ Inventory/
    │   ├── Skills/ Codex/ Save/ API/
    ├── Art/
    │   ├── Characters/ Environment/ Props/ Weapons/ UI/ VFX/
    ├── Audio/
    │   ├── Music/ SFX/ Voice/ Ambient/
    ├── Data/                     ← ScriptableObjects
    │   ├── Characters/ Weapons/ Artifacts/
    │   ├── Quests/ Enemies/ Dialogue/ Codex/
    ├── Resources/                ← keep minimal; prefer Addressables
    ├── Editor/                   ← custom inspectors, data validators
    └── Tests/                    ← PlayMode + EditMode tests
```

## 56.6 Backend Scope Ruling — Slice Is Offline-Only

**The vertical slice ships with zero server dependency.**

* **In slice scope:** local save only (JSON on disk; §46 "Local Save").
* **Deferred to post-slice (Phase 3+):** the entire Laravel/MySQL/Redis/S3 stack (§44), the API list (§45), cloud save sync, achievements, leaderboards, analytics.
* **Data ownership rule:** **ScriptableObjects are the single source of truth** for all game content (quests, artifacts, codex entries, dialogue, enemies, weapons). If/when the backend returns, the database tables in `blueprints.md` §41–43 become **mirrors for telemetry/progress only** — never a second authoring surface. Content is never defined in two places.
* **Cloud save (future):** per-slot **last-write-wins with slot timestamp** is the default rule; revisit before any cross-buy or cross-progression feature.

## 56.7 Source References

The `[1]`–`[7]` citation markers used across all three documents resolve to:

* **[1][6]** NHCP — National Historical Commission of the Philippines, historical marker records (Mazaua/Limasawa arrival Mar 28 1521; Easter Sunday mass Mar 31; departure Apr 4; Battle of Mactan Apr 27 1521; Lapulapu as leader of Mactan): https://www.nhcp.gov.ph / historic sites registry: http://nhcphistoricsites.blogspot.com/
* **[2][3][4][5][7]** National Museum of the Philippines — archaeological collections (earthenware, foreign trade ceramics from China/Vietnam/Thailand, beads, gold and bronze ornaments, burial goods): https://www.nationalmuseum.gov.ph/our-collections/archaeology/

## 56.8 Immediate Next Step

Per `core_of_bayani.md` §47: build the **5-minute combat + discovery prototype** in Unity 6 (URP, Windows-only, graybox environment, one swing set, one Anino, one artifact scan, no backend, no story cinematics beyond placeholder text). Green light for the vertical slice = that sequence feels good.
