# 事件实现

当前事件实现位于 [Content/Events](../../src/STS2-Charlotte/Content/Events/)：新事件×3 继承 CharlotteEventBase（ModEventTemplate 脚手架），原版追加选项×5 在 [VanillaEventCharlotteOptions](../../src/STS2-Charlotte/Content/Events/VanillaEventCharlotteOptions.cs)。叙述与效果见设计案（私有），机制知识沿用本页。

## 当前接线

| 新事件 | Act | 选项与实现 |
|---|---|---|
| BreathtakingView（美不胜收） | Overgrowth，一层 | 河水=选打击/防御附魔 Spiral 1（FromDeckForEnchantment 过滤重载）；鸟儿=原版 ByrdSwoop 入手牌；晚霞=3 张全升级卡牌奖励（CreateForReward 后统一 Upgrade，非概率 roll） |
| MerchantsRequest（商人的请求） | Hive，二层 | 应允=随机稀有度遗物；拒绝=角色药池+共享池按 Rare 过滤抽取（空池时仅完结，设计未定义空池行为） |
| HeatedDebate（激烈的辩论） | Glory，三层 | 煎制=失 3 最大生命+回 22（DrowningBeacon 的 LoseMaxHp 范式）；清蒸=失第一瓶药水+50 金（工程侧取第一瓶，设计未指定选择方式）；无药水时锁死选项 |

设计案事件名与原版对照：**透镜高塔=DrowningBeacon（沉没灯塔，文本「掉入湖面」即塔下水面）、圆桌骑士=RoundTeaParty（圆桌茶会）**；透镜高塔与药水快递员的新选项奖励为原版遗物 FresnelLens/MembershipCard（Mirror 2026-10-02 确认），直接 `ModelDb.Relic` 引用，无需自建。

| 原版类 | 追加选项 | 行为 |
|---|---|---|
| DrowningBeacon | 擦拭镜片 | 失去初始遗物（MonsieurVerite），获得原版菲涅耳透镜 |
| SelfHelpBook | 记录有用的信息 | 选一卡升级 |
| RoundTeaParty | 鞭辟入里地分析 | 随机药水（角色池+共享池） |
| PotionCourier | 试着唤醒他 | 失去所有药水，获得原版会员卡 |
| CrystalSphere | 抵押 | 移除一卡 + Doubt 入手牌 + 占卜 4 次（CrystalSphereMinigame(player, rng, 4)，PaymentPlan 范式） |

三个新事件肖像待美术（素材库中的 事件1.png 画面主体为娜维娅，不可用于夏洛蒂，已退回）；AssetProfile 未覆写，走 RitsuLib 占位图与默认背景。

## 注册与门控

新事件继承 `CharlotteEventBase`，由 RitsuLib 的 RegisterActEvent 注册到对应 Act。IsAllowed 查询队伍中是否有本角色——「队伍含本角色」与「事件归属者是本角色」（原版追加选项查询事件 Owner.Character）是**两种不同门控**，多人验收应覆盖两者。

## 原版追加选项

给原版事件追加角色专属选项：在 `EventModel.GenerateInitialOptionsWrapper` 基方法上打 Harmony postfix（本仓实现即 VanillaEventCharlotteOptions）。要点：

- 当前目标未覆写该方法；游戏升级时重新核对方法及目标类。
- SetEventFinished 是受保护成员，外部处理器使用启动时缓存的反射委托；找不到方法时记录错误并停止追加，保留原版选项。
- 原版追加选项使用原版 Entry 下的 `<MOD>_*` 键。

## 初始化通道（关键教训）

**ModEntry 带 ModInitializer 时，游戏加载器不会再自动 PatchAll**——Init 完成注册后必须显式调用 `Harmony.PatchAll`。发现补丁类、编译通过或看到新事件注册，都不能证明追加选项已执行；补丁声明、DLL 编译、日志与真实选项出现提供不同层次的证据。

## 本地化与资源

新事件复合 Entry 为 `STS2_<MOD>_EVENT_<类名蛇形大写>`，在 `localization/{lang}/events.json` 维护：`Entry.title`、`Entry.pages.INITIAL.description`、`Entry.pages.INITIAL.options.<KEY>.title／description`、各结果页 description。ModEventTemplate 的 InitialOptionKey 和 PageDescription 拼接层级。动态变量随 CanonicalVars 声明，选牌提示、代价、悬停预览与效果一致。

事件肖像 `images/events/<类名>.png`；尺寸见[素材规范](assets.md)。

## 修改与验收

同步修改事件类、中英本地化、资源映射和对应设计差异。源码检查覆盖键和文本资源，PCK 检查覆盖代表性资源解析，真实游戏还需确认门控、选项、支付不足、不可操作卡、空池和奖励结果。DevConsole 的 `event <Entry>` 可直达目标事件——绕过自然生成门控，只能验收处理与呈现，不能证明 IsAllowed 或自然生成概率正确。未进行的多人、存档或边界场景如实记录。
