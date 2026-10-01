# 现行卡表

<!-- 生成文件:scripts/export-card-table.py 从源码 ctor 与 zhs 本地化生成,勿手改。 -->
<!-- 过时校验:check.sh 调用 --check;再生成:python3 scripts/export-card-table.py -->

共 32 张（含衍生 token）。效果文本为当前运行文本;升级数值以源码与游戏内为准。
稀有度颜色对照：普通=白卡，罕见=蓝卡，稀有=金卡。
原案数值与设计过程见[技术历史](../history/design/README.md)。

## 初始卡（4）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 防御 | 初始 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。 |
| 咔嚓！ | 初始 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。选择你的一张手牌[gold]留影[/gold]。 |
| 茄子！ | 初始 | 0 | 攻击 | 造成{Damage:diff()}点伤害。你的消耗牌堆里每有1张牌，就额外造成{ExtraDamage:diff()}点伤害。{InCombat:
（当前共造成{CalculatedDamage:diff()}点伤害）\|} |
| 打击 | 初始 | 1 | 攻击 | 造成{Damage:diff()}点伤害。 |

## 攻击（14）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 闪光打击 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害。选择1张手牌[gold]留影[/gold]。 |
| 聚焦打击 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害，给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 高危调查 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害。抽{Cards:diff()}张牌。这张卡的伤害下降1点。 |
| 正当防卫 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害，获得{Block:diff()}点[gold]格挡[/gold]。 |
| 精彩纷呈 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害，然后将这张牌[gold]留影[/gold]。 |
| 一针见血 | 普通 | 2 | 攻击 | 对所有敌人造成{Damage:diff()}点伤害。 |
| 按下快门 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害。本回合内，该敌人每受到1次攻击牌的伤害，都会额外失去{ShutterPursuitPower:diff()}点生命。 |
| 羽笔打击 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害。生成1张[gold]新闻[/gold]。 |
| 连续抓拍 | 普通 | 2 | 攻击 | 为随机1张手牌[gold]留影[/gold]，造成{Damage:diff()}点伤害。 |
| 可靠信源 | 普通 | 0 | 攻击 | 造成{CalculatedDamage:diff()}点伤害，抽{Cards:diff()}张牌。本回合每打出过1张其他攻击牌，伤害提升{ExtraDamage:diff()}点。 |
| 掷地有声 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害，如果这是你本回合打出的第一张攻击牌，则再造成{ExtraDamage:diff()}点伤害。 |
| 针砭时弊 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害。抽{Cards:diff()}张牌，如果抽到的牌是0费的牌，则额外抽1张。 |
| 变焦摄像 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害，给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 大字通告 | 罕见 | 2 | 攻击 | 造成{Damage:diff()}点伤害，给予{WeakPower:diff()}层虚弱。 |

## 技能（7）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 笔走龙蛇 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。下个回合开始时，你的格挡不会消失。 |
| 鞭辟入里 | 普通 | 0 | 技能 | 去除敌人所有的格挡与人工制品，并给予{WeakPower:diff()}层虚弱。 |
| 早间特报 | 普通 | 0 | 技能 | 从3张[gold]新闻[/gold]中挑选1张加入你的手牌。 |
| 校对样稿 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]，抽{Cards:diff()}张牌。 |
| 不攻自破 | 普通 | 0 | 技能 | 获得{CalculatedBlock:diff()}点[gold]格挡[/gold]，给予{VulnerablePower:diff()}层易伤。本回合内每打出过1张攻击牌，额外获得{CalculationExtra:diff()}点格挡。 |
| 抽丝剥茧 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 撰写稿件 | 普通 | 0 | 技能 | 给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
