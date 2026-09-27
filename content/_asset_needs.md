# BAYANI — 3D Asset Needs Tracker

> **Purpose:** the single list of what art assets the project needs, gated by milestone.
> **Golden rule (from README + ggd §56.8):** do NOT produce a real asset until the tier above it is proven. The 5-minute graybox prototype needs **zero** finished models — cubes and free Mixamo placeholders are the intended start.
> **Formats:** animated things = `.fbx`; static env = `.fbx` props + Unity Terrain; textures = `.png` (masks `.tga`). Sources (`.blend`/`.psd`) stay OUT of this repo; game-ready binaries go through Git LFS. See README "Repository Layout".

---

## Legend
- **Tier 0 — Prototype:** graybox, prove combat feel. Target: "Is it fun to control Kai?"
- **Tier 1 — Feel pass:** a handful of real assets to replace graybox. Target: "Does BAYANI look/feel like BAYANI?"
- **Tier 2 — Vertical Slice:** full content (see `blueprints.md` §50). Target: "Can we show this to people?"

Status: `[ ] todo` · `[~] in progress` · `[x] done` · `[f] free/placeholder ok`

---

## TIER 0 — Prototype (start here)

| ✓ | Asset | Type | Format | Source | Notes |
|---|---|---|---|---|---|
| `[f]` | Kai | character | capsule primitive | Unity | No model needed — controller/camera/combat test |
| `[f]` | Anino (basic Limot) | enemy | cube/capsule | Unity | One AI target is enough |
| `[f]` | Balikan Edge | weapon | cube | Unity | Attack readability comes from VFX + anim, not shape |
| `[f]` | Jungle / beach | environment | planes + cubes | Unity Terrain/TMP | Graybox cover for line-of-sight |
| `[f]` | Move/attack/dodge anims | animation | `.fbx` | **Mixamo (free)** | Only external asset at this stage; drop-in auto-rigged |

**Exit gate:** combat + artifact-scan loop feels good → green light for Tier 1.

---

## TIER 1 — Feel pass (only after Tier 0 passes)

| ✓ | Asset | Type | Format | Count | Priority |
|---|---|---|---|---|---|
| `[ ]` | **Kai** — rigged + walk/run/attack×3/heavy/dodge/parry/scan | character | `.fbx` + `.png` | 1 | 🔴 highest — player is always on screen |
| `[ ]` | **Kai animation set** (or upgrade Mixamo) | animation | inside `.fbx` | 1 pack | 🔴 combat is sold by animation |
| `[ ]` | **Balikan Edge** (futuristic) | weapon prop, hand-socketed | `.fbx` | 1 | 🟠 |
| `[ ]` | **Anino** — rigged + 2–3 attacks + death | enemy | `.fbx` + `.png` | 1 | 🔴 proves the full enemy pipeline |
| `[ ]` | **Artifact** (earthenware pot) + scan VFX | prop | `.fbx` | 1 | 🟠 the discovery mechanic hero object |
| `[ ]` | **Environment kit** — tropical foliage, ground tiles, a hut, boat, rock, water | static set | `.fbx` props + Terrain + materials | ~10–15 pieces | 🟠 replace graybox jungle/beach |
| `[ ]` | Combat VFX (hit spark, slash trail, dodge flash) | VFX | Unity particles/shaders | — | 🟡 not a model but needed to feel good |

**Style anchor (ggd §49):** stylized cinematic 3D, NOT photorealistic — this choice slashes modeling cost. Lock it before Tier 1 starts.

---

## TIER 2 — Vertical slice (do NOT start until Tier 1 feels good)

Full scope lives in `blueprints.md` §50. Summary of asset load:

| Category | Target count | Notes |
|---|---|---|
| Characters / NPCs | Kai, TALA (form?), Lapu-Lapu, 15–20 fictionalized NPCs | ggd §51 classification per NPC |
| Enemies (rigged) | 6 Limot types + Duwende + Kapre + Anito + mini-boss + Memory Devourer | Filipino roster = ggd §7; names supersede core §15 |
| Locations | 6 scenes (`SC_00`–`SC_05`) | assembled as Prefabs referencing FBX props — never one giant file |
| Weapons | Kampilan, Spear, Bow, Balikan Edge + upgrades | `WeaponData` SO (ggd §56.4 — durability reserved/unused) |
| Artifacts / heritage items | 10–20 collectibles + 5–10 mythic | archaeological basis: National Museum (ggd §56.7) |
| Props / treasure containers | woven/wood/clay vessels (NO generic EU chests, ggd §39) | |

---

## Decisions to lock before Tier 1
- `[ ]` **Art style** confirmed = stylized cinematic (not photoreal)
- `[ ]` **Kai silhouette** concept sketch approved
- `[ ]` **Animation source**: Mixamo vs custom mocap (Mixamo fine through slice?)
- `[ ]` **Git LFS** tracking configured (`.fbx` `.png` `.tga` `.wav` `.ogg`) — must be done BEFORE first binary commit

## Referenced files
- Templates (design/schema layer): [`_templates/character.md`](_templates/character.md) · [`_templates/enemy.md`](_templates/enemy.md) · [`_templates/location.md`](_templates/location.md)
- Canonical specs: [`docs/ggd.md`](../docs/ggd.md) §49 art direction, §56.5 project structure, §56.8 prototype
