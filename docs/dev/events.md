# 事件实现（可选模块）

骨架不含事件内容；需要时按本页接线。事件基线类 `Content/Events/<Short>EventBase.cs`。

## 注册与门控

新事件继承 `<Short>EventBase`，由 RitsuLib 的 RegisterActEvent 注册到对应 Act。IsAllowed 查询队伍中是否有本角色——「队伍含本角色」与「事件归属者是本角色」（原版追加选项查询事件 Owner.Character）是**两种不同门控**，多人验收应覆盖两者。

## 原版追加选项

给原版事件追加角色专属选项：在 `EventModel.GenerateInitialOptionsWrapper` 基方法上打 Harmony postfix（参考源工程的 `VanillaEventNaviaOptions`）。要点：

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
