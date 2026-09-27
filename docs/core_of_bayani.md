# BAYANI: ECHOES OF MACTAN

> ## 📎 DESIGN RATIONALE APPENDIX
> This document is **supporting material**. The canonical spec is [`ggd.md`](ggd.md) — see its **§56 Canonical Resolutions**. Where they differ, ggd.md wins. Notably superseded here: **§15 Limot names** (Filipino roster now canonical), **§26 "Memory XP"** (single XP currency), **§38 Unity structure** (invalid — everything must live under `Assets/`; corrected layout in ggd §56.5), **§40 backend** (deferred; the slice is offline-only).

### *Travel the past. Fight the forgotten. Discover the Bayani.*

The historical backbone will be **Limasawa → Cebu → Mactan**. NHCP's historical marker records the expedition's arrival at Mazaua/Limasawa on March 28, 1521, the Easter Sunday mass on March 31, and departure toward Cebu on April 4. ([Philippine Historical Sites][1])

The artifact system can be grounded in real Philippine archaeological material—earthenware, trade ceramics, ornaments, metal tools, beads, etc.—rather than inventing everything. The National Museum documents these categories and specifically notes evidence of extensive maritime trade involving Chinese, Southeast Asian and other foreign ceramics. ([National Museum of the Philippines][2])

---

# 1. THE COMPLETE PLAYER JOURNEY

```text
                         BAYANI
                           │
                           ▼
                    FUTURE — 2187
                           │
                    National Archive
                           │
                     Archive Failure
                           │
                           ▼
                        TALA AI
                           │
                           ▼
                       BALIKAN
                           │
                     TIME TRAVEL
                           │
                           ▼
                    ┌─────────────┐
                    │   LIMASAWA  │
                    │    1521     │
                    └──────┬──────┘
                           │
                    Explore / Fight
                           │
                 ┌─────────┼─────────┐
                 │         │         │
               Duwende   Anito     Kapre
                 │         │         │
                 └─────────┼─────────┘
                           │
                           ▼
                         CEBU
                           │
                  Hub / Quests / NPC
                           │
                           ▼
                        MACTAN
                           │
                       Lapu-Lapu
                           │
                      Historical Event
                           │
                           ▼
                  Corrupted Memory
                           │
                           ▼
                   FINAL BOSS FIGHT
                           │
                           ▼
                    MEMORY RESTORED
                           │
                           ▼
                    RETURN TO 2187
                           │
                           ▼
                  "WHO IS THE NEXT
                       BAYANI?"
```

That is the **vertical slice**.

---

# 2. WHAT THE PLAYER ACTUALLY DOES

The game loop should be:

```text
EXPLORE
   ↓
DISCOVER
   ↓
FIGHT
   ↓
LOOT
   ↓
LEARN
   ↓
UPGRADE
   ↓
QUEST
   ↓
BOSS
   ↓
MEMORY
   ↓
NEW ABILITY
   ↓
NEW AREA
```

So this is **not an educational walking simulator**.

It is an actual adventure RPG.

---

# 3. FIRST 15 MINUTES

## 00:00–02:00

### CINEMATIC

Future Philippines.

Narrator:

> "Every generation leaves something behind."

> "A name."

> "A story."

> "A memory."

Future Manila appears.

---

## 02:00–05:00

### GAMEPLAY

Kai walks around the archive.

Tutorial:

* WASD
* mouse
* interact
* scan
* inventory

Player discovers disappearing historical records.

---

## 05:00–07:00

### BALIKAN

TALA discovers:

**TEMPORAL ANOMALY — 1521**

Kai enters the machine.

---

## 07:00–10:00

### LIMASAWA

Player wakes on beach.

Free exploration begins.

---

## 10:00–12:00

### FIRST ARTIFACT

Player discovers:

**Earthenware vessel**

TALA scans it.

Codex entry unlocked.

The game teaches:

> Objects can tell stories.

The National Museum's archaeological collection includes earthenware spanning thousands of years and notes that different vessel types and contexts provide information about early societies, trade, cultural identity and spirituality. ([National Museum of the Philippines][3])

