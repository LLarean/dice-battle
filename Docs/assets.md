# Third-party assets

Everything under `Assets/` that was not made for this project, with its source, licence and whether it has to appear in the in-game credits.

**Credit** column:

- **required** — the licence demands attribution; must be in the credits.
- **requested** — the author asks for it but does not demand it; put it in the credits anyway.
- **optional** — no obligation (CC0 and similar).
- **notice** — a code licence (MIT / OFL): keep the licence text with the project; a line in the credits is good manners.
- **?** — source or licence not confirmed yet, see [Open questions](#open-questions).

**Build** column: **yes** — referenced by a scene, prefab or data asset; **no** — lies in the project but nothing references it (no credit needed until it is used; candidates for removal).

Paths are relative to `Assets/_DiceBattle` unless they start with `Assets/`.

## Art

| What it is in the game | Asset | Author | Path | Licence | Credit | Build |
|---|---|---|---|---|---|---|
| Battle backgrounds (10 images) | [Backgrounds: Dungeons, Ruins, & Caves](https://lornn.itch.io/backgrounds-dungeons-ruins-caves) | Lornn | `Art/Sources/Backgrounds` | Author's terms: free for personal and commercial projects "provided your content isn't racist or hateful". Made in Midjourney and painted over | requested | yes |
| Enemy sprites | [JRPG Monster Assets](https://jmungall.itch.io/jrpg-monster-assets) | James Mungall | `Art/Sources/JRPG Monster Asset 1.png` (+ `(Source)` copy, unused) | Author's terms: free for personal and commercial projects, royalty-free, not CC0; attribution requested | requested | yes |
| Window frames, buttons, bars, name plates | [GUI Parts](https://assetstore.unity.com/packages/package/159068) | PONETI | `Assets/GUI_Parts` | Unity Asset Store EULA (free asset) | optional | yes (13 of 44 sprites) |
| Dice faces: sword, shield, empty; lock, crossed hand, skull | [Board Game Icons](https://kenney.nl/assets/board-game-icons) | Kenney | `Art/Sources/Dices` | CC0 | optional | yes (skull, lock, hand: no) |
| Dice faces: extra, golden, health, joker, reliable, reroll | derived from the Kenney dice above | ? | `Art/Sources/Dices` | ? | ? | yes |
| UI icons (arrows, hourglass, book, card, d4, heart, shield, sword…) | [Board Game Icons](https://kenney.nl/assets/board-game-icons) 1.1 | Kenney | `Art/Sources/kenney_board-game-icons` | CC0 (`License.txt` in the folder) | optional | yes (14 of 510 icons) |
| Volume slider | probably [UI Pack](https://kenney.nl/assets/ui-pack) | Kenney | `Art/Sources/slide_horizontal_*.png`, `button_square_depth_flat.png` | CC0 if confirmed | ? | sliders yes, button no |
| Angry emote | probably [Emotes Pack](https://kenney.nl/assets/emotes-pack) | Kenney | `Art/Sources/Emotes` | CC0 if confirmed | ? | no |
| Tavern background | ? | ? | `Art/Sources/Tavern.jpeg` | ? | ? | yes |
| Menu background (village by the lake) | ? | ? | `Art/Sources/village-island-1.png` | ? | ? | yes |
| Studio logo (Manul Wizard) | own | — | `Art/Logo` | — | — | yes (`05.png`) |
| Hero sprites | [Medieval Warrior Pack 3](https://luizmelo.itch.io/medieval-warrior-pack-3) | LuizMelo | `Art/Sources/Medieval Warrior Pack 3` | CC0 (`License.txt` in the folder) | optional | no |
| Wooden pixel UI sheet | ? | ? | `Art/Sources/freeversion.png` | ? | ? | no |

## Music

| What it is in the game | Asset | Author | Path | Licence | Credit | Build |
|---|---|---|---|---|---|---|
| Tavern music: "Mosslight Market", "Willow's Waltz" | [Forest Folk – Cozy Fantasy Loop Pack](https://promptfm.itch.io/forest-folk-cozy-fantasy-loop-pack) | Prompt.fm | `Audio/Music/Forest_Folk_Pack` | Author's terms (`license.txt` in the folder): royalty-free, **credit required ("Prompt.fm")**, no re-upload as a pack. AI-generated | **required** | yes |
| "Daggers in the Dark" | [Dragon Tales 1](https://ivanduch.com/albums/dragon-tales-1/) | Ivan Duch | `Audio/Music/Daggers in the Dark.mp3` | Author's terms: free tracks may be used **if the composer is credited**; no redistribution as standalone audio | **required** | yes |

## Sounds

| What it is in the game | Asset | Author | Path | Licence | Credit | Build |
|---|---|---|---|---|---|---|
| Dice grab / shake / throw | [Casino Audio](https://kenney.nl/assets/casino-audio) 1.1 | Kenney | `Audio/Sounds/kenney_casino-audio` | CC0 (`License.txt` in the folder) | optional | yes |
| UI click | probably [Interface Sounds](https://kenney.nl/assets/interface-sounds) | Kenney | `Audio/Sounds/switch_002.ogg` | CC0 if confirmed | ? | yes |
| Gold, party created / disbanded, war declared | [16 Free Fantasy SFX – Guild / Equipment / Extras](https://juandefuego.itch.io/16-free-fantasy-sfx-party-equipment-extras) | juandefuego | `Audio/free 16 sfxs - juandefuego` | ? (no licence file in the pack) | ? | yes (4 of 16) |
| Sword slice | ? (possibly the 400 Sounds Pack) | ? | `Audio/Sounds/sword_slice.wav` | ? | ? | yes |
| Hits, crunches, splats | [400 Sounds Pack](https://ci.itch.io/400-sounds-pack), "Combat & Gore" | Chequered Ink | `Audio/Sounds/Combat and Gore` | Author's terms: any use incl. commercial, with or without credit; unaltered files may not be resold or redistributed | optional | no |
| Locked door rattle | [LockedDoor-01](https://freesound.org/people/wavewire/sounds/833020/) | wavewire (Freesound) | `Audio/Sounds/833020__wavewire__lockeddoor-01.wav` | **CC BY 4.0** | **required** once used | no |

## Fonts

| What it is in the game | Asset | Author | Path | Licence | Credit | Build |
|---|---|---|---|---|---|---|
| Main font | Stieglitz SP Bold | Sasha Pavljenko ([pavljenko.ru](https://pavljenko.ru)) | `Fonts/Sources/stieglitz SP` | Author's free licence (`Short License.pdf` in the folder): free incl. commercial use, the font file may not be resold; a signed licence certificate is sold separately (500 RUB / 5 USD) | requested | yes |
| Japanese fallback | [Yuji Syuku](https://github.com/Kinutafontfactory/Yuji) | The Yuji Project Authors | `Fonts/Sources/Yuji Syuku` | SIL OFL 1.1 (`OFL.txt` in the folder) | notice | yes |
| Chinese fallback | [Ma Shan Zheng](https://github.com/googlefonts/mashanzheng) | The Ma Shan Zheng Project Authors | `Fonts/Sources/Ma Shan Zheng` | SIL OFL 1.1 (`OFL.txt` in the folder) | notice | yes |
| Dice hints, credits, generic fallback | Liberation Sans (TMP Essentials) | The Liberation Fonts Project | `Assets/TextMesh Pro/Fonts` | SIL OFL 1.1 (`LiberationSans - OFL.txt` in the folder) | notice | yes |
| TMP default emoji sprite sheet | EmojiOne (TMP Essentials) | EmojiOne | `Assets/TextMesh Pro/Sprites` | CC BY 4.0 (`EmojiOne Attribution.txt` in the folder) | required only if emoji are shown | in `Resources`, not shown |
| — | [Kenney Fonts](https://kenney.nl/assets/kenney-fonts), Kenney Future | Kenney | `Fonts/Sources/Kenney Future.ttf` | CC0 | optional | no |

## Code

| What it does | Asset | Author | Path | Licence | Credit | Build |
|---|---|---|---|---|---|---|
| Tweens | [LeanTween](https://github.com/dentedpixel/LeanTween) | Russell Savage (Dented Pixel); easing equations by Robert Penner | `Assets/LeanTween` | MIT + BSD for the easing equations (`License.txt` in the folder) | notice | yes |
| Localization (CSV) | [Simple Localization](https://github.com/hippogamesunity/SimpleLocalization) | Hippo Games | `Assets/SimpleLocalization` | ? (no licence file in the folder; check the repository / Asset Store page) | ? | yes |
| Inspector attributes | [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes) | Denis Rizov | UPM package | MIT | notice | editor-facing, compiled in |
| Signals | [signal-system](https://github.com/llarean/signal-system) | own | UPM package | — | — | yes |
| Engine, TextMesh Pro, URP, Input System and other `com.unity.*` packages | Unity | Unity Technologies | UPM packages | Unity Companion Licence | — | yes |

## What the credits must contain today

Required:

- Music: "Mosslight Market", "Willow's Waltz" — Prompt.fm
- Music: "Daggers in the Dark" — Ivan Duch

Requested by the authors:

- Battle backgrounds — Lornn
- Monsters — James Mungall
- Font Stieglitz SP — Sasha Pavljenko

Good manners (no obligation): Kenney (icons, dice, dice sounds), PONETI (GUI Parts), juandefuego (sounds), fonts Yuji Syuku, Ma Shan Zheng, Liberation Sans, LeanTween, Simple Localization, NaughtyAttributes.

## Open questions

- Source and licence of `Tavern.jpeg` and `village-island-1.png` (both are in the build).
- Source of `sword_slice.wav`, `freeversion.png`, and who drew the six extra dice faces.
- Confirm the three "probably Kenney" rows (UI Pack, Emotes Pack, Interface Sounds).
- Licence of the juandefuego pack and of Simple Localization — neither ships a licence file.
- Unused assets (Build = no) can be deleted to shrink the repository and this list.
