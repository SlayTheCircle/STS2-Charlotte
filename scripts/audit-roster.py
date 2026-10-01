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
DEFERRED = {
    '三审三校',
    '临场发挥',
    '争分夺秒',
    '全勤奖金',
    '再拍一张',
    '冷色摄影',
    '切中要害',
    '剪贴相册',
    '力透纸背',
    '加班加点',
    '动作捕捉',
    '勘正谬误',
    '单刀直入',
    '占领头版',
    '吸引视线',
    '唇枪舌剑',
    '复制胶卷',
    '多重取证',
    '奋笔疾书',
    '妙笔生花',
    '实地考察',
    '左右逢源',
    '广角镜头',
    '延时摄影',
    '开门见山',
    '忠实记录',
    '按图索骥',
    '摄影形态',
    '摄影技巧',
    '文思泉涌',
    '时效原则',
    '时效核验',
    '暂时规避',
    '曝光录像',
    '最佳记者',
    '有力物证',
    '核实流程',
    '核验稿件',
    '检索信息',
    '洛阳纸贵',
    '深入调查',
    '温馨笔触',
    '爆炸新闻',
    '特制镜头',
    '现场报道',
    '看镜头！',
    '真实至上',
    '真相大白',
    '神来之笔',
    '笑一个！',
    '精力充沛',
    '紧跟时事',
    '线索收集',
    '职业素养',
    '背光拍摄',
    '脑洞大开',
    '谨遵事实',
    '走街串巷',
    '跟进调查',
    '辟谣祛魅',
    '追踪调查',
    '野外调查',
    '风雨满楼',
}
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
