"""Exact win-probability solver for a DiceBattle campaign battle.

Rules mirrored from GameLogic / DiceResult / DiceRuleset / UnitPanel:
- faces Empty/Attack/Defense/Heal, uniform; Reliable never rolls Empty
- one roll plus (RerollsPerTurn + AdditionalTry) rerolls of any subset
- heal first (capped), then the hero hits for max(1, dmg - enemyArmor), 5+ Attack faces kill
- a surviving enemy hits for max(0, enemyDmg - armor); LastStand saves once at 1 HP
- the hero starts every battle at full health

CLASSES and ENEMIES are a copy of GameConfig.asset: change them here when the config changes.
ENEMY_DMG=2,4,8,... in the environment overrides the damage column for a what-if run.

usage: balance.py verify                         # solver against Monte Carlo
       balance.py base                           # default dice only, every class and enemy
       balance.py campaign <strong> <casual>     # campaigns with loot, runs per class (20 150 takes minutes)

"strong" picks the best card, deck and rerolls (upper bound); "casual" picks a random card,
equips into free slots only and rerolls only empty faces (closer to a lower bound).
"""
import functools
import random
import sys

import numpy as np

EMPTY, ATK, DEF, HEAL = 0, 1, 2, 3
TYPES = ['Default', 'Sharp', 'Sturdy', 'Healing', 'Reliable', 'Thorns', 'Vampiric', 'Golden', 'Joker',
         'LastStand', 'AdditionalTry', 'AdditionalDice']
WEIGHT = {'Sharp': 10, 'Sturdy': 10, 'Healing': 10, 'Reliable': 10, 'Thorns': 6, 'Vampiric': 6, 'Joker': 6,
          'Golden': 3, 'AdditionalTry': 3, 'LastStand': 3, 'AdditionalDice': 2}
EFFECT_FACE = {'Sharp': ATK, 'Sturdy': DEF, 'Healing': HEAL}
CLASSES = {'Knight': (16, 1, 2), 'Archer': (18, 2, 1), 'Mage': (15, 3, 0)}  # health, damage, armor
ENEMIES = [(5, 2, 0), (8, 4, 0), (14, 8, 0), (14, 8, 0), (12, 7, 2), (15, 9, 0), (16, 9, 0), (15, 8, 1),
           (16, 8, 2), (23, 8, 1)]  # health, damage, armor
import os
if os.environ.get('ENEMY_DMG'):
    ENEMIES = [(h, int(d), a) for (h, _, a), d in zip(ENEMIES, os.environ['ENEMY_DMG'].split(','))]
BASE_DICE = 5
MAX_EXTRA = 2
REROLLS_PER_TURN = 1
CRIT = 5


def canon(equipped):
    return tuple(sorted(equipped, key=TYPES.index))


def board(equipped):
    extra = min(MAX_EXTRA, sum(1 for t in equipped if t == 'AdditionalDice'))
    count = BASE_DICE + extra
    deck = list(equipped) + ['Default'] * count
    return deck[:count]


@functools.lru_cache(maxsize=None)
def tables(equipped):
    deck = board(equipped)
    n = len(deck)
    f = np.indices((4,) * n)
    resolved = []
    for i, t in enumerate(deck):
        if t != 'Joker':
            resolved.append(f[i])
            continue
        best_key = np.full(f[i].shape, -1)
        best_face = f[i].copy()
        for v in (ATK, DEF, HEAL):
            cnt = np.zeros(f[i].shape, dtype=int)
            pos = np.full(f[i].shape, 99)
            for j in range(n - 1, -1, -1):
                if j == i:
                    continue
                hit = f[j] == v
                cnt += hit
                pos = np.where(hit, j, pos)
            key = np.where(cnt > 0, cnt * 100 - pos, -1)
            better = key > best_key
            best_face = np.where(better, v, best_face)
            best_key = np.where(better, key, best_key)
        resolved.append(best_face)
    dmg = np.zeros(f[0].shape, dtype=int)
    arm = np.zeros_like(dmg)
    heal = np.zeros_like(dmg)
    attacks = np.zeros_like(dmg)
    for i, t in enumerate(deck):
        face = resolved[i]
        value = np.where(face == EMPTY, 0, np.where((t == 'Golden') | (EFFECT_FACE.get(t, -1) == face), 2, 1))
        dmg += np.where(face == ATK, value, 0) + np.where((face == DEF) & (t == 'Thorns'), 1, 0)
        arm += np.where(face == DEF, value, 0)
        heal += np.where(face == HEAL, value, 0) + np.where((face == ATK) & (t == 'Vampiric'), 1, 0)
        attacks += face == ATK
    weights = [(np.array([0, 1, 1, 1]) / 3 if t == 'Reliable' else np.full(4, 0.25)).astype(np.float32) for t in deck]
    empties = np.zeros_like(dmg)
    for i in range(n):
        empties |= (f[i] == EMPTY) << i
    rerolls = REROLLS_PER_TURN + deck.count('AdditionalTry')
    return n, dmg, arm, heal, attacks >= CRIT, weights, empties, rerolls, 'LastStand' in deck


