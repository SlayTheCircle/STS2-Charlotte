using System;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Models;
using CharlotteMod.Content.Relics;

namespace CharlotteMod.Content.Patches;

/// <summary>
/// 维修工具的花费追踪:PlayerCmd.LoseGold 没有模型钩子(只有 AfterGoldGained),只能 Harmony 收口。
/// 只计 Spent 型失金(购买/移卡),赌输/被偷不计。由 ModEntry 的 PatchAll 装配。
/// 注意:postfix 必须是 void/async void——带 Task 返回值会被 Harmony 判为透传 postfix,
/// 其返回类型校验直接抛 HarmonyException 炸初始化(2026-10-02 冒烟实测)。
/// async void 延续在调用方的同步上下文(游戏主循环)上恢复,异常就地捕获记日志。
/// </summary>
[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.LoseGold))]
public static class RepairToolsLoseGoldPatch
{
    public static async void Postfix(decimal amount, MegaCrit.Sts2.Core.Entities.Players.Player player, GoldLossType goldLossType)
    {
        if (goldLossType != GoldLossType.Spent || amount <= 0m)
        {
            return;
        }
        foreach (RelicModel relic in player.Relics)
        {
            if (relic is RepairTools tools)
            {
                try
                {
                    await tools.OnGoldSpent(amount, player);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"[STS2-Charlotte] 维修工具花费追踪失败: {e}");
                }
            }
        }
    }
}