---

## 12:00–15:00

### FIRST ENEMY

Something moves in the forest.

TALA:

> "Kai."

Kai:

> "What?"

TALA:

> "Run."

**COMBAT STARTS.**

---

# 4. COMBAT DESIGN

I'd make combat accessible but satisfying.

## Basic Combo

```text
LIGHT
 ↓
LIGHT
 ↓
LIGHT
 ↓
HEAVY
```

Then:

```text
DODGE
 ↓
COUNTER
```

And:

```text
ATTACK
 ↓
SKILL
 ↓
DODGE
 ↓
HEAVY
```

---

# 5. WEAPON TYPES

For the vertical slice:

### Kampilan

Primary melee.

Fast + balanced.

### Spear

Long range.

Good for keeping enemies away.

### Bow

Ranged.

Weak damage but useful against flying creatures.

### Future Blade

Available in the opening/tutorial and later becomes important.

---

# 6. FUTURE BLADE

Don't make Kai suddenly become a medieval warrior.

His first weapon is:

## BALIKAN EDGE

A futuristic energy blade.

But once he reaches 1521, it starts malfunctioning.

TALA:

> "The weapon is losing synchronization."

Kai:

> "Can you fix it?"

TALA:

> "No."

Kai:

> "You really need to stop saying that."

This gives the player a reason to eventually use historical weapons.

---

# 7. WEAPON PROGRESSION

```text
BALIKAN EDGE
      ↓
KAMPILAN
      ↓
UPGRADED KAMPILAN
      ↓
MEMORY-INFUSED WEAPON
```

The player doesn't just get stronger.

Their **relationship with the past changes their equipment**.

---

# 8. ARTIFACT SYSTEM

This is where I think BAYANI can become special.

Don't make artifacts just:

> +10 attack

Instead, each artifact tells a story.

Example:

## EARTHENWARE POT

**Category:** Heritage

**Era:** Precolonial

**Function:** Food storage / domestic use

**Game Effect:** None

**Codex Reward:** +50 Memory XP

---

## TRADE CERAMIC

**Category:** Heritage

**Function:** Trade

**Game Effect:**

Unlocks:

**Trade Routes**

This reveals additional map locations.

This is inspired by actual archaeological evidence of imported ceramics from China, Vietnam, Thailand and elsewhere found in Philippine archaeological contexts. ([National Museum of the Philippines][2])

---

# 9. TREASURE TYPES

## COMMON

* shells
* beads
* pottery fragments
* food
* crafting materials

## UNCOMMON

* ornaments
* tools
* trade ceramics
* weapons

## RARE

* gold ornaments
* special weapons
* unique cultural objects

The National Museum's collections include shell, bone, clay, stone, glass and metal ornaments, as well as gold and bronze objects. ([National Museum of the Philippines][4])

## MYTHIC

These are **game-fiction objects**:

* Anito's Ember
* Duwende's Coin
* Kapre's Token
* Diwata's Veil
* Bathala Fragment

---

# 10. IMPORTANT: BATHALA

I'd actually change how we implement Bathala.

Don't have Kai simply meet:

> "BATHALA — NPC"

That could flatten a complex belief tradition into a fantasy character.

Instead:

### BATHALA FRAGMENT

A mysterious mythological memory.

The player finds fragments of stories.

TALA:

> "The archive cannot determine whether this memory represents a deity, a cultural concept, or a later interpretation."

Kai:

> "So you don't know?"

TALA:

> "History rarely gives us everything."

That is much more interesting.

---

# 11. ANITO SYSTEM

Anito can become a **whole gameplay category**.

Some are:

### Guardian

Protects sacred locations.

### Ancestor

Provides a memory.

### Lost

Needs help.

### Corrupted

Boss enemy.

### Watcher

Reveals hidden locations.

This lets you have spiritual encounters without making every spirit hostile.

---

# 12. DUWENDE SYSTEM

Duwende are your:

## Treasure / Puzzle NPCs

