# 现行卡表

<!-- 生成文件:scripts/export-card-table.py 从源码 ctor 与 zhs 本地化生成,勿手改。 -->
<!-- 过时校验:check.sh 调用 --check;再生成:python3 scripts/export-card-table.py -->

共 95 张（含衍生 token）。效果文本为当前运行文本;升级数值以源码与游戏内为准。
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

## 攻击（33）

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
| 冷色摄影 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害。选择1张弃牌堆中的牌[gold]留影[/gold]。本回合内，每打出过1张攻击牌，就可以额外[gold]留影[/gold]一张弃牌堆中的牌。 |
| 开门见山 | 罕见 | 0 | 攻击 | 造成{Damage:diff()}点伤害，给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 走街串巷 | 罕见 | 2 | 攻击 | 造成{Damage:diff()}点伤害。选择你手中的1张[gold]新闻[/gold]，将一张复制品加入你的抽牌堆。 |
| 忠实记录 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害，给予{WeakPower:diff()}层虚弱。本回合打出其他攻击牌时，都会给予目标{FaithfulRecordPower:diff()}层虚弱。 |
| 大字通告 | 罕见 | 2 | 攻击 | 造成{Damage:diff()}点伤害，给予{WeakPower:diff()}层虚弱。 |
| 线索收集 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害。选择手牌中1张[gold]留影纪念[/gold]，这张牌可以免费打出。 |
| 深入调查 | 罕见 | 2 | 攻击 | 造成{Damage:diff()}点伤害。将此卡的1张0费的复制品置入你的抽牌堆中。 |
| 现场报道 | 罕见 | 1 | 攻击 | 给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。下个回合开始时，给予{DelayedFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 力透纸背 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害。如果敌人的意图是防御，则额外再造成{ExtraDamage:diff()}点伤害。 |
| 有力物证 | 罕见 | 3 | 攻击 | 造成{CalculatedDamage:diff()}点伤害。本回合内每打出过1张其他牌，这张卡的伤害就下降{ExtraDamage:diff()}点，同时耗能减少1。 |
| 延时摄影 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害，消耗你手牌中的1张[gold]留影纪念[/gold]。 |
| 核验稿件 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害，从弃牌堆中选择至多{Cards:diff()}张费用为0的牌加入你的手牌。 |
| 左右逢源 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害。在本场战斗中，这张卡以外的攻击牌造成的伤害+{PressMomentumPower:diff()}。 |
| 辟谣祛魅 | 稀有 | 0 | 攻击 | 造成{Damage:diff()}点伤害。这张卡的耗能增加1，且造成的伤害{IfUpgraded:show:变为三倍\|翻倍}。 |
| 曝光录像 | 稀有 | 4 | 攻击 | 造成{Damage:diff()}点伤害。消耗牌堆中的每一张牌，都会使这张卡的耗能下降1点。 |
| 神来之笔 | 稀有 | 0 | 攻击 | 消耗你抽牌堆中所有的[gold]新闻[/gold]。每消耗1张，就对随机敌人造成{Damage:diff()}点伤害1次。 |
| 多重取证 | 稀有 | 2 | 攻击 | 造成{Damage:diff()}点伤害{Cards:diff()}次。将此卡的1张费用降低1点的复制品放入你的弃牌堆。[gold]消耗[/gold]。 |
| 单刀直入 | 稀有 | 0 | 攻击 | 本场战斗中，你每打出过1张攻击牌，此卡就造成{ExtraDamage:diff()}点伤害。 |
| 追踪调查 | 稀有 | 9 | 攻击 | 对全体敌人造成{Damage:diff()}点伤害。每当你消耗1张牌，这张卡的费用就降低1点。（使用后恢复。） |
| 唇枪舌剑 | 稀有 | 0 | 攻击 | 造成{Damage:diff()}点伤害。回合开始时，若此牌在消耗牌堆中，则在本场战斗中，将此卡的伤害提升{ExtraDamage:diff()}点，然后将其返回你的手牌。 |

## 技能（30）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 笔走龙蛇 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。下个回合开始时，你的格挡不会消失。 |
| 鞭辟入里 | 普通 | 0 | 技能 | 去除敌人所有的格挡与人工制品，并给予{WeakPower:diff()}层虚弱。 |
| 早间特报 | 普通 | 0 | 技能 | 从{IfUpgraded:show:全部(7张)\|3张}[gold]新闻[/gold]中挑选1张加入你的手牌。 |
| 校对样稿 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]，抽{Cards:diff()}张牌。 |
| 不攻自破 | 普通 | 0 | 技能 | 获得{CalculatedBlock:diff()}点[gold]格挡[/gold]，给予{VulnerablePower:diff()}层易伤。本回合内每打出过1张攻击牌，额外获得{CalculationExtra:diff()}点格挡。 |
| 抽丝剥茧 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 撰写稿件 | 普通 | 0 | 技能 | 给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 背光拍摄 | 罕见 | 2 | 技能 | 选择你手牌中的1张牌。如果是[gold]留影纪念[/gold]，则将2张0费复制品加入你的手牌。如果不是，则[gold]留影[/gold]这张牌，然后抽3张牌。 |
| 脑洞大开 | 罕见 | 1 | 技能 | 消耗你手中的一张[gold]新闻[/gold]。接下来的{BrainstormPower:diff()}个回合开始时，将这张新闻的0费、带有消耗的复制品加入你的手牌。 |
| 占领头版 | 罕见 | 1 | 技能 | 在你的抽牌堆里生成随机{Cards:diff()}张[gold]新闻[/gold]，并随机抽取其中一张。 |
| 时效核验 | 罕见 | 1 | 技能 | 获得{CalculatedBlock:diff()}点[gold]格挡[/gold]。本回合内你每消耗过1张牌，就再获得{CalculationExtra:diff()}点格挡。 |
| 奋笔疾书 | 罕见 | 1 | 技能 | 抽满你的手牌。{IfUpgraded:show:\|本回合内你无法再获得格挡。} |
| 吸引视线 | 罕见 | 1 | 技能 | 给予自身{LensFocusPower:diff()}层[gold]聚焦[/gold]，抽{Cards:diff()}张牌。 |
| 勘正谬误 | 罕见 | 1 | 技能 | 丢弃至多{Cards:diff()}张[gold]新闻[/gold]，每丢弃1张就获得{Block:diff()}点[gold]格挡[/gold]。 |
| 核实流程 | 罕见 | 1 | 技能 | 给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 野外调查 | 罕见 | 1 | 技能 | 恢复等同于你拥有的[gold]聚焦[/gold]层数的生命值。 |
| 跟进调查 | 罕见 | 1 | 技能 | 本回合内，当你打出[gold]新闻[/gold]时，抽{FollowUpPower:diff()}张牌。 |
| 临场发挥 | 罕见 | 1 | 技能 | 给予自身{LensFocusPower:diff()}层[gold]聚焦[/gold]，获得{Block:diff()}点[gold]格挡[/gold]。本回合内，你的生命值不会因为[gold]聚焦[/gold]而减少。 |
| 检索信息 | 罕见 | 1 | 技能 | 抽{Cards:diff()}张牌。丢弃其中所有的无色牌。 |
| 紧跟时事 | 罕见 | 1 | 技能 | {IfUpgraded:show:本场战斗中\|本回合中}，当你的[gold]新闻[/gold]被消耗时，生成这张牌的[gold]留影纪念[/gold]。 |
| 再拍一张 | 罕见 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。选择手牌中的1张[gold]留影纪念[/gold]，将一张复制品放入你的抽牌堆。 |
| 加班加点 | 罕见 | 1 | 技能 | 消耗1张牌，获得{Energy:diff()}点能量。 |
| 精力充沛 | 罕见 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。你下次造成伤害时，将获得与伤害量等量的格挡。 |
| 实地考察 | 罕见 | 1 | 技能 | 你的抽牌堆中每有1张[gold]留影纪念[/gold]，都会对全体敌人施加{LensFocusPower:diff()}层[gold]聚焦[/gold]。 |
| 暂时规避 | 罕见 | 2 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。在下个回合获得{EnergyNextTurnPower:diff()}点能量。 |
| 爆炸新闻 | 稀有 | 3 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。用随机的[gold]新闻[/gold]填满你的手牌。 |
| 按图索骥 | 稀有 | 0 | 技能 | 从你的抽牌堆、弃牌堆各选择{Cards:diff()}张牌加入你的手牌。 |
| 争分夺秒 | 稀有 | 1 | 技能 | 给予{LensFocusPower:diff()}层[gold]聚焦[/gold]。结束你的回合。 |
| 真相大白 | 稀有 | 2 | 技能 | 消耗目标所有的[gold]聚焦[/gold]，然后对目标造成{CalculationExtra:diff()}倍于原有[gold]聚焦[/gold]层数的伤害。 |
| 广角镜头 | 稀有 | 2 | 技能 | [gold]留影[/gold]你所有的手牌。这些牌的费用降低1。 |

