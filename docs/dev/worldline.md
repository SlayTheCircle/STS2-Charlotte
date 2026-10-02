# 世界线

世界线＝章节揭示／内容解锁／先古对话体系。当前代码位于 [Content/Timeline](../../src/STS2-Charlotte/Content/Timeline/) 及 [Charlotte](../../src/STS2-Charlotte/Content/Characters/Charlotte.cs) 上的揭示条件特性；机制知识来自源工程 STS2-Navia 的接线经验。

## 四章与三池门控

故事与纪元**必须继承 RitsuLib 脚手架基类**：CharlotteStory 继承 ModStoryTemplate（Id 由 StoryKey 推导，章节顺序来自 RegisterStoryEpoch 的绑定）；各章继承 Mod／Relic／Potion／CardUnlockEpochTemplate（UnlockText 与 QueueUnlocks 由模板提供，每章解锁物恰好三件以满足原版展示文案读取前三项）。原版时间线屏幕的槽位合并按 `is ModEpochTemplate` 过滤，直接继承 EpochModel 会出现「解锁链照常触发但整列不渲染」——该契约不可绕过。每章 Id 为 STS2_CHARLOTTE_EPOCH_1 至 _4，StoryId 为 Charlotte。

| 章 | 揭示条件（挂 Charlotte 类） | 奖励 | 时间线列 |
|---|---|---|---|
| 新的报道 | 完成一局夏洛蒂 | 剧情节点；角色安装后直接可选 | Blight2 |
| 全新视角 | 夏洛蒂首胜 | 追踪调查、摄影形态、广角镜头 | Flourish2 |
| 独家爆料 | 夏洛蒂累计击败三个首领 | 特殊分析变焦镜头、镜头盖、采访稿记录本 | Flourish3 |
| 独家专访 | 夏洛蒂进阶一通关 | 香浓的热咖啡、镜头清洁剂、提神醒脑茶 | Invitation5 |

各章通过 AutoTimelineSlot 落位；模板密封了 Era/EraPosition，由布局注册表解析（列内首个空闲位置），不要手工覆写。奖励 UI 与内容可获得性分别接线：模板 QueueUnlocks 负责揭示界面；[CharlotteRelicPool.GetUnlockedRelics](../../src/STS2-Charlotte/Content/RelicPools/CharlotteRelicPool.cs)、[CharlottePotionPool.GetUnlockedPotions](../../src/STS2-Charlotte/Content/PotionPools/CharlottePotionPool.cs) 和 [CharlotteCardPool.FilterThroughEpochs](../../src/STS2-Charlotte/Content/CardPools/CharlotteCardPool.cs) 按对应纪元过滤内容，与纪元类的 *UnlockTypes 类型数组共用同一来源。六件解锁遗物／药水的稀有度走常规档（组织先例：Navia 解锁件同为 Common；提神醒脑茶按效果强度定 Uncommon），解锁前的不进池由池过滤承担。

变更章节奖励时同时更新对应纪元的 UnlockTypes 数组、池过滤、两语言文本及验收场景。

## 纪元肖像（全局资源）

四章立绘由 scripts/art/stories.sh 从母版派生：1672×941 大图放 `assets/global/`（全局 `res://images/timeline/epoch_portraits/sts2_charlotte_epoch_<序号>.png` 推导，打包器保留全局路径，不能套用普通 Mod 前缀）；272×174 缩略图经各纪元类 AssetProfile.PackedPortraitPath 覆写指向 `res://STS2-Charlotte/images/timeline/`。派生和资源存在性检查不能代替游戏内时间线验收。

## 先古对话（B8 待接线）

RitsuLib 根据 `localization/{lang}/ancients.json` 自动组装对话。键族为 `<先古Entry>.talk.STS2_CHARLOTTE_CHARACTER_CHARLOTTE.<序号>-<行号>.char／ancient`，续接键 `.next`。要点（Navia 实战沉淀）：

- **`.next` 是逐行键**：每轮除末行外各行都要有，缺失时游戏把键名原文回显在界面角落。
- **行号带 `r` 后缀** = 加入可重复池：轮次精确匹配用尽后从已解锁轮次随机重放（原版行为）。同轮全行（含 .next）一致，混合会抛异常。全族对话皆可重复（2026-10-01 Navia 设计者定案）。
- **同轮变体**：多段对话共享同一 VisitIndex 时，游戏在候选中随机挑一段播放；序号 >0 的变体段需 `<序号>-visit` 控制键回指轮次（如 `1-visit`=0），否则被默认映射成后续轮次。坦克斯两段即此形态。
- **拜访语义**：各先古按「角色×先古」计次；**整个存档的首次遇见由全角色共享的 firstVisitEver 通用台词占用**——纯新档各族第 0 轮不可达，与原版一致；第 N 次遇见显示 VisitIndex=N-1 的轮次。非建筑师的序号→轮次默认映射为 0→0、1→1、2→4、+3。
- 建筑师终局走同一机制：对话序号即登顶轮次，另有可选的 -visit／-attack／-startattack／-endattack 键控制编排；无键时 RitsuLib 空对话兜底并告警。
- 角色初始遗物的先古升级（欧罗巴斯之触类）：用 RitsuLib 的 RegisterTouchOfOrobasRefinement 注册映射挂获取时替换（已接：MonsieurVerite→千织屋纪念版），**不要**按「以前遇见过」的相遇记录自建持久化。

调试：控制台 `ancient <Entry>` 直达画面，但绕过自然门控，仍需自然进度验证。

## 验收边界

分别确认新档与已有档的角色可用性、揭示条件、奖励 UI、三池过滤、纪元图（图接入后）、先古对话（B8 后）。游戏内验收证据由 [STATUS](../../STATUS.md) 维护。