They hide:

* keys
* maps
* coins
* artifact clues
* shortcuts

They can also prank the player.

Example:

Player finds a treasure chest.

Opens it.

Empty.

Duwende laughs.

### QUEST:

**"You Should Have Looked Behind You."**

The actual treasure is behind the player.

---

# 13. KAPRE SYSTEM

Kapre should be a powerful forest guardian.

Not necessarily evil.

The player first thinks:

> Boss.

But eventually discovers:

> Guardian.

This teaches the player that **not every supernatural being is an enemy**.

---

# 14. MYTHOLOGY ENCOUNTER TABLE

| Creature                 | Role       | Gameplay          |
| ------------------------ | ---------- | ----------------- |
| Duwende                  | Trickster  | Puzzle / treasure |
| Kapre                    | Guardian   | Boss / challenge  |
| Anito                    | Spirit     | Quest / blessing  |
| Diwata                   | Guide      | Lore / ability    |
| Tikbalang                | Hunter     | Combat            |
| Aswang-inspired creature | Enemy      | Combat            |
| Bakunawa                 | Major boss | Later chapter     |
| Limot                    | Main enemy | Combat            |

---

# 15. THE LIMOT — MAIN ENEMY

The Limot are the game's original fantasy concept.

They should visually feel Filipino without pretending to be historical creatures.

### Basic Limot

Black smoke + humanoid form.

### Limot Hunter

Four-legged.

### Limot Flyer

Flying.

### Limot Guardian

Heavy.

### Limot Shaman

Magic.

### Limot Warden

Elite.

> **Superseded:** canonical roster uses the Filipino names from ggd §7 — Anino, Limot Lingid (Hunter), Limot Bantay (Guardian), Limot Lumilipad (Flyer), Limot Mangkukulam (Shaman), Limot Tagapuksa (Warden).

---

# 16. THE LIMOT LORE

Eventually Kai discovers:

> The Limot aren't destroying history.

They're feeding on **forgotten memories**.

The more people forget:

**the stronger they become.**

That's the main story mystery.

---

# 17. CEBU HUB

This is where the game opens up.

Player gets:

### Shop

Weapons.

### Blacksmith

Upgrades.

### Herbalist

Healing.

### Trader

Artifacts.

### Story NPCs

Historical context.

### Duwende

Secret treasure quests.

### Shrine

Memory upgrades.

---

# 18. CRAFTING

Keep it simple.

### Materials

* wood
* stone
* fiber
* metal
* herbs
* shells
* rare materials

Craft:

* healing items
* arrows
* weapon upgrades
* artifact holders

---

# 19. PLAYER LEVEL

Start:

**Level 1**

Maximum for vertical slice:

**Level 10**

XP comes from:

* combat
* quests
* exploration
* artifacts
* codex
* bosses

---

# 20. LEVEL-UP SCREEN

```text
              LEVEL UP!

                 KAI

             LEVEL 7 → 8

        + HP
        + STAMINA
        + DIWA

        SKILL POINT +1

       [OPEN SKILL TREE]
```

---

# 21. SKILL TREE

Three branches.

### MANDIRIGMA

Combat.

```text
Basic Attack
     │
Heavy Attack
     │
Counter
     │
Perfect Dodge
     │
Combo Master
```

### MANLALAKBAY

Adventure.

```text
Climb
 │
Swim
 │
Track
 │
Treasure Sense
 │
Spirit Sight
```

### TAGAPAGMANA

Memory.

```text
Diwa Recovery
 │
Memory Shield
 │
Artifact Resonance
 │
Ancestor's Call
 │
Memory Burst
```

---

# 22. UI — FINAL STRUCTURE

The game should have these primary screens:

```text
MAIN MENU
│
├── CONTINUE
├── NEW GAME
├── LOAD GAME
├── CODEX
├── SETTINGS
└── EXIT

IN GAME
│
├── HUD
├── MAP
├── QUESTS
├── INVENTORY
├── EQUIPMENT
├── SKILLS
├── BAYANI MEMORIES
├── CODEX
└── SETTINGS
```

