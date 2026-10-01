# 卡牌、Power 与支援

当前状态见 [STATUS](../../STATUS.md)，环境与协作规则见[贡献指南](../../CONTRIBUTING.md)。本页维护当前写法。示例件以骨架自带的 `CharlotteStrike`／`CharlotteDefend` 等为参照。

## 0. 修改范围

1. 卡牌继承 `<Short>CardBase`，具名 Power 使用 `<Short>PowerBase` 或明确的隐藏标记类型，按注册特性接入。
2. 可以修改所需共享实现、本地化和资源映射；内容、运行文本及对应设计差异在同一变更更新。TypeList 池无需手工追加每张卡。
3. 本地运行与变更相称的检查；DLL 编译不要求私有素材。命令见[构建管线](pipeline.md)。
4. 新增原案外内容须在设计对照与花名册审计的新增集合注明；玩法调整按已确认设计处理。

卡牌目录约定：普通池使用 Attacks／Skills／Powers 下的稀有度子目录；Basic、Ancient、Tokens 单独集中。命名空间保持 `<Short>Mod.Content.Cards`，注册特性、复合 ID 和图片文件名与类名保持对应关系。新增卡牌直接放入相应子目录，检查脚本递归枚举。

## 1. 专有名词关键词（名词解释系统）

**悬停注释按声明集合生成，不扫描卡面文本**——文本里写了名词但没挂声明的卡，悬停就没有注释（模板源工程实测踩过）。

- **机制名词**（你 Mod 的核心机制词）→ 关键词横幅 + 文字解释框，在 `CanonicalKeywords` 挂：
  - 卡牌：`<Short>Keywords.AddTo(set, <Short>Keywords.X)`（签名 `public override IEnumerable<CardKeyword>`，protected 会 CS0507；已有原生关键词的卡在同一覆写里合并）。
  - **非卡牌模型（遗物／能力／药水／附魔）没有该管线**，须在 `AdditionalHoverTips`（模板基类）或 `ExtraHoverTips`（附魔）显式挂词条——`<Short>Keywords.HoverTips(...)` 助手转发 RitsuLib `ToHoverTips()`。
- **卡牌名词**（token 卡等）→ **卡面预览**（原版「精准」引小刀同款）：`AdditionalHoverTips => new[] { HoverTipFactory.FromCard<TokenCard>() }`（IHoverTip 不可单值直赋，必须包数组）。

解释文本的唯一权威来源：`localization/{lang}/card_keywords.json`（键 `STS2_<MOD>_KEYWORD_*.title/.description`，在 ModEntry 注册）。**修改机制实现时必须同步核对解释文本**，文案不准就是事故。描述正文里的名词用 `[gold]名词[/gold]` 高亮。

## 2. 支援系（附魔底盘，vanilla 原生）

施加型支援卡 = 附魔类 + 施加卡两张东西；协同型卡只做查询。骨架样例：`CharlotteVigorSupport`。

| 调用/成员 | 用途 |
|---|---|
| `CardCmd.Enchant<T>(card, amount)` | 给卡施加附魔（不可附魔会抛，选牌过滤器先排除） |
| `card.Enchantment is <Short>SupportEnchantment` | 协同卡查询「带有支援效果的牌」（**唯一判据**，别用具体附魔型判） |
| 继承 `<Short>SupportEnchantment` | 支援附魔基类：HasExtraCardText 已开、图标自动解析、标记家族 |
| `EnchantDamageAdditive/Multiplicative`、`EnchantBlockAdditive/Multiplicative`、`EnchantPlayCount` | 修改被附魔卡的伤害/格挡/打出次数——**必须用这组专用钩子**（先于遗物/能力钩子），不要用 Modify* |
| `OnPlay(ctx, cardPlay)` | 被附魔的卡打出时触发 |
| `OnEnchant()` + `RecalculateValues()` | 附魔即改卡（如费用/关键词类支援，类比升级） |
| `CanEnchantCardType(CardType)` | 限定可附魔卡型（如仅攻击） |
| loc 表 `enchantments` | 键 `STS2_<MOD>_ENCHANTMENT_<类名蛇形大写>.{title,description,extraCardText}`，extraCardText 附加显示在被附魔卡面上，`{Amount}` 可用 |