def expectations(w, n, weights, lead):
    """E[S] = value after rerolling the dice in mask S, kept broadcastable against w."""
    exp = [None] * (1 << n)
    exp[0] = w
    for s in range(1, 1 << n):
        i = (s & -s).bit_length() - 1
        shape = [1] * (lead + n)
        shape[lead + i] = 4
        exp[s] = (exp[s & (s - 1)] * weights[i].reshape(shape)).sum(axis=lead + i, keepdims=True)
    return exp


@functools.lru_cache(maxsize=None)
def solve(cls, enemy, equipped, policy='opt', dmg_delta=0, hp_mult=1.0):
    max_hp, start_dmg, start_arm = CLASSES[cls]
    e_hp, e_dmg, e_arm = ENEMIES[enemy]
    e_hp = int(round(e_hp * hp_mult))
    e_dmg += dmg_delta
    n, dmg, arm, heal, crit, weights, empties, rerolls, has_ls = tables(equipped)
    stages = 2 if has_ls else 1
    value = np.zeros((stages, max_hp + 1, e_hp + 1), dtype=np.float32)
    hp = np.arange(1, max_hp + 1).reshape((max_hp,) + (1,) * n)
    hit = np.maximum(1, start_dmg + dmg - e_arm)
    taken = np.maximum(0, e_dmg - (start_arm + arm))
    after_heal = np.minimum(max_hp, hp + heal)
    left = after_heal - taken
    dead = left <= 0
    left_idx = np.clip(left, 1, max_hp)
    for e in range(1, e_hp + 1):
        e_left = np.where(crit, 0, e - hit)
        win = e_left <= 0
        e_idx = np.broadcast_to(np.clip(e_left, 1, e_hp), left.shape)
        w = np.empty((stages,) + left.shape, dtype=np.float32)
        for stage in range(stages):
            alive = value[stage][left_idx, e_idx]
            if has_ls and stage == 0:
                lost = value[1][1, e_idx]
            else:
                lost = np.float32(0)
            w[stage] = np.where(win, np.float32(1), np.where(dead, lost, alive))
        for _ in range(rerolls):
            exp = expectations(w, n, weights, 2)
            if policy == 'opt':
                best = w
                for s in range(1, 1 << n):
                    best = np.maximum(best, exp[s])
                w = best
            else:
                out = np.zeros_like(w)
                for s in range(1 << n):
                    out += np.where(empties == s, exp[s], np.float32(0))
                w = out
        value[:, 1:, e] = expectations(w, n, weights, 2)[(1 << n) - 1].reshape(stages, max_hp)
    return float(value[0, max_hp, e_hp])


def simulate(cls, enemy, equipped, trials, rng):
    """Monte Carlo of the 'reroll every empty face' policy, to cross-check solve()."""
    max_hp, start_dmg, start_arm = CLASSES[cls]
    e_hp0, e_dmg, e_arm = ENEMIES[enemy]
    n, dmg, arm, heal, crit, weights, empties, rerolls, has_ls = tables(equipped)
    deck = board(equipped)
    wins = 0
    for _ in range(trials):
        hp, e_hp, ls_used = max_hp, e_hp0, False
        while True:
            faces = [rng.randint(1 if t == 'Reliable' else 0, 3) for t in deck]
            for _ in range(rerolls):
                faces = [rng.randint(1 if t == 'Reliable' else 0, 3) if v == EMPTY else v for v, t in zip(faces, deck)]
            idx = tuple(faces)
            hp = min(max_hp, hp + heal[idx])
            e_hp = 0 if crit[idx] else e_hp - max(1, start_dmg + dmg[idx] - e_arm)
            if e_hp <= 0:
                wins += 1
                break
            hp -= max(0, e_dmg - (start_arm + arm[idx]))
            if hp <= 0:
                if has_ls and ls_used == False:
                    ls_used, hp = True, 1
                else:
                    break
    return wins / trials


