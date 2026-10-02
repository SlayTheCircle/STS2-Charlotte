# 当前状态（2026-10-03 · 全卡审阅闭环 + 美术补齐；待游戏内验收与设计裁决）

本文件维护当前实现、证据边界、限制与待办。安装入口见 README，长期工程规则见 docs/dev/README.md。

## 内容与机制

设计案内容侧**全量落地**：95 张卡（初始 4 / 先古 2 / 普通 20 / 罕见 36 / 稀有 26 / 新闻 7）、遗物 7、药水 4、三机制闭环。本地与游戏内冒烟验收通过（2026-10-02）。

| 类别 | 实现情况 |
|---|---|
| 机制 | [留影]（Snapshot 收口 + SnapshotMemento 标记，随存档序列化）、[聚焦]（LensFocusPower + Focus.TriggerAll，Unblockable 掉血、持有者回合末 -1；类名避原版宝珠 FocusPower 撞名）、[新闻]（News.Play 收口：效果→聚焦→观察者） |
| 观察者分发 | ISnapshotObserver / INewsObserver / IFocusApplyObserver / IFocusDamageObserver / IExileReturner（消耗堆回归） |
| 卡牌 | 花名册 95/95/0 全实装；新闻牌 Token 稀有度无色池不进奖励；计算三件套契约全守齐（四张残缺已修） |
| 遗物 | 7 件全实装：温亨廷先生（初始）/ 千织屋纪念版（Refinement）/ 特殊分析变焦镜头 / 镜头盖 / 采访稿记录本（世界线解锁件暂挂 Event 稀有度）/ 大份的炸鱼薯条 / 维修工具（Harmony 花费追踪 + SavedProperty 计数） |
| 药水 | 4 件全实装：热咖啡 / 镜头清洁剂 / 枫丹洋葱汤（普通池）/ 提神醒脑茶（解锁件暂挂 Event） |
| 角色 | HP 70，主题色 #8CCEEA 全接线（卡框 HSV/名字/描边/对话/地图）；能量球三面覆盖（战斗能量计自建场景 + big/text 池覆写）；头像图标/描边/地图标记已用自备素材（characters.sh 一图三用） |
| 关键词 | 留影/聚焦/新闻双语悬停注册 |

## 验收证据

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
- 14 题设计裁决（留影纪念颜色/选牌空过/结算顺序等）待 Mirror 回复后批量实装；测试分发通道为 local_dev 测试分发（当前 d 包）。
