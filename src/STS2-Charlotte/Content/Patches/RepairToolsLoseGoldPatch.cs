using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Gold;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using CharlotteMod.Content.Relics;

namespace CharlotteMod.Content.Patches;

/// <summary>
/// 维修工具的花费追踪:PlayerCmd.LoseGold 没有模型钩子(只有 AfterGoldGained),只能 Harmony 收口。
/// 只计 Spent 型失金(购买/移卡),赌输/被偷不计。由 ModEntry 的 PatchAll 装配。
/// </summary>
[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.LoseGold))]
public static class RepairToolsLoseGoldPatch
{
    public static async Task Postfix(decimal amount, MegaCrit.Sts2.Core.Entities.Players.Player player, GoldLossType goldLossType)
    {
        if (goldLossType != GoldLossType.Spent || amount <= 0m)
        {
            return;
        }
        foreach (RelicModel relic in player.Relics)
        {
            if (relic is RepairTools tools)
            {
                await tools.OnGoldSpent(amount, player);
            }
        }
    }
}
