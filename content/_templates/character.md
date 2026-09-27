# Template: Character / NPC

> Copy this file to `content/characters/<id>.md` (e.g. `content/characters/kai.md`) and fill it in.
> Fields marked ⚑ are required. `historical_status` enum values are canonical per ggd.md §51 — never invent new ones.
> The `id` in front matter is the stable key used by quests, dialogue and codex files. Never rename an id after other files reference it.

```markdown
---
id: CH_0001                        ⚑ stable unique id (prefix CH_)
name: ""                           ⚑ display name
role: protagonist                  ⚑ protagonist | companion | npc | quest_giver | merchant | historical_figure | spirit | non_human
faction: none                      tribe | Cebu | Mactan | duwende | archive | none
era: 2187                          ⚑ 2187 | 1521 | both
locations: []                      list of LOC_ ids this character appears in
historical_status: fictional       ⚑ historical | archaeological | ethnographic | folklore | fictional (ggd §51)
source_reference: ""               NHCP / NatMus URL or book — required if historical_status ≠ fictional
voice_direction: ""                casting & delivery notes (see blueprints.md §45)
---

## Appearance
Silhouette, build, clothing, distinguishing marks. Art brief — 3–6 bullets.

## Personality
3–5 core traits + how they behave under pressure. (Kai: curious, sarcastic, impatient…)

## Relationships
| With (CH_/NPC_ id) | Type | Notes |
|---|---|---|
| CH_0002 | companion | TALA exists in the Balikan device |

## Story Function
What this character exists to do in the slice. One paragraph.

## Dialogue Hooks
2–3 lines that capture their voice — writers use these as tone anchors.

## Codex Entry
codex_category: PEOPLE    ⚑ one of the 7 canonical tabs (ggd §56.1 #3)
unlock_condition: ""      e.g. "Meet in SC_02_Limasawa"

## Notes / Open Questions
```
