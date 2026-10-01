# 世界线（可选模块）

世界线＝角色 Mod 的章节揭示／内容解锁／先古对话体系。**模板骨架不含其代码**：纪元肖像是全局域 PNG，媒体不入公开仓，骨架无法自证。接线时参照源工程 [STS2-Navia](https://github.com/SlayTheCircle/STS2-Navia) 的 `Content/Timeline/` 与角色解锁属性。本页保留机制知识。

## 章节与三池门控

故事与纪元**必须继承 RitsuLib 脚手架基类**：ModStoryTemplate（Id 由 StoryKey 推导，章节顺序来自 RegisterStoryEpoch 的绑定）；各章继承 Relic／Potion／CardUnlockEpochTemplate（UnlockText 与 QueueUnlocks 由模板提供，每章解锁物恰好三件以满足原版展示文案读取前三项）。原版时间线屏幕的槽位合并按 `is ModEpochTemplate` 过滤，直接继承 EpochModel 会出现「解锁链照常触发但整列不渲染」——该契约不可绕过。

奖励 UI 与内容可获得性分别接线：模板 QueueUnlocks 负责揭示界面；遗物／药水池与卡池过滤按对应纪元过滤，与纪元类的 UnlockTypes 数组共用同一来源。变更章节奖励时同时更新数组、池过滤、两语言文本及验收场景。

## 纪元肖像（全局资源）

纪元肖像是**两个独立资源槽**：大图按全局 `res://images/timeline/epoch_portraits/<键>.png` 推导（放 `assets/global/`，打包器保留全局路径，不能套用普通 Mod 前缀）；缩略图原版走 epoch_atlas 图集、mod 无图集条目会显示 NOPE，须经各纪元类的 AssetProfile.PackedPortraitPath 覆盖指向 `res://<ModId>/images/timeline/` 下的派生图（从大图裁切 272×174）。

## 先古对话与资源

RitsuLib 根据 `localization/{lang}/ancients.json` 自动组装对话。键族为 `<先古Entry>.talk.<角色Entry>.<序号>-<行号>.char／ancient`，续接键 `.next`。要点：

- **`.next` 是逐行键**：每轮除末行外各行都要有，缺失时游戏把键名原文回显在界面角落。
- **行号带 `r` 后缀** = 加入可重复池：轮次精确匹配用尽后从已解锁轮次随机重放（原版行为）。同轮全行（含 .next）一致，混合会抛异常。
- **同轮变体**：多段对话共享同一 VisitIndex 时，游戏在候选中随机挑一段播放；序号 >0 的变体段需 `<序号>-visit` 控制键回指轮次（如 `1-visit`=0），否则被默认映射成后续轮次。
- **拜访语义**：各先古按「角色×先古」计次；**整个存档的首次遇见由全角色共享的 firstVisitEver 通用台词占用**——纯新档各族第 0 轮不可达，与原版一致；第 N 次遇见显示 VisitIndex=N-1 的轮次。非建筑师的序号→轮次默认映射为 0→0、1→1、2→4、+3。
- 建筑师终局走同一机制：对话序号即登顶轮次，另有可选的 -visit／-attack／-startattack／-endattack 键控制编排；无键时 RitsuLib 空对话兜底并告警。
- 角色初始遗物的先古升级（欧罗巴斯之触类）：用 RitsuLib 的注册映射挂获取时替换，**不要**按「以前遇见过」的相遇记录自建持久化。

调试：控制台 `ancient <Entry>` 直达画面，但绕过自然门控，仍需自然进度验证。

## 验收边界

分别确认新档与已有档的角色可用性、揭示条件、奖励 UI、三池过滤、纪元图、先古对话。世界线完整验收语义见源工程的 STATUS 记录。