---

# 23. HUD

### PC

```text
┌───────────────────────────────────────────┐
│ KAI                                       │
│ HP     █████████████                      │
│ STAM   ██████████░░                       │
│ DIWA   ███████░░░░                        │
│                                           │
│                              MINI MAP     │
│                              ┌────────┐   │
│                              │  ◉  ▲  │   │
│                              └────────┘   │
│                                           │
│                    ENEMY                  │
│                ████████████               │
│                                           │
│                                           │
│                               ATTACK      │
│                           SKILL   DODGE   │
└───────────────────────────────────────────┘
```

---

# 24. MOBILE HUD

Mobile gets a completely different layout rather than shrinking the PC HUD.

```text
┌─────────────────────────────────────┐
│ HP █████████                        │
│ DIWA ███████                        │
│                         MAP        │
│                      ┌───────┐     │
│                      │   ◉   │     │
│                      └───────┘     │
│                                     │
│             GAME                    │
│                                     │
│                                     │
│  JOYSTICK             ○ ATTACK      │
│                      ○ SKILL        │
│                 ○ DODGE   ○ JUMP   │
└─────────────────────────────────────┘
```

---

# 25. MAP UI

The map should look like a **living historical map**.

Instead of Google Maps styling:

```text
      MACTAN

       🌴🌴🌴
    ┌───────────┐
    │   VILLAGE │
    │     ●     │
    │           │
🌊  │       X   │
    │           │
    │  CAVE     │
    └───────────┘
       BEACH
```

Undiscovered areas are covered by:

### MEMORY FOG

When discovered:

**MEMORY RESTORED**

---

# 26. QUEST UI

```text
━━━━━━━━━━━━━━━━━━━━━━━━━━
       THE LITTLE TREASURE
━━━━━━━━━━━━━━━━━━━━━━━━━━

A strange creature claims
something was stolen from
its hiding place.

OBJECTIVES

□ Enter the forest
□ Follow the tiny footprints
□ Find the hidden cave
□ Recover the coin

REWARD

XP +250
DUWENDE'S COIN
MEMORY +1
━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

# 27. DIALOGUE UI

Characters should remain visible.

```text
         LAPU-LAPU

"Why have you come?"

Kai:
"I'm looking for something."

Lapu-Lapu:
"And what is it?"

Kai:
"A memory."

             [CONTINUE]
