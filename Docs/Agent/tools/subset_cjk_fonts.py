"""Cut the Japanese and Chinese fonts down to the characters the localization uses.

The TMP font assets are dynamic, so the build ships the whole source font file. The full fonts
live next to the cut ones in a `Full~` folder, which Unity does not import. Each cut font keeps
every character of its language column in the CSVs, printable ASCII and the CJK punctuation.

Run it after any change to the Japanese or Chinese texts; `check` only reports what is missing.
Needs fontTools (pip install fonttools).

usage: subset_cjk_fonts.py          # rewrite both fonts
       subset_cjk_fonts.py check    # exit 1 if a used character is missing from a cut font
"""
import csv
import glob
import os
import re
import sys

from fontTools import subset
from fontTools.ttLib import TTFont

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..', '..'))
LOCALIZATION = os.path.join(ROOT, 'Assets', 'SimpleLocalization', 'Resources', 'Localization')
FONTS = os.path.join(ROOT, 'Assets', '_DiceBattle', 'Fonts', 'Sources')
TARGETS = {
    'Japanese': os.path.join(FONTS, 'Yuji Syuku', '{}YujiSyuku-Regular.ttf'),
    'Chinese': os.path.join(FONTS, 'Ma Shan Zheng', '{}MaShanZheng-Regular.ttf'),
}
MARKUP = re.compile(r'\\[nrt]|\{\d+\}')
EXTRA = ''.join(chr(code) for code in range(0x20, 0x7f)) + '、。，．・：；？！ー〜…‥「」『』（）［］【】〈〉《》“”‘’•　'


def used_characters(language):
    characters = set(EXTRA)
    for path in glob.glob(os.path.join(LOCALIZATION, '*.csv')):
        with open(path, encoding='utf-8-sig', newline='') as file:
            rows = list(csv.reader(file))
        column = rows[0].index(language)
        for row in rows[1:]:
            if len(row) > column:
                characters.update(MARKUP.sub('', row[column]))
    return {character for character in characters if character.isprintable()}


def main():
    check_only = sys.argv[1:] == ['check']
    missing_any = False

    for language, path in TARGETS.items():
        full = path.format('Full~' + os.sep)
        cut = path.format('')
        characters = used_characters(language)
        available = TTFont(full).getBestCmap()
        absent = sorted(c for c in characters if ord(c) not in available and c not in EXTRA)

        if absent:
            print(f'{language}: the full font itself lacks {"".join(absent)}')

        if check_only:
            in_cut = TTFont(cut).getBestCmap()
            missing = sorted(c for c in characters if ord(c) in available and ord(c) not in in_cut)
            missing_any |= bool(missing)
            print(f'{language}: ' + (f'missing {"".join(missing)}' if missing else 'ok'))
            continue

        options = subset.Options()
        options.layout_features = ['*']
        options.name_IDs = ['*']
        options.notdef_outline = True
        font = subset.load_font(full, options)
        subsetter = subset.Subsetter(options)
        subsetter.populate(unicodes=[ord(c) for c in characters if ord(c) in available])
        subsetter.subset(font)
        subset.save_font(font, cut, options)
        print(f'{language}: {len(characters)} characters, '
              f'{os.path.getsize(full) // 1024} KB -> {os.path.getsize(cut) // 1024} KB')

    sys.exit(1 if missing_any else 0)


if __name__ == '__main__':
    main()
