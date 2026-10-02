#!/usr/bin/env python3
"""本地化覆盖审计:每个注册内容类必须在 zhs+eng 有对应复合键。
卡=错误(title+description);可见 Power=错误(三件套);遗物=错误(title/description)+警告(flavor);药水=错误。"""
import json, re, glob, sys
from pathlib import Path

import modmeta

ROOT = Path(__file__).resolve().parent.parent
MOD_ID = modmeta.mod_id(ROOT)
LOC_PREFIX = modmeta.loc_prefix(ROOT)
SRC_DIR = modmeta.src_dir(ROOT)
CARD_BASE = modmeta.base_class(ROOT, 'card')
POWER_BASE = modmeta.base_class(ROOT, 'power')
RELIC_BASE = modmeta.base_class(ROOT, 'relic')
POTION_BASE = modmeta.base_class(ROOT, 'potion')
ENCHANT_BASE = modmeta.base_class(ROOT, 'support_enchantment')

SNAKE = lambda s: re.sub(r'(?<!^)(?=[A-Z])', '_', s).upper()
errors, warns = [], []

def load(lang, table):
    return json.load(open(f'localization/{lang}/{table}.json', encoding='utf-8'))

def need(cls, table, keys, severity='error', flavor_optional=False):
    for lang in ('zhs', 'eng'):
        d = load(lang, table)
        for k in keys:
            # 表名→类别词:cards→CARD powers→POWER relics→RELIC potions→POTION
            full = f'{LOC_PREFIX}{ {"cards":"CARD","powers":"POWER","relics":"RELIC","potions":"POTION","enchantments":"ENCHANTMENT"}[table] }_{SNAKE(cls)}.{k}'
            if full not in d:
                (errors if severity == 'error' else warns).append(f'[{lang}] {cls} 缺 {full}')

for f in glob.glob(f'{SRC_DIR}/Content/Cards/**/*.cs', recursive=True):
    m = re.search(r'class (\w+)\s*:\s*' + CARD_BASE, open(f).read())
    if m:
        need(m.group(1), 'cards', ['title', 'description'])
for f in glob.glob(f'{SRC_DIR}/Content/Powers/*.cs'):
    src = open(f).read()
    m = re.search(r'class (\w+)\s*:\s*' + POWER_BASE, src)
    if m:
        need(m.group(1), 'powers', ['title', 'description', 'smartDescription'])
for f in glob.glob(f'{SRC_DIR}/Content/Relics/*.cs'):
    m = re.search(r'class (\w+)\s*:\s*' + RELIC_BASE, open(f).read())
    if m:
        need(m.group(1), 'relics', ['title', 'description'])
        need(m.group(1), 'relics', ['flavor'], severity='warn')
for f in glob.glob(f'{SRC_DIR}/Content/Potions/*.cs'):
    m = re.search(r'class (\w+)\s*:\s*' + POTION_BASE, open(f).read())
    if m:
        need(m.group(1), 'potions', ['title', 'description'])
for f in glob.glob(f'{SRC_DIR}/Content/Enchantments/*.cs'):
    m = re.search(r'class (\w+)\s*:\s*' + ENCHANT_BASE, open(f).read())
    if m:
        need(m.group(1), 'enchantments', ['title', 'description', 'extraCardText'])


# 代码侧 loc 键引用校验(2026-10-02 按图索骥事故:LocString 引用的键缺失会原文回显在界面):
# ① 字面量 new LocString("表","键") → 键必须存在于对应表(双语);
# ② 卡类源码出现 ".selectionScreenPromptDiscard" 字面 → 该卡在 cards 表必须有该后缀键。
import glob as _glob
for f in _glob.glob(f'{SRC_DIR}/**/*.cs', recursive=True):
    src = open(f).read()
    for table, key in re.findall(r'new LocString\("(\w+)",\s*"([^"]+)"\)', src):
        if key.endswith('.title') or '+' in key:
            continue  # 事件 Entry.title 之类动态拼接不动;纯字面量键才校验
        for lang in ('zhs', 'eng'):
            if key not in load(lang, table):
                errors.append(f'[{lang}] 代码引用的键缺失: {table}/{key} ({f})')
    m = re.search(r'class (\w+)\s*:\s*' + CARD_BASE, src)
    if m and '.selectionScreenPromptDiscard' in src:
        need(m.group(1), 'cards', ['selectionScreenPromptDiscard'])

# 英文占位桩检测(2026-10-03 审阅 #15:35 张卡 eng description 退化成卡名未被发现):
# eng description 与 eng title 完全相等 → 视为占位错误。zhs 为源语言不适用此查。
_eng_cards = load('eng', 'cards')
for k, v in sorted(_eng_cards.items()):
    if not k.endswith('.description') or not isinstance(v, str):
        continue
    title = _eng_cards.get(k[:-len('.description')] + '.title')
    if isinstance(title, str) and title.strip() and v.strip() == title.strip():
        errors.append(f'[eng] description 是标题占位桩: {k}')

for w in warns:
    print('警告:', w)
for e in errors:
    print('错误:', e)
if errors:
    sys.exit(1)
print(f'本地化覆盖审计通过(警告 {len(warns)} 条)')