```

Choices should be used sparingly.

Don't make this a dialogue-heavy RPG.

---

# 28. ARTIFACT UI

When discovering something:

```text
╔══════════════════════════════╗
║       MEMORY DISCOVERED      ║
║                              ║
║        [3D OBJECT]           ║
║                              ║
║       EARTHENWARE            ║
║                              ║
║       HERITAGE OBJECT        ║
║                              ║
║   HISTORICAL INSPIRATION     ║
║                              ║
║       [VIEW CODEX]           ║
╚══════════════════════════════╝
```

---

# 29. CODEX — VERY IMPORTANT

This should become one of the game's coolest screens.

```text
╔══════════════════════════════════╗
║ BAYANI CODEX                     ║
╠══════════════════════════════════╣
║ HISTORY                          ║
║ PEOPLE                           ║
║ PLACES                           ║
║ HERITAGE                         ║
║ MYTHOLOGY                        ║
║ CREATURES                        ║
║ ARTIFACTS                        ║
╚══════════════════════════════════╝
```

Each entry has:

### STATUS

**DOCUMENTED**

**ARCHAEOLOGICAL**

**FOLKLORE**

**GAME FICTION**

That distinction is important for a history-inspired game.

---

# 30. NARRATION DESIGN

I recommend **three narrative layers**.

### Layer 1 — Cinematic Narrator

Major historical events.

### Layer 2 — TALA

Gameplay information.

### Layer 3 — NPCs

Human stories.

This prevents the narrator from explaining everything.

---

# 31. NARRATOR STYLE

The narrator shouldn't sound like a teacher.

More like:

**National Geographic + cinematic adventure.**

Example:

> "Long before the arrival of foreign ships, these islands were already connected by the sea."

Then let the player discover the rest.

---

# 32. TALA STYLE

TALA handles information.

Example:

> "Object identified."

> "Possible trade ceramic."

> "Origin: Southeast Asian maritime trade network."

Then:

> "Would you like the historical explanation?"

Player can skip it.

---

# 33. NPC NARRATION

NPCs tell personal stories.

For example:

A fisherman:

> "The sea gives us food."

> "But the sea also takes."

That tells the player about life without dumping information.

---

# 34. HISTORICAL EVENT NARRATION

When approaching major events, use short cinematic narration.

For Limasawa:

> "The ships had crossed an ocean."

> "But the sea did not bring them to an empty land."

> "They had entered a world already connected by people, trade and tradition."

This is consistent with archaeological evidence of long-distance maritime trade and foreign ceramics in Philippine contexts. ([National Museum of the Philippines][2])

---

# 35. MACTAN NARRATION

Before the final battle:

Screen becomes quiet.

Ocean sounds.

Wind.

Then:

> "History remembers the battle."

> "But before the battle..."

> "There were people."

> "Families."

> "Homes."

> "Leaders."

> "Fears."

> "Choices."

Pause.

> "And on this island..."

> "those choices would meet."

Then:

# MACTAN

## 27 APRIL 1521

The historical date is documented by the NHCP. ([Philippine Historical Sites][1])

---

# 36. FINAL BOSS NARRATION

During the Memory Devourer:

> "A memory forgotten becomes a story."

> "A story forgotten becomes a shadow."

> "And a shadow..."

> "can become a monster."

---

# 37. ENDING NARRATION

After the boss:

> "Kai had traveled through time looking for a hero."

> "Instead, he found people."

> "People who lived."

> "People who struggled."

> "People who chose."

> "People who remembered."

Then:

> "Perhaps that is what a bayani really is."

Fade.

# BAYANI

---

# 38. UNITY PROJECT STRUCTURE

I would actually organize the project like this:

```text
BAYANI/
│
├── Assets/
│
├── Art/
│   ├── Characters/
│   ├── Environment/
│   ├── Props/
│   ├── Weapons/
│   ├── UI/
│   └── VFX/
│
├── Audio/
│   ├── Music/
│   ├── SFX/
│   ├── Voice/
│   └── Ambient/
│
├── Scenes/
│   ├── 00_Future/
│   ├── 01_Balikan/
│   ├── 02_Limasawa/
│   ├── 03_Cebu/
│   ├── 04_Mactan/
│   └── 05_MemoryVoid/
│
├── Scripts/
│   ├── Core/
│   ├── Player/
│   ├── Combat/
│   ├── Enemy/
│   ├── NPC/
│   ├── Quest/
│   ├── Dialogue/
│   ├── Artifact/
│   ├── Inventory/
│   ├── Skills/
│   ├── Codex/
│   ├── Save/
│   └── API/
│
├── ScriptableObjects/
│   ├── Characters/
│   ├── Weapons/
│   ├── Artifacts/
│   ├── Quests/
│   ├── Enemies/
│   └── Dialogue/
│
└── UI/
    ├── MainMenu/
    ├── HUD/
    ├── Inventory/
    ├── Map/
    ├── Codex/
    ├── Dialogue/
    └── Mobile/
```

---

# 39. SCRIPTABLE OBJECT ARCHITECTURE

This is particularly useful for your game.

For example:

```text
WeaponData
 ├── Name
 ├── Damage
 ├── AttackSpeed
 ├── Durability
 ├── Icon
 ├── Model
 ├── Description
 └── Era
```

Then:

```text
Kampilan.asset
Spear.asset
Bow.asset
BalikanEdge.asset
```

Same concept for:

```text
EnemyData
ArtifactData
QuestData
DialogueData
CharacterData
```

This makes the game much easier to expand later.

---

# 40. BACKEND

Your existing Laravel knowledge fits this very well.

```text
UNITY
   │
 HTTPS
   │
   ▼
