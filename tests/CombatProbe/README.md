# 联机卡牌确定性战斗探针

本探针使用真实游戏、RitsuLib 和 Charlotte 程序集，调用实际卡牌与 Power 钩子。最小夹具组装玩家、敌人和战斗牌堆；只隔离动画、日志、抽牌／加牌等命令边界。保留真实生成牌归属、卡牌克隆、留影标记、Power 实例数据和模型升级。

覆盖五张 Mirror 联机卡的限定标记与升级值、咔嚓的接收者／创建者／费用／虚无、众目睽睽逐玩家首次未格挡攻击伤害、随行采访的一次性消耗触发、同行特刊双方独立的每回合触发，以及此刻长存的混合手牌留影与全体格挡。虚无消耗按原版黑暗拥抱口径验证回合末延后抽牌。

## 执行

在 Bash 中从仓库根运行。每个游戏目标分别编译并启动新进程，设置一致的游戏引用、RitsuLib 目标和完整运行时 DLL 目录：

```bash
source scripts/dev-env.sh
export RITSULIB_TARGET=0.111.0
# GAME_REFS_DIR 默认由 .local-dev.env 提供；也可在此覆盖。
"$DOTNET_EXE" build tests/CombatProbe/CombatProbe.csproj -c Release
"$DOTNET_EXE" tests/CombatProbe/bin/Release/net9.0/CombatProbe.dll \
  "$GAME_REFS_DIR" "$GAME_DIR/data_sts2_windows_x86_64" "$RITSULIB_DIR" "$RITSULIB_TARGET"
```

0.107.1 使用对应的引用、运行时目录和 `RITSULIB_TARGET`。实际本机变量以 `.local-dev.env` 为准；运行时目录还须包含 RitsuLib 的附加依赖。

## 证据边界

探针不启动真实网络、完整出牌队列或 Godot 战斗场景，不能证明单人卡池 UI、联机选择同步、手牌隐私、跨玩家动画时序、死亡流程、存档恢复或新图游戏内显示。上述项目仍需 Mirror 在真实两人／三人局验收。Loader 注册接线由独立的 [LoaderProbe](../LoaderProbe/README.md) 验证。