硬约束：
- **作用域=本场战斗，由引擎保证**（战斗牌堆是主卡组的克隆，战斗内附魔随 CombatState 消亡）——不要写清场钩子；
- 一卡一附魔槽：已附魔的卡不能再次获得支援（协同「换支援」设计需先 `CardCmd.ClearEnchantment`）；
- 图标：专属图按类名 `images/enchantments/<类名>.png`，缺图回落共享徽记 `<short>_support.png`。

## 3. 卡牌类模板与风格

以骨架的示例卡为基准：sealed class 继承 `<Short>CardBase`（不要直接继承 `ModCardTemplate`——基线类按类名约定解析卡图 `res://<ModId>/images/cards/<类名>.png`，缺图自动回退占位），构造器传 `(费用, CardType, CardRarity, TargetType)`，`OnPlay`/`OnUpgrade` 覆写，XML doc 注释写设计文案。

映射表：
- 类型：攻击=Attack 技能=Skill 能力=Power
- 稀有度：**普通=Common，罕见=Uncommon，稀有=Rare**（白卡／蓝卡／金卡）
- 关键词（加进 `CanonicalKeywords`）：消耗=Exhaust 虚无=Ethereal 固有=Innate 保留=Retain
- 打击类卡必须显式声明 `protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };`。原版打击木偶等联动读取 `Tags`，不按本地化名称或 C# 类名判断。新增或改名时按设计确认归属，不能按某种语言的标题自动推断。
- 目标：自身=Self 单体敌人=AnyEnemy
- 获得格挡的卡：`public override bool GainsBlock => true;`
- 多段攻击：`DamageCmd.Attack(...).WithHitCount(n)`；全体敌人参考 vanilla Shiv 的 `TargetingAllOpponents(combatState)`
- 能力卡施加 Power：`PowerVar<T>` 声明数值（loc 用 `{T类名:diff()}` 引用）；升级抬数值 `UpgradeValueBy`，降费用 `base.EnergyCost.UpgradeBy(-1)`

## 4. 实战踩坑清单（违反会出运行时 Bug）

1. **CalculatedVar 三件套**：卡面显示「每有 X 则 +Y」类动态数值，必须同时声明基准/每份/计算三件，显示值 = 基准 + 每份 × 份数。**三件缺一，卡牌创建时直接 KeyNotFoundException**。multiplier 必须是静态 lambda 且空安全。**负缩放可用**（「每层 X -Y」用负每份值）。
   - 伤害/格挡缩放卡**必须用预览感知变体**（参考 vanilla PerfectedStrike/Stack）：
     - 伤害：`CalculationBaseVar(基准)` + `ExtraDamageVar(每份)` + `CalculatedDamageVar(ValueProp.Move)`；OnPlay 读 `base.DynamicVars.CalculatedDamage.Calculate(target)`
     - 格挡：`CalculationBaseVar(基准)` + `CalculationExtraVar(每份)` + `CalculatedBlockVar(ValueProp.Move)`；OnPlay 读 `base.DynamicVars.CalculatedBlock.Calculate(target)`
   - 通用 `CalculatedVar` 的面板**不含**力量/虚弱/敏捷修正，只用于非伤害格挡的自定义数值。
2. **先算后耗**：凡「消耗资源结算效果」的卡，必须在消耗**之前**读取/计算数值（后算会把面板值算成基准值）。
3. `PowerCmd.Apply<T>(...)` 返回 `Task<T?>`，战斗结束等时点会返回 null，接返回值必须判空。
4. **0.107.1 API 垫片**：跨版本调用差异（FromCard 双参、CardPlay.GetPlayer、LoseBlock 等）一律走 `Content/Compat/` 下注入游戏命名空间的扩展方法垫片，调用点零感知；新增跨版本差异时在这里吸收，不要散在内容代码里 `#if`。

## 5. 本地化

卡牌键 `STS2_<MOD>_CARD_<类名蛇形大写>.{title,description}`；可见 Power 三键 `{title,description,smartDescription}`。描述里的动态数值用 `{VarName:diff()}`（audit-placeholders 校验变量存在）。**PowerVar 占位符用类名不经前缀**（`{LoadPower:diff()}` 形态）。中英两表键集必须一致（audit-assets 校验）。
