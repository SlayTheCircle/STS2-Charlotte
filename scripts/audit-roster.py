#!/usr/bin/env python3
"""核对公开设计卡表、中文标题与内容类；不验证数值或升级行为。
默认读取 docs/history/design/card-roster.txt（原案档案），ROSTER_DESIGN_FILE 可覆盖。
已确认的原案外新增内容单独声明，缺失或空表应失败。"""
import json, os, re, glob, sys
from pathlib import Path

import modmeta

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
MOD_ID = modmeta.mod_id(Path(ROOT))
CARD_BASE = modmeta.base_class(Path(ROOT), 'card')
DOC = os.environ.get('ROSTER_DESIGN_FILE') or os.path.join(ROOT, 'docs', 'history', 'design', 'card-roster.txt')
if not os.path.isfile(DOC):
    print(f'错误: 找不到设计花名册 {DOC}')
    sys.exit(1)

# 衍生仓按需登记:原案外已确认新增(ADDITIONS)、衍生 token(TOKENS)、明确延后(DEFERRED);模板默认全空。
DEFERRED = set()
TOKENS = set()
ADDITIONS = set()

names = []
for line in open(DOC, encoding='utf-8'):
    m = re.match(r'^(.+?)（(攻击|技能|能力)）', line.strip())
    if m:
        names.append(m.group(1))
design = set(names)
if not design:
    print('错误: 设计花名册没有可解析的卡牌条目')
    sys.exit(1)
if len(names) != len(design):
    dup = {n for n in names if names.count(n) > 1}
    print(f'错误: 设计文档重名 {dup}')
    sys.exit(1)

loc = json.load(open(os.path.join(ROOT, 'localization', 'zhs', 'cards.json'), encoding='utf-8'))
titles = {v for k, v in loc.items() if k.endswith('.title')}
cls_n = len([f for f in glob.glob(os.path.join(ROOT, 'src', MOD_ID, 'Content', 'Cards', '**', '*.cs'), recursive=True)
             if not os.path.basename(f).startswith(CARD_BASE)])

errors = []
if len(titles) != cls_n:
    errors.append(f'类文件 {cls_n} ↔ loc 标题 {len(titles)} 不齐(有类没 loc 或反之)')
ghost = titles - design - TOKENS - ADDITIONS
if ghost:
    errors.append(f'实装了但不在设计表/白名单: {sorted(ghost)}')
pending = design - titles
unexpected = pending - DEFERRED
if unexpected:
    errors.append(f'未实装且不在缓做清单(在途批次落地后应清零): {sorted(unexpected)}')

for e in errors:
    print('错误:', e)
print(f'花名册: 原案 {len(design)} / 原案已实装 {len(titles & design)} / 延后 {len(DEFERRED)} / 新增 {sorted(titles & ADDITIONS)} / token {sorted(TOKENS)}')
if errors:
    sys.exit(1)
print('花名册审计通过')
