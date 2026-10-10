# 当前状态（2026-10-10 · 0.1.5 报告五项修复已双端发版）

本文件维护当前实现、证据边界、限制与待办。安装入口见 README，长期工程规则见 docs/dev/README.md。

## 内容与机制

设计案内容侧**全量落地**：95 张卡（初始 4 / 先古 2 / 普通 20 / 罕见 36 / 稀有 26 / 新闻 7）、遗物 7、药水 4、三机制闭环。本地与游戏内冒烟验收通过（2026-10-02）。

| 类别 | 实现情况 |
|---|---|
| 机制 | [留影]（Snapshot 收口 + SnapshotMemento 标记，随存档序列化）、[聚焦]（LensFocusPower + Focus.TriggerAll，Unblockable 掉血、持有者回合末 -1；类名避原版宝珠 FocusPower 撞名）、[新闻]（News.Play 收口：效果→聚焦→观察者） |
| 观察者分发 | ISnapshotObserver / INewsObserver / IFocusApplyObserver / IFocusDamageObserver；唇枪舌剑直接用原生回合钩子回归 |
| 卡牌 | 花名册 95/95/0 全实装；新闻牌 Token 稀有度无色池不进奖励；计算三件套契约全守齐（四张残缺已修） |
| 遗物 | 7 件全实装：温亨廷先生（初始）/ 千织屋纪念版（Refinement）/ 特殊分析变焦镜头 / 镜头盖 / 采访稿记录本（世界线解锁件暂挂 Event 稀有度）/ 大份的炸鱼薯条 / 维修工具（Harmony 花费追踪 + SavedProperty 计数） |
| 药水 | 4 件全实装：热咖啡 / 镜头清洁剂 / 枫丹洋葱汤（普通池）/ 提神醒脑茶（解锁件暂挂 Event） |
| 角色 | HP 70，主题色 #8CCEEA 全接线（卡框 HSV/名字/描边/对话/地图）；能量球三面覆盖（战斗能量计自建场景 + big/text 池覆写）；头像图标/描边/地图标记已用自备素材（characters.sh 一图三用） |
| 关键词 | 留影/聚焦/新闻双语悬停注册 |

## 验收证据

- 2026-10-10 **0.1.5 报告五项修复**：动作捕捉升级新增固有＋保留；神来之笔扩至抽牌堆＋弃牌堆；鞭辟入里／跟进调查升级增加保留，原有数值升级保留；唇枪舌剑回归改由卡牌原生 BeforeSideTurnStart 处理，删除 0 层施加导致从未挂载的隐藏扫描能力及遗物接线。源码检查、双目标完整构建、完整素材与 PCK 561 文件验证通过。0.107.1／0.111.0 隔离 Godot 宿主（真实 CardPlay／回合流，RitsuLib 0.6.6）逐项通过：两堆 5 张新闻→5 次 4／6 伤害；升级三牌实际保留而基础牌弃置；无初始遗物时基础／升级唇枪舌剑两轮回手增伤为 5→7／6→9，非消耗堆卡不增伤；跟进调查抽 1／2，动作捕捉抽 1 耗 1。tag v0.1.5（源码 11496d5）推送后源码／编译／Release CI 全绿；草稿 6 件资产下载校验、双目标 DLL 对应及 PCK 561 个解密资源内容一致性验证通过，CI 工件双目标报告用例复测通过后公开发布。工坊物品 3812082715 使用同一 Release 工件更新成功，随后从 Steam 重新下载的 9 件文件与 GitHub 工坊包逐字节一致；线上简介与双语改动说明完整匹配本地文本，旧简介被双引号截断的日志尾段和链接已恢复。隔离重建宿主不能替代发行游戏像素、多人和长流程验收。