LARAVEL API
   │
 ┌─┼───────────────┐
 │ │               │
MySQL Redis       R2/S3
 │
 ├── Players
 ├── Saves
 ├── Quests
 ├── Artifacts
 ├── Codex
 ├── Memories
 └── Achievements
```

However, **the core game should work offline**.

The server should not be required for basic gameplay.

---

# 41. CROSS-PLATFORM STRATEGY

Unity is appropriate because the game is intended for multiple platforms.

I'd develop in this order:

### Phase 1

**Windows**

This is the easiest environment for rapid development and debugging.

### Phase 2

**Android**

This forces mobile optimization early.

### Phase 3

**iOS**

### Phase 4

**macOS**

### Phase 5

**Web**

The Web version should probably be treated as a lighter build rather than assuming the full PC-quality game will run identically in a browser.

---

# 42. DEVELOPMENT PHASES

## PHASE 1 — Prototype

**4–8 weeks**

Build only:

* Kai
* movement
* camera
* combat
* one enemy
* one weapon
* one small environment
* basic UI

No story yet.

Goal:

### "Is it fun to control Kai?"

---

# 43. PHASE 2 — VERTICAL SLICE

**8–16 weeks**

Add:

* Future
* Balikan
* Limasawa
* artifacts
* Duwende
* Anito
* Kapre
* Cebu
* Mactan
* Lapu-Lapu encounter
* quests
* boss
* narration
* codex
* save system

Goal:

### "Does BAYANI feel like BAYANI?"

---

# 44. PHASE 3 — POLISH

Add:

* better animation
* voice acting
* music
* VFX
* cinematic transitions
* optimization
* controller
* mobile UI
* accessibility
* localization

Goal:

### "Can we show this to other people?"

---

# 45. PHASE 4 — FULL GAME

Only after the vertical slice works.

Then expand:

```text
MACTAN
   ↓
REVOLUTION
   ↓
RIZAL
   ↓
PHILIPPINE-AMERICAN WAR
   ↓
WWII
   ↓
MODERN PHILIPPINES
   ↓
FUTURE
```

Each era becomes an adventure chapter.

---

# 46. THE BIGGER GAME

Eventually the player's Memory Map becomes:

```text
                       2187
                        │
                        │
                    BALIKAN
                        │
       ┌────────────────┼────────────────┐
       │                │                │
     1521             1896             1940s
       │                │                │
    Mactan          Revolution          WWII
       │                │                │
       └────────────────┼────────────────┘
                        │
                     FUTURE
```

And eventually:

# BAYANI

becomes an entire **Philippine historical action-adventure universe**.

---

## 47. THE ONE THING I WOULD PROTOTYPE FIRST

Before making all the beautiful maps, artifacts and mythology, make this **5-minute combat prototype**:

```text
Kai
 ↓
Walk through jungle
 ↓
Duwende appears
 ↓
Duwende disappears
 ↓
Limot attacks
 ↓
Player fights
 ↓
Second Limot appears
 ↓
Player dodges
 ↓
Player uses skill
 ↓
Enemy dies
 ↓
Artifact drops
 ↓
Kai scans artifact
 ↓
TALA explains it
 ↓
Hidden path opens
 ↓
Kapre silhouette appears
 ↓
CUT TO BLACK
```

If **that 5-minute sequence feels good**, then we have the core of BAYANI.

---

## Source References

[1]: http://nhcphistoricsites.blogspot.com/ "NHCP National Registry of Historic Sites and Structures"
[2]: https://www.nationalmuseum.gov.ph/our-collections/archaeology/ "National Museum of the Philippines — Archaeological Collections"
[3]: https://www.nationalmuseum.gov.ph/our-collections/archaeology/earthenware/ "National Museum of the Philippines — Earthenware"
[4]: https://www.nationalmuseum.gov.ph/our-collections/archaeology/ "National Museum of the Philippines — Collections (ornaments, gold and bronze)"