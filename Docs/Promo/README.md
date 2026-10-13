# Promo materials

Everything a store page needs, kept outside `Assets/` so that it never gets into a build. Source files (PSD and the like) go next to the exported ones.

Sizes marked *(unverified)* are from memory — check them on the store's own page before drawing.

| Folder | What goes in | Status |
| --- | --- | --- |
| `Icon/` | Square icon, 1024x1024 master; stores take 512x512 | missing — the app icon in the build is the only one |
| `Cover/` | itch.io cover 630x500; Yandex Games cover 800x470 *(unverified)*; GitHub social preview 1280x640 | missing, needs art |
| `Screenshots/` | Store screenshots at 1080x1920, per language where the store asks for it. The README images live in `Docs/Screenshots` (540x960, English) | missing |
| `Gif/` | A short gameplay GIF or video: one full turn with a reroll and a hit | missing |
| `Texts/` | Short and full descriptions, one file per language, plus the AI-content disclosure for itch.io | missing |

Screenshots and the GIF can be captured from the editor (see [unity-workflow.md](../Agent/unity-workflow.md)); the icon and the covers need art.