def offer(rng, owned):
    candidates = [t for t in TYPES[1:] if not (t == 'AdditionalDice' and owned.count(t) >= MAX_EXTRA)]
    result = []
    while len(result) < 3:
        pick = rng.choices(candidates, [WEIGHT[t] for t in candidates])[0]
        result.append(pick)
        candidates.remove(pick)
    return result


def capacity(equipped):
    return BASE_DICE + min(MAX_EXTRA, equipped.count('AdditionalDice'))


def fits(equipped):
    return len(equipped) <= capacity(equipped) and equipped.count('AdditionalDice') <= MAX_EXTRA


def equip_options(equipped, new):
    """Decks reachable by taking `new`: add it, swap it for one equipped die, or leave it in the bag."""
    options = [tuple(equipped), tuple(equipped) + (new,)]
    for old in set(equipped):
        swapped = list(equipped)
        swapped.remove(old)
        options.append(tuple(swapped) + (new,))
    return [deck for deck in options if fits(deck)]


def campaign(job):
    cls, seed, strong, delta = job
    rng = random.Random(seed)
    equipped, owned, rates, trail = (), [], [], []
    for enemy in range(len(ENEMIES)):
        policy = 'opt' if strong else 'empties'
        rates.append(solve(cls, enemy, canon(equipped), policy, delta))
        trail.append(canon(equipped))
        if enemy == len(ENEMIES) - 1:
            break
        cards = offer(rng, owned)
        if strong:
            best = None
            for card in cards:
                for deck in equip_options(equipped, card):
                    score = solve(cls, enemy + 1, canon(deck), 'opt', delta)
                    if best is None or score > best[0]:
                        best = (score, card, deck)
            owned.append(best[1])
            equipped = best[2]
        else:
            card = rng.choice(cards)
            owned.append(card)
            if fits(equipped + (card,)):
                equipped += (card,)
    return cls, strong, rates, trail


def row(label, rates):
    print(f'{label:<28}' + ' '.join(f'{r * 100:5.1f}' for r in rates))


def main():
    mode = sys.argv[1]
    if mode == 'verify':
        rng = random.Random(1)
        for cls, enemy, deck in [('Knight', 2, ()), ('Mage', 5, ('Joker', 'Thorns', 'LastStand')),
                                 ('Archer', 9, ('Reliable', 'Golden', 'AdditionalTry', 'AdditionalDice', 'Vampiric'))]:
            exact = solve(cls, enemy, canon(deck), 'empties')
            print(cls, enemy + 1, deck, f'exact {exact:.4f}  monte-carlo {simulate(cls, enemy, canon(deck), 200000, rng):.4f}')
    elif mode == 'base':
        print(' ' * 28 + ' '.join(f'{i + 1:>5}' for i in range(10)))
        row('target', [1, .99, .97, .95, .88, .85, .82, .80, .68, .55])
        for cls in CLASSES:
            for policy in ('opt', 'empties'):
                row(f'{cls} default dice {policy}', [solve(cls, e, (), policy) for e in range(10)])
    elif mode == 'campaign':
        from multiprocessing import Pool
        from collections import Counter
        strong_runs, casual_runs = int(sys.argv[2]), int(sys.argv[3])
        delta = int(sys.argv[4]) if len(sys.argv) > 4 else 0
        jobs = [(cls, seed, True, delta) for seed in range(strong_runs) for cls in CLASSES]
        jobs += [(cls, seed, False, delta) for seed in range(casual_runs) for cls in CLASSES]
        results, decks, trails = {}, {}, {}
        with Pool(10) as pool:
            for cls, strong, rates, trail in pool.imap_unordered(campaign, jobs, chunksize=1):
                results.setdefault((cls, strong), []).append(rates)
                decks.setdefault((cls, strong), Counter()).update(trail[-1])
                trails.setdefault((cls, strong), []).append(trail)
        import pickle
        if len(sys.argv) > 5:
            pickle.dump(trails, open(sys.argv[5], 'wb'))
        print(f'enemy damage {delta:+d}', [d for _, d, _ in ENEMIES])
        print(' ' * 28 + ' '.join(f'{i + 1:>5}' for i in range(10)))
        row('target', [1, .99, .97, .95, .88, .85, .82, .80, .68, .55])
        for cls in CLASSES:
            for strong in (True, False):
                data = np.array(results[(cls, strong)])
                name = 'strong' if strong else 'casual'
                row(f'{cls} {name} mean', data.mean(axis=0))
                row(f'{cls} {name} worst 10%', np.percentile(data, 10, axis=0))
        for key, counter in decks.items():
            runs = len(results[key])
            print(key, ', '.join(f'{t} {c / runs:.2f}' for t, c in counter.most_common()))

if __name__ == '__main__':
    main()