- 2026-10-05 **0.1.4 双端发版**：海玻璃按角色分键修复（SEA_GLASS.STS2_CHARLOTTE_CHARACTER_CHARLOTTE.title = 镜头玻璃/Lens Glass，模板 issue#1 口径，Navia/Dehya 同款先例）。tag v0.1.4 → CI 全绿 → GitHub Release 发布（sha256 复核一致，verify-pck 对 CI 产物实地解析通过）；工坊物品同日更新，描述 Change Log 段补 0.1.4 条目。**changenote 管线定论**：steamcmd VDF 本身支持 changenote——真正的原因是模板脚手架的 VDF 多带 title/visibility/previewfile 三键导致 changenote 被静默丢弃（0.1.4 全键 VDF 上传成功但条目空白；Navia 0.3.0/0.4.0/0.4.1 用 5 键最小形式说明全部正常，含多行中文）。publish.sh 已对齐 Navia 最小键形式（代价：标题/可见性/封面转网页端维护）。0.1.3/0.1.4 两次更新的说明已随旧形式丢失，网页端「编辑」补录或随下版自然覆盖；MegaCrit 官方 sts2-mod-uploader 已备在 local_dev/tools 作为备选（需运行中的 Steam 客户端，本机 Linux 无客户端）。
- 2026-10-05 **0.1.3 紧急热修发版**：0.1.2 留影纪念双贴片以两参形式指向 CardModel 属性，Harmony 不解析 getter → PatchAll 抛错 → Init 中断 → 启动 ModelNotFoundException（0.1.2 全体用户受影响；离线探针复现并验证修复，5586fc7 补 MethodType.Getter）。tag v0.1.3 → CI 全绿 → GitHub Release 发布（6 件资产，sha256 复核一致，包内清单 0.1.3，DLL 含补丁类）。同日工坊物品 3812082715 重发完成：描述与线上业主手订版逐字核对一致后补双语「更新日志 / Change Log」段（0.1.3 热修 + 0.1.2 摘要 + GitHub Releases 链接），首次上传遇断连（result 3 No Connection）重试即成功，API 复核 time_updated 与描述已生效，订阅侧自动更新。
- 2026-10-03 **0.1.2 双端发版**：tag v0.1.2 → CI（Source checks/Compile/Release build 全绿）→ GitHub Release 发布（CHANGELOG 段落为发行说明，6 件资产；与本地包文件树逐项一致，DLL/PCK 差异为跨机构建不可复现项）；工坊物品 3812082715 经缓存凭据从 Release 工件更新成功，线上简介/署名为业主手订版（零 AI 字样，双 Credit 完整），订阅侧自动更新。同批收口精彩纷呈入堆路由 bug（引擎在 OnPlay 前预计算去向，OnPlay 内补标记赶不上路由；改走 GetResultLocationForCardPlay 覆写，双目标同缝异形垫片）。
- 2026-10-03 设计裁决复批第二批（0.1.2）：Mirror 全部复批 14+3 题——A1 留影纪念真无色+统一卡名（VisualCardPool/TitleLocString 双贴片，原池不动）、A5 谨遵事实合作口径（观察者全场玩家）、C2 按下快门文案消歧（行为不动）、A2 附议力透纸背金光高亮、D3 纪念标记悬挂九处；**A7/C1 推翻 0.1.1 代决**（真实至上撤 GainsBlock、连续抓拍回先伤后影）；B 组按原版「有消耗就消耗」口径复核未回退，修两处偏差（加班加点能量无条件、不足 N 按现存张数）+ C4 实装缺口（动作捕捉只耗 1 张→全部消耗）。双目标编译绿；游戏内验收通过（业主 2026-10-03）。
- 2026-10-03 工坊封面：已参照 Navia 同系列版式补齐含中英文标题的 Charlotte 封面（1024×1024 JPEG，旧图已备份）；随后第二轮工坊上传已生效——简介（对齐 Navia 现行版含 Credits 段）与新封面均已在线验证（物品 3812082715）。
- 2026-10-03 美术补齐：源码检查、完整素材检查、0.111.0 编译及 PCK 验证通过；新增事件/纪元/能力纹理可加载，尺寸及透明通道断言通过。已随 ee49b44 部署，游戏内验收待做。
- 2026-10-02/03 全卡审阅：95 张逐张（设计案↔代码↔双语卡面三层 + 机制链条契约），对抗核验后 72 证实——高/中危 34 项（605d129）与纯工程低危 5 项（0f84fdd）已修，其余 33 项低危中 14 题整理为设计裁决清单待 Mirror（`local_dev/.../卡牌审阅裁决清单（转Mirror）.md`），其余依赖裁决结论；原始发现存档 `local_dev/.../卡牌审阅原始发现-20261002.json`。修复含 X 费正确范式（HasEnergyCostX）、三起在堆卡 API 误用、唇枪舌剑/多重取证补消耗、面板可见的衰减/成长（DamageVar 直改）、聚焦续层触发（AfterPowerAmountChanged）等。
- 本地门禁：check.sh --source-only、双目标 build（0.111.0/0.107.1）、audit-assets 全模式（139→0 缺口清零）、export-card-table 全绿（2026-10-02）。
- 端到端：package.sh 双变体打包 → 变体布局部署（Loader 壳 + lib/game-*/）→ 游戏内启动、开局、战斗、击杀后奖励屏、卡池掉落冒烟通过（2026-10-02，用户验收）。
- 修复记录：① Harmony 透传 postfix 返回类型校验炸初始化（改 async void）；② 四张卡计算三件套残缺致奖励生成 KeyNotFound 软锁；③ 单刀直入/有力物证误用格挡系 CalculationExtra 键喂 CalculatedDamageVar——战斗 Calculate 与奖励/商店预览镜像（RandomForeseer 的 Populate 族补丁）KeyNotFound('ExtraDamage')，表现为商店商品槽损坏连环 NRE 与战后奖励三缺一（2026-10-02，已修并对齐 PerfectedStrike 形态；三件套配对契约入 audit-placeholders 门禁）；④ 新闻池未注册为共享池——新闻卡实例的 Pool 懒查找只在 AllCardPools（角色池+共享池）里找归属，生成入堆/镜像预览即抛 InvalidProgramException 炸断出牌（占领头版卡死空中+异常风暴卡顿，2026-10-02 已修，[RegisterSharedCardPool]）。角色小人姿势切换（攻击/施法/受击）为非 Spine 路线自接（SetAnimationTrigger 对 Sprite 小人 no-op 的 postfix 切纹理）；⑤ 早间特报升级版 7 张新闻喂给硬上限 3 张的 FromChooseACardScreen——ArgumentException 炸断出牌（卡悬空中），4 张以上改走 FromSimpleGrid 网格选择器（选卡器分档契约入 docs/dev/cards.md）。

## 限制与已知事项

- 角色立绘母版（7 姿势）、选人背景、头像/地图标记/能量球已有自备素材；选人半身新增专属透明母版。全部已部署，游戏内验收待做。
- 卡面右上角费用底球走原版共享图集回退（mod 无法扩原版图集，与 Navia 正式版一致）；能量文本字模为回退告警（可请设计师补一张字模纹理根除）。
- 设计语义 12 问已全部经 Mirror 确认闭环（2026-10-02，含高危调查/奋笔疾书升级补正、透镜高塔/药水快递员奖励=原版遗物）；决议记录见 `local_dev/杀戮尖塔2——夏洛蒂角色mod/待确认问题与素材缺口（转Mirror）.md`。
- 三个事件肖像、四章世界线立绘与缩略图、「墨迹未干」专属图标及透明选人半身已补齐并接线（2026-10-03，ee49b44）；「摄影技巧」保留已有正式相机图标。新增画面尚未游戏内验收。
- 未做过游戏内长流程回归（多层推进/多场战斗/存档中断恢复），首版验收为冒烟深度。
- 14 题设计裁决已于 0.1.2 复批落地；历史测试分发包不代表当前发行物。