## 能力（19）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 特制镜头 | 罕见 | 1 | 能力 | 每当你打出[gold]留影纪念[/gold]，获得{CustomLensPower:diff()}点[gold]格挡[/gold]。 |
| 动作捕捉 | 罕见 | 1 | 能力 | 每当你打出1张[gold]留影纪念[/gold]，抽{MotionCapturePower:diff()}张牌，消耗{MotionCapturePower:diff()}张手牌。 |
| 全勤奖金 | 罕见 | 1 | 能力 | 每回合开始时，获得{PerfectAttendancePower:diff()}金币。 |
| 风雨满楼 | 罕见 | 0 | 能力 | 每回合你第一次失去生命时，将{StormBrewingPower:diff()}张随机[gold]新闻[/gold]添加到你的手牌。 |
| 温馨笔触 | 罕见 | 2 | 能力 | 每当你[gold]留影[/gold]或给予[gold]聚焦[/gold]时，若生命值低于最大生命值的50%，则恢复{WarmBrushstrokePower:diff()}点生命。 |
| 谨遵事实 | 稀有 | 2 | 能力 | 每当场上的[gold]聚焦[/gold]造成伤害时，你获得与那个伤害值相同的格挡。 |
| 最佳记者 | 稀有 | 1 | 能力 | 战斗结束时，获得{BestJournalistPower:diff()}点最大生命值。 |
| 文思泉涌 | 稀有 | 1 | 能力 | 每当你没有手牌时，抽{FlowOfIdeasPower:diff()}张牌。 |
| 妙笔生花 | 稀有 | 1 | 能力 | 每个回合开始时，将一张随机的[gold]新闻[/gold]牌添加到你的手牌。 |
| 洛阳纸贵 | 稀有 | 2 | 能力 | 每回合你第一次打出[gold]新闻[/gold]时，将这张[gold]新闻[/gold]的1张复制品加入你的手牌。 |
| 摄影形态 | 稀有 | 3 | 能力 | 你每消耗1张牌，对全体敌人造成{PhotographyFormPower:diff()}点伤害。 |
| 摄影技巧 | 稀有 | 1 | 能力 | 每回合你打出的第一张攻击牌会被打出2次。 |
| 职业素养 | 稀有 | 2 | 能力 | 每个回合开始时，获得{ProfessionalismPower:diff()}点能量。 |
| 复制胶卷 | 稀有 | 2 | 能力 | 每回合你打出的第一张[gold]留影纪念[/gold]将会打出2次。 |
| 剪贴相册 | 稀有 | 1 | 能力 | 当你打出[gold]留影纪念[/gold]时，抽{ScrapbookPower:diff()}张牌。 |
| 切中要害 | 稀有 | 2 | 能力 | 每当你给予[gold]聚焦[/gold]，都对全体敌人造成{SharpReportPower:diff()}点伤害。 |
| 时效原则 | 稀有 | 2 | 能力 | 每当你的[gold]新闻[/gold]进入消耗牌堆，都会对全体敌人造成{TimelinessPower:diff()}点伤害。 |
| 三审三校 | 稀有 | 3 | 能力 | 每回合开始时，从3张[gold]新闻[/gold]中选择1张加入你的手牌。 |
| 真实至上 | 稀有 | 1 | 能力 | 你每打出1张牌，都会获得{TruthAbovePower:diff()}点[gold]格挡[/gold]。 |

## 先古强化（2）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 看镜头！ | 先古 | 0 | 能力 | 选择X张手牌[gold]留影[/gold]。在接下来的X个回合内，被消耗的手牌中的{IfUpgraded:show:一张由你选择的牌\|随机一张牌}将升级并返回你的手牌。 |
| 笑一个！ | 先古 | 0 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。选择你最多{Cards:diff()}张手牌，将他们[gold]留影[/gold]。 |
