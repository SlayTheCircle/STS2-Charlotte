# 现行卡表

<!-- 生成文件:scripts/export-card-table.py 从源码 ctor 与 zhs 本地化生成,勿手改。 -->
<!-- 过时校验:check.sh 调用 --check;再生成:python3 scripts/export-card-table.py -->

共 11 张（含衍生 token）。效果文本为当前运行文本;升级数值以源码与游戏内为准。
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
