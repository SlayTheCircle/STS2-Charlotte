using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using CharlotteMod.Content.Afflictions;
using CharlotteMod.Content.CardPools;

namespace CharlotteMod.Content.Patches;

/// <summary>
/// [留影纪念]身份贴片(设计师 2026-10-03 A1/D2 裁决):纪念克隆统一为「名为留影纪念的无色牌」,
/// 卡图/效果/费用/类型保持原牌。两条贴片都只认 <see cref="SnapshotMemento"/> 标记,不动 Pool——
/// 原池是卡图解析(PortraitPath 按 Pool.Title 找目录)、[新闻]判据与池过滤管线的事实源,
/// 换池会同时弄丢原图与新闻克隆的 IsNews 命中;视觉走 vanilla 留的 VisualCardPool 缝
/// (Trash Heap 事件卡借角色卡框的同款机制),指向 CharlotteNewsPool(无色卡框,能量配色不变)。
/// 补丁由 ModEntry.Init 的显式 PatchAll 挂载。
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.VisualCardPool))]
internal static class SnapshotMementoVisualPoolPatch
{
    private static void Postfix(CardModel __instance, ref CardPoolModel __result)
    {
        if (__instance.Affliction is SnapshotMemento)
        {
            __result = ModelDb.CardPool<CharlotteNewsPool>();
        }
    }
}

/// <summary>
/// 卡名统一:纪念克隆的 Title 定位到纪念词条 loc(「留影纪念/Snapshot Souvenir」,与纪念标记
/// 悬停共用同一份文案),Title getter 的升级 +N 后缀逻辑照常生效。原牌名只在战斗记录等
/// Title 渲染处出现,均走本属性,无第二事实源。
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.TitleLocString))]
internal static class SnapshotMementoTitlePatch
{
    private static readonly LocString MementoTitle =
        new("afflictions", "STS2_CHARLOTTE_AFFLICTION_SNAPSHOT_MEMENTO.title");

    private static bool Prefix(CardModel __instance, ref LocString __result)
    {
        if (__instance.Affliction is SnapshotMemento)
        {
            __result = MementoTitle;
            return false;
        }
        return true;
    }
}
