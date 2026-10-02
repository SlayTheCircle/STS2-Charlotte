#!/usr/bin/env python3
"""占位符审计:本地化文本里引用的 {Var...} 键必须存在于对应卡类的变量集。
实测事故:E 批换 ExtraDamageVar 后 loc 仍写 {CalculationExtra:diff()},格式化器解析失败,
卡面显示原始占位符代码并炸断卡牌生命周期(手牌消失/打出不清理)。构建时跑,缺失即失败。"""
import json, re, glob, sys
from pathlib import Path

import modmeta

ROOT = Path(__file__).resolve().parent.parent
LOC_PREFIX = modmeta.loc_prefix(ROOT)
SRC_DIR = modmeta.src_dir(ROOT)
CARD_BASE = modmeta.base_class(ROOT, 'card')

INTRINSIC = {  # 无显式名字参数的构造 → 内建键名
    'CalculationBaseVar': 'CalculationBase',
    'CalculationExtraVar': 'CalculationExtra',
    'ExtraDamageVar': 'ExtraDamage',
    'CalculatedDamageVar': 'CalculatedDamage',
    'CalculatedBlockVar': 'CalculatedBlock',
    'DamageVar': 'Damage',
    'BlockVar': 'Block',
    'CardsVar': 'Cards',
    'EnergyVar': 'Energy',
}

# 引擎在 CardModel.GetDescriptionForPile 无条件注入的变量,任何卡都可直接引用:
# IfUpgraded 配 show 格式化器({IfUpgraded:show:升级后|升级前},原版 DARKNESS 等实证)。
ENGINE_VARS = {'InCombat', 'IfUpgraded'}

def card_vars(path):
    src = open(path).read()
    keys = set()
    for m in re.finditer(r'new (\w+)\("([^"]+)"', src):
        keys.add(m.group(2))  # DynamicVar("Hits") / CalculatedVar("CalculatedX") 等
    for m in re.finditer(r'new (\w+)\(', src):
        if m.group(1) in INTRINSIC:
            keys.add(INTRINSIC[m.group(1)])
    for m in re.finditer(r'new PowerVar<(\w+)>', src):
        keys.add(m.group(1))
    for m in re.finditer(r'new PowerVar<\w+>\("([^"]+)"', src):
        keys.add(m.group(1))  # 自定义键名 PowerVar<T>("Name", ...) → 运行时键为 Name(vanilla MadScience 同款)
    return keys

# 计算三件套配对契约(2026-10-02 商店事故):CalculatedDamageVar.GetExtraVar 只认 ExtraDamage 键,
# CalculatedBlockVar.GetExtraVar 只认 CalculationExtra 键,基类 Base 都读 CalculationBase——
# 声明了 Calculated* 却缺对应 Extra/Base,打出或被奖励镜像(RandomForeseer 等)读取时 KeyNotFound。
TRIO_REQUIRED = {
    'CalculatedDamageVar': ('CalculationBase', 'ExtraDamage'),
    'CalculatedBlockVar': ('CalculationBase', 'CalculationExtra'),
}

def card_ctor_set(path):
    src = open(path).read()
    return set(re.findall(r'new (\w+)\(', src))

bad = []
varmap = {}
for path in sorted(glob.glob(f'{SRC_DIR}/Content/Cards/**/*.cs', recursive=True)):
    source = open(path, encoding='utf-8').read()
    model = re.search(r'class (\w+)\s*:\s*' + CARD_BASE + r'\b', source)
    if not model:
        continue
    cls = model.group(1)
    if cls in varmap:
        bad.append(f'重复卡牌类: {cls} ({path})')
        continue
    varmap[cls] = card_vars(path)
    ctors = card_ctor_set(path)
    for calc_var, required in TRIO_REQUIRED.items():
        if calc_var in ctors:
            missing = [k for k in required if k not in varmap[cls]]
            if missing:
                bad.append(f'{cls}: 声明 {calc_var} 但三件套缺 {missing}')
for lang in ('zhs', 'eng'):
    cards = json.load(open(f'localization/{lang}/cards.json', encoding='utf-8'))
    for k, v in cards.items():
        if not k.endswith('.description'):
            continue
        stem = k.split('.')[0].replace(LOC_PREFIX + 'CARD_', '')
        cls = ''.join(w.capitalize() for w in stem.split('_'))
        if cls not in varmap:
            bad.append(f'[{lang}] {cls}: 未找到对应卡牌类，无法检查占位符')
            continue
        used = set(re.findall(r'\{(\w+)(?::[^}]*)?\}', v)) - ENGINE_VARS
        missing = used - varmap[cls]
        if missing:
            bad.append(f'[{lang}] {cls}: {sorted(missing)}')

if bad:
    print('占位符审计失败——以下卡牌文本引用了不存在的变量:')
    print('\n'.join(bad))
    sys.exit(1)
print(f'占位符审计通过({len(varmap)} 卡 × zhs/eng)')
